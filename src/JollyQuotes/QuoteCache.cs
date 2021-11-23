using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace JollyQuotes
{
	/// <summary>
	/// Represents a list-based cache of quotes.
	/// </summary>
	/// <typeparam name="T">Type of quotes this class can store.</typeparam>
	public class QuoteCache<T> : IQuoteCache<T> where T : class, IQuote
	{
		[DebuggerDisplay("{Lookup}, nq")]
		private sealed class TagCacheEntry : IEnumerable<int>
		{
			private readonly Dictionary<int, int> _map;
			public int Count => _map.Count;

			public bool IsEmpty => Count == 0;

			public List<int> Lookup { get; }

			public TagCacheEntry()
			{
				Lookup = new();
				_map = new();
			}

			public void Add(int lookupIndex)
			{
				if (_map.TryAdd(lookupIndex, Lookup.Count))
				{
					Lookup.Add(lookupIndex);
				}
			}

			public void Clear()
			{
				_map.Clear();
				Lookup.Clear();
			}

			public List<int>.Enumerator GetEnumerator()
			{
				return Lookup.GetEnumerator();
			}

			public int GetLookupIndex(int at)
			{
				return Lookup[at];
			}

			public bool Remove(int lookupIndex)
			{
				if (_map.Remove(lookupIndex, out int value))
				{
					Lookup.RemoveAt(value);
					return true;
				}

				return false;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator<int> IEnumerable<int>.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		// Maximal number of times a number can be regenerated until calling Defragment().
		private const int _maxRandomMismatch = 3;

		// If number of values in the _removed list is less than this value, only one random mismatch is allowed.
		private const int _minRemovedForMismatch = 3;

		private readonly object _lockObject;
		private readonly Dictionary<int, int> _map;
		private readonly List<int> _removed;
		private bool _isSorted;
		private List<T?> _lookup;
		private Dictionary<string, TagCacheEntry>? _tagCache;

		/// <inheritdoc/>
		public int Count
		{
			get
			{
				lock (_lockObject)
				{
					return _map.Count;
				}
			}
		}

		/// <summary>
		/// <see cref="IEqualityComparer{T}"/> that is used to decide whether two <see cref="IQuote"/>s are equal.
		/// </summary>
		public IEqualityComparer<T> EqualityComparer { get; }

		/// <inheritdoc/>
		public bool IsEmpty => Count == 0;

		/// <summary>
		/// Service that generates a value used to determine which random quote to return.
		/// </summary>
		public IRandomNumberGenerator RandomNumberGenerator { get; }

		/// <summary>
		/// Determines whether to use a cache of tags. Will speed up tag-related operations, such as <see cref="TryGetRandomQuote(string, out T?, bool)"/>,
		/// but will have negative impact on performance of enumerator-based methods (e.g. <see cref="Defragment()"/>, <see cref="GetCached()"/> or <see cref="GetEnumerator()"/>).
		/// </summary>
		/// <remarks>The default value is <see langword="false"/>.</remarks>
		[MemberNotNullWhen(true, nameof(_tagCache))]
		public bool UseTagCache
		{
			get
			{
				lock (_lockObject)
				{
					return _tagCache is not null;
				}
			}
			set
			{
				if (value)
				{
					lock (_lockObject)
					{
						if (_tagCache is not null)
						{
							return;
						}

						InitializeTagCache();
					}
				}
				else
				{
					lock (_lockObject)
					{
						if (_tagCache is not null)
						{
							_tagCache = null;
						}
					}
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class.
		/// </summary>
		public QuoteCache() : this(new ThreadRandom(), EqualityComparer<T>.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class with an <paramref name="equalityComparer"/> specified.
		/// </summary>
		/// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> that is used to decide whether two <see cref="IQuote"/>s are equal.</param>
		/// <exception cref="ArgumentNullException"><paramref name="equalityComparer"/> is <see langword="null"/>.</exception>
		public QuoteCache(IEqualityComparer<T> equalityComparer) : this(new ThreadRandom(), equalityComparer)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class with a <paramref name="randomNumberGenerator"/> specified.
		/// </summary>
		/// <param name="randomNumberGenerator">Service that generates a value used to determine which random quote to return.</param>
		/// <exception cref="ArgumentNullException"><paramref name="randomNumberGenerator"/> is <see langword="null"/>.</exception>
		public QuoteCache(IRandomNumberGenerator randomNumberGenerator) : this(randomNumberGenerator, EqualityComparer<T>.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class with a <paramref name="randomNumberGenerator"/> and <paramref name="equalityComparer"/> specified.
		/// </summary>
		/// <param name="randomNumberGenerator">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> that is used to decide whether two <see cref="IQuote"/>s are equal.</param>
		/// <exception cref="ArgumentNullException"><paramref name="randomNumberGenerator"/> is <see langword="null"/>. -or- <paramref name="equalityComparer"/> is <see langword="null"/>.</exception>
		public QuoteCache(IRandomNumberGenerator randomNumberGenerator, IEqualityComparer<T> equalityComparer)
		{
			if (randomNumberGenerator is null)
			{
				throw Error.Null(nameof(randomNumberGenerator));
			}

			if (equalityComparer is null)
			{
				throw Error.Null(nameof(equalityComparer));
			}

			_lockObject = new();
			_map = new();
			_lookup = new();
			_removed = new();

			RandomNumberGenerator = randomNumberGenerator;
			EqualityComparer = equalityComparer;
		}

		/// <summary>
		/// Adds the specified <paramref name="quote"/> to the cache.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to cache.</param>
		/// <param name="replace">Determines whether to replace an <see cref="IQuote"/> with the same id.</param>
		/// <returns><see langword="true"/> if the quote was successfully cached, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="quote"/> is <see langword="null"/>.</exception>
		public bool CacheQuote(T quote, bool replace = false)
		{
			if (quote is null)
			{
				throw Error.Null(nameof(quote));
			}

			if (replace)
			{
				return TryCacheOrReplace(quote);
			}

			return TryCache(quote);
		}

		/// <summary>
		/// Adds the specified <paramref name="quotes"/> to the cache.
		/// </summary>
		/// <param name="quotes">A collection of <see cref="IQuote"/>s to add.</param>
		/// <param name="replace">Determines whether to replace an <see cref="IQuote"/> with the same id.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quotes"/> is <see langword="null"/>.</exception>
		public void CacheQuotes(IEnumerable<T> quotes, bool replace = false)
		{
			if (quotes is null)
			{
				throw Error.Null(nameof(quotes));
			}

			if (replace)
			{
				foreach (T quote in quotes)
				{
					if (quote is null)
					{
						continue;
					}

					TryCacheOrReplace(quote);
				}
			}
			else
			{
				foreach (T quote in quotes)
				{
					if (quote is null)
					{
						continue;
					}

					TryCache(quote);
				}
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			lock (_lockObject)
			{
				_lookup.Clear();
				_map.Clear();
				_removed.Clear();
				_tagCache?.Clear();
				_isSorted = false;
			}
		}

		/// <summary>
		/// Removes accumulated garbage memory still held by the cache.
		/// </summary>
		public void Defragment()
		{
			if (_removed.Count == 0)
			{
				return;
			}

			lock (_lockObject)
			{
				DefragmentInternal();
			}
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached()
		{
			lock (_lockObject)
			{
				DefragmentInternal();

				return _lookup.ToArray()!;
			}
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			return GetCachedInternal(tag);
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return Array.Empty<T>();
			}

			if (tags.Length == 1)
			{
				string tag = tags[0];

				if (string.IsNullOrWhiteSpace(tag))
				{
					return Array.Empty<T>();
				}

				return GetCachedInternal(tag);
			}

			string[] tagCollection = tags.Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();

			lock (_lockObject)
			{
				if (_tagCache is null)
				{
					return GetQuotesWithTagsWithoutTagCache(tagCollection);
				}
				else if (_tagCache.Count == 0)
				{
					return Array.Empty<T>();
				}

				return GetQuotesWithTagsWithTagCache(tagCollection);
			}
		}

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			lock (_lockObject)
			{
				DefragmentInternal();

				return _lookup.GetEnumerator();
			}
		}

		/// <summary>
		/// Attempts to return an <see cref="IQuote"/> by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of <see cref="IQuote"/> to return.</param>
		/// <exception cref="ArgumentException">Quote with <paramref name="id"/> not found.</exception>
		/// <exception cref="InvalidOperationException">Cannot return a quote from empty cache.</exception>
		public T GetQuote(int id)
		{
			lock (_lockObject)
			{
				EnsureNotEmpty();

				if (!_map.TryGetValue(id, out int index))
				{
					throw new ArgumentException($"Quote with id '{id}' not found");
				}

				return _lookup[index]!;
			}
		}

		/// <summary>
		/// Returns a random <see cref="IQuote"/> from the cache.
		/// </summary>
		/// <param name="remove">Determines whether to remove the returned quote from the cache.</param>
		/// <exception cref="InvalidOperationException">Cannot return a quote from empty cache.</exception>
		public T GetRandomQuote(bool remove = false)
		{
			lock (_lockObject)
			{
				EnsureNotEmpty();

				T quote = GetRandomQuoteInternal(out int index);

				if (remove)
				{
					RemoveQuoteAtIndex(quote, index);
					TryRemoveQuoteFromTagCache(quote, index);
				}

				return quote;
			}
		}

		/// <inheritdoc/>
		public bool IsCached(T quote)
		{
			if (quote is null)
			{
				throw Error.Null(nameof(quote));
			}

			int id = quote.GetId();
			return TryGetQuote(id, out T? other) && EqualityComparer.Equals(other, quote);
		}

		/// <summary>
		/// Determines whether an <see cref="IQuote"/> with the specified <paramref name="id"/> is to be found in the cache.
		/// </summary>
		/// <param name="id">Id of <see cref="IQuote"/> to check for.</param>
		public bool IsCached(int id)
		{
			lock (_lockObject)
			{
				return _map.ContainsKey(id);
			}
		}

		/// <inheritdoc/>
		public bool RemoveQuote(T quote)
		{
			if (quote is null)
			{
				throw Error.Null(nameof(quote));
			}

			int id = quote.GetId();

			lock (_lockObject)
			{
				if (_map.TryGetValue(id, out int index))
				{
					T other = _lookup[index]!;

					if (!EqualityComparer.Equals(other, quote))
					{
						return false;
					}

					_map.Remove(id);
					RemoveIndex(index);
					TryRemoveQuoteFromTagCache(quote, index);

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Removes an <see cref="IQuote"/> with the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id <see cref="IQuote"/> to remove.</param>
		public bool RemoveQuote(int id)
		{
			return RemoveQuote(id, out _);
		}

		/// <summary>
		/// Removes an <see cref="IQuote"/> with the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id <see cref="IQuote"/> to remove.</param>
		/// <param name="quote"><see cref="IQuote"/> that was removed.</param>
		public bool RemoveQuote(int id, [NotNullWhen(true)] out T? quote)
		{
			lock (_lockObject)
			{
				if (_map.Remove(id, out int index))
				{
					quote = _lookup[index]!;
					RemoveIndex(index);
					TryRemoveQuoteFromTagCache(quote, index);

					return true;
				}

				quote = default;
				return false;
			}
		}

		/// <inheritdoc/>
		public bool RemoveQuotes(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			lock (_lockObject)
			{
				int count = _map.Count;

				if (count == 0)
				{
					return false;
				}

				if (_tagCache is null)
				{
					DefragmentInternal();

					int length = _lookup.Count;

					for (int i = 0; i < length; i++)
					{
						T? quote = _lookup[i];

						if (quote is null || Array.IndexOf(quote.Tags, tag) == -1)
						{
							continue;
						}

						RemoveQuoteAtIndex(quote, i);
					}
				}
				else
				{
					if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry))
					{
						return false;
					}

					foreach (int index in entry)
					{
						T? quote = _lookup[index];

						if (quote is null)
						{
							continue;
						}

						RemoveQuoteAtIndex(quote, index);
						RemoveQuoteFromTagCache(quote, index);
					}
				}

				return _map.Count < count;
			}
		}

		/// <summary>
		/// Removes all quotes with the specified <paramref name="tag"/> from the cache.
		/// </summary>
		/// <param name="tag">Tag remove all quotes associated with.</param>
		/// <param name="removed">Array containing <see cref="IQuote"/>s that were removed from the cache.</param>
		/// <returns><see langword="true"/> if any quote associated with the specified <paramref name="tag"/> was successfully removed, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		public bool RemoveQuotes(string tag, [NotNullWhen(true)] out T[]? removed)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			lock (_lockObject)
			{
				int count = _map.Count;

				if (count == 0)
				{
					removed = null;
					return false;
				}

				List<T> quotes;

				if (_tagCache is null)
				{
					DefragmentInternal();

					int length = _lookup.Count;

					quotes = new(length);

					for (int i = 0; i < length; i++)
					{
						T? quote = _lookup[i];

						if (quote is null || Array.IndexOf(quote.Tags, tag) == -1)
						{
							continue;
						}

						quotes.Add(quote);
						RemoveQuoteAtIndex(quote, i);
					}
				}
				else
				{
					if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry))
					{
						removed = null;
						return false;
					}

					quotes = new(entry.Count);

					foreach (int index in entry)
					{
						T? quote = _lookup[index];

						if (quote is null)
						{
							continue;
						}

						quotes.Add(quote);
						RemoveQuoteAtIndex(quote, index);
						RemoveQuoteFromTagCache(quote, index);
					}
				}

				removed = quotes.Count > 0 ? quotes.ToArray() : null;
				return _map.Count < count;
			}
		}

		/// <summary>
		/// Attempts to return an <see cref="IQuote"/> by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of <see cref="IQuote"/> to return.</param>
		/// <param name="quote">Returned <see cref="IQuote"/>.</param>
		public bool TryGetQuote(int id, [NotNullWhen(true)] out T? quote)
		{
			lock (_lockObject)
			{
				if (_map.TryGetValue(id, out int index))
				{
					quote = _lookup[index]!;
					return true;
				}
			}

			quote = null;
			return false;
		}

		/// <summary>
		/// Attempts to return a random <see cref="IQuote"/> from the cache associated with the specified <paramref name="tag"/> or
		/// <see langword="null"/> if there is no such <see cref="IQuote"/> in the cache.
		/// </summary>
		/// <param name="tag">Tag to generate a quote associated with.</param>
		/// <param name="quote">Returned <see cref="IQuote"/>.</param>
		/// <param name="remove">Determines whether to remove the returned quote from the cache.</param>
		/// <returns><see langword="true"/> if a <see cref="IQuote"/> associated with the specified <paramref name="tag"/> was found, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		public bool TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote, bool remove = false)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			if (_lookup.Count == 0)
			{
				quote = null;
				return false;
			}

			lock (_lockObject)
			{
				if (_lookup.Count == 0)
				{
					quote = null;
					return false;
				}

				List<int>? quotes = GetCachedWithTag(tag);

				if (quotes is null || quotes.Count == 0)
				{
					quote = null;
					return false;
				}

				quote = GetRandomQuoteFromIndexList(quotes, out int randomIndex);

				if (remove)
				{
					RemoveQuoteAtIndex(quote, randomIndex);
					TryRemoveQuoteFromTagCache(quote, randomIndex);
				}

				return true;
			}
		}

		void IQuoteCache<T>.CacheQuote(T quote)
		{
			CacheQuote(quote);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		T IQuoteCache<T>.GetRandomQuote()
		{
			return GetRandomQuote();
		}

		bool IQuoteCache<T>.TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote)
		{
			return TryGetRandomQuote(tag, out quote);
		}

		private void CacheTag(string tag, int index)
		{
			if (_tagCache!.TryGetValue(tag, out TagCacheEntry? entry))
			{
				entry.Add(index);
			}
			else
			{
				entry = new();
				entry.Add(index);
				_tagCache.Add(tag, entry);
			}
		}

		private void DefragmentInternal()
		{
			if (_removed.Count == 0)
			{
				return;
			}

			SortRemoved();

			int count = _removed.Count;
			int lookUpCount = _lookup.Count;
			int current = _removed[0];
			int numRemoved = 1;

			List<T> quotes = new(_map.Count);

			for (int quoteIndex = 0; quoteIndex < current; quoteIndex++)
			{
				quotes.Add(_lookup[quoteIndex]!);
			}

			for (int i = 1; i < count; i++)
			{
				int next = _removed[i];

				for (int quoteIndex = current + 1; quoteIndex < next; quoteIndex++)
				{
					MoveQuote(quoteIndex, numRemoved);
				}

				current = next;
				numRemoved++;
			}

			for (int quoteIndex = current + 1; quoteIndex < lookUpCount; quoteIndex++)
			{
				MoveQuote(quoteIndex, numRemoved);
			}

			_removed.Clear();
			_lookup = quotes!;

			void MoveQuote(int quoteIndex, int numRemoved)
			{
				T quote = _lookup[quoteIndex]!;
				int id = quote.GetId();
				quotes.Add(quote);

				_map[id] -= numRemoved;
			}
		}

		private void EnsureNotEmpty()
		{
			if (_map.Count == 0)
			{
				throw new InvalidOperationException("Cannot return a quote from empty cache");
			}
		}

		private IEnumerable<T> GetCachedInternal(string tag)
		{
			lock (_lockObject)
			{
				List<int>? indices = GetCachedWithTag(tag);

				if (indices is null)
				{
					return Array.Empty<T>();
				}

				int length = indices.Count;
				T[] quotes = new T[length];

				for (int i = 0; i < length; i++)
				{
					int index = indices[i];
					quotes[i] = _lookup[index]!;
				}

				return quotes;
			}
		}

		private List<int> GetCachedWithoutTagCache(string tag)
		{
			int length = _lookup.Count;
			List<int> indices = new(length);

			for (int i = 0; i < length; i++)
			{
				T quote = _lookup[i]!;

				if (Array.IndexOf(quote.Tags, tag) > -1)
				{
					indices.Add(i);
				}
			}

			return indices;
		}

		private List<int>? GetCachedWithTag(string tag)
		{
			if (_tagCache is null)
			{
				DefragmentInternal();
				return GetCachedWithoutTagCache(tag);
			}
			else if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry))
			{
				return null;
			}
			else
			{
				return entry.Lookup;
			}
		}

		private int GetMaxMismatchNumber()
		{
			return _removed.Count >= _minRemovedForMismatch ? _maxRandomMismatch : 1;
		}

		private List<T> GetQuotesWithTagsWithoutTagCache(string[] tagCollection)
		{
			List<T> quotes = new(_lookup.Count);
			DefragmentInternal();

			foreach (T? quote in _lookup)
			{
				foreach (string tag in tagCollection)
				{
					if (Array.IndexOf(quote!.Tags, tag) > -1)
					{
						quotes.Add(quote);
						break;
					}
				}
			}

			return quotes;
		}

		private List<T> GetQuotesWithTagsWithTagCache(string[] tagCollection)
		{
			List<T> quotes = new(_lookup.Count);
			HashSet<int> used = new();

			foreach (string tag in tagCollection)
			{
				if (!_tagCache!.TryGetValue(tag, out TagCacheEntry? entry))
				{
					continue;
				}

				foreach (int index in entry)
				{
					if (used.Add(index))
					{
						T quote = _lookup[index]!;
						quotes.Add(quote);
					}
				}
			}

			return quotes;
		}

		private int GetRandomIndex(int max)
		{
			return RandomNumberGenerator.RandomNumber(0, max);
		}

		private T GetRandomQuoteFromIndexList(List<int> indices, out int index)
		{
			int length = indices.Count;

			if (_removed.Count > 0)
			{
				int maxMismatch = GetMaxMismatchNumber();

				for (int i = 0; i < maxMismatch; i++)
				{
					T? q = GetRandomQuote(out int randomIndex);

					if (q is not null)
					{
						index = randomIndex;
						return q;
					}
				}

				DefragmentInternal();
			}

			return GetRandomQuote(out index)!;

			T? GetRandomQuote(out int index)
			{
				int randomIndex = GetRandomIndex(length);
				index = indices[randomIndex];
				return _lookup[index];
			}
		}

		private T GetRandomQuoteInternal(out int index)
		{
			int length = _lookup.Count;

			if (_removed.Count > 0)
			{
				int maxMismatch = GetMaxMismatchNumber();

				for (int i = 0; i < maxMismatch; i++)
				{
					T? q = GetRandomQuote(out int randomIndex);

					if (q is not null)
					{
						index = randomIndex;
						return q;
					}
				}

				DefragmentInternal();
			}

			return GetRandomQuote(out index)!;

			T? GetRandomQuote(out int index)
			{
				int randomIndex = GetRandomIndex(length);
				index = randomIndex;
				return _lookup[randomIndex]!;
			}
		}

		private void InitializeTagCache()
		{
			_tagCache = new();
			int length = _lookup.Count;

			for (int i = 0; i < length; i++)
			{
				T? quote = _lookup[i];

				if (quote is null)
				{
					continue;
				}

				foreach (string tag in quote.Tags)
				{
					CacheTag(tag, i);
				}
			}
		}

		private void RemoveIndex(int index)
		{
			_lookup[index] = null;
			_removed.Add(index);
			_isSorted = false;
		}

		private void RemoveQuoteAtIndex(T quote, int index)
		{
			_map.Remove(quote.GetId());
			RemoveIndex(index);
		}

		private void RemoveQuoteFromTagCache(T quote, int index)
		{
			foreach (string tag in quote.Tags)
			{
				if (_tagCache!.TryGetValue(tag, out TagCacheEntry? entry))
				{
					entry.Remove(index);
				}
			}
		}

		private bool ReplaceQuote(T quote, int id, out int index)
		{
			index = _map[id];
			T old = _lookup[index]!;

			if (quote is IEquatable<T> eq)
			{
				if (eq.Equals(old))
				{
					return false;
				}
			}
			else if (ReferenceEquals(old, quote))
			{
				return false;
			}

			if (_tagCache is not null)
			{
				foreach (string tag in old.Tags)
				{
					if (Array.IndexOf(old.Tags, tag) > -1 || !_tagCache.TryGetValue(tag, out TagCacheEntry? oldEntry))
					{
						continue;
					}

					oldEntry.Remove(index);
					CacheTag(tag, index);
				}
			}

			_lookup[index] = quote;

			return true;
		}

		private void SortRemoved()
		{
			if (!_isSorted)
			{
				_removed.Sort();
				_isSorted = true;
			}
		}

		private bool TryCache(T quote)
		{
			int id = quote.GetId();

			lock (_lockObject)
			{
				int index = _lookup.Count;

				if (_map.TryAdd(id, index))
				{
					_lookup.Add(quote);
					TryCacheTags(quote, index);

					return true;
				}
			}

			return false;
		}

		private bool TryCacheOrReplace(T quote)
		{
			int id = quote.GetId();

			lock (_lockObject)
			{
				int index = _lookup.Count;

				if (!_map.TryAdd(id, index))
				{
					return ReplaceQuote(quote, id, out index);
				}

				_lookup.Add(quote);
				TryCacheTags(quote, index);
			}

			return true;
		}

		private void TryCacheTags(T quote, int index)
		{
			if (_tagCache is not null)
			{
				foreach (string tag in quote.Tags)
				{
					if (string.IsNullOrWhiteSpace(tag))
					{
						continue;
					}

					CacheTag(tag, index);
				}
			}
		}

		private void TryRemoveQuoteFromTagCache(T quote, int index)
		{
			if (_tagCache is not null && _tagCache.Count > 0)
			{
				RemoveQuoteFromTagCache(quote, index);
			}
		}
	}
}
