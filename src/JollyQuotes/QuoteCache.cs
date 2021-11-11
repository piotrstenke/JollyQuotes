using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JollyQuotes
{
	/// <summary>
	/// Represents a list-based cache of quotes.
	/// </summary>
	/// <typeparam name="T">Type of quotes this class can store.</typeparam>
	public class QuoteCache<T> : IQuoteCache<T> where T : IQuote
	{
		private sealed class IndexComparer : IComparer<int>
		{
			public int Compare(int x, int y)
			{
				return y.CompareTo(x);
			}
		}

		private sealed class TagCacheEntry : IEnumerable<int>
		{
			private readonly List<int> _lookup;
			private readonly Dictionary<int, int> _map;

			public int Count => _map.Count;

			public bool IsEmpty => Count == 0;

			public TagCacheEntry()
			{
				_lookup = new();
				_map = new();
			}

			public void Add(int index)
			{
				if (_map.TryAdd(index, _lookup.Count))
				{
					_lookup.Add(index);
				}
			}

			public void Clear()
			{
				_map.Clear();
				_lookup.Clear();
			}

			public List<int>.Enumerator GetEnumerator()
			{
				return _lookup.GetEnumerator();
			}

			public int GetLookupIndex(int at)
			{
				return _lookup[at];
			}

			public bool Remove(int lookupIndex)
			{
				if (_map.Remove(lookupIndex, out int value))
				{
					_lookup.RemoveAt(value);
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

		private readonly IndexComparer _comparer;
		private readonly object _lockObject = new();
		private readonly List<T> _lookup;

		// key - IQuote.GetId(), value - index in _lookup
		private readonly Dictionary<int, int> _map;

		private readonly List<int> _removed;

		// key - tag, value - list of indices of quotes with that tag
		private readonly Dictionary<string, TagCacheEntry> _tagCache;

		/// <inheritdoc/>
		public bool IsEmpty => NumCached == 0;

		/// <summary>
		/// Number of cached values.
		/// </summary>
		public int NumCached
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
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class.
		/// </summary>
		public QuoteCache()
		{
			_map = new();
			_tagCache = new();
			_lookup = new();
			_removed = new();
			_comparer = new();
		}

		/// <summary>
		/// Adds the specified <paramref name="quote"/> to the cache.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to cache.</param>
		/// <param name="replace">Determines whether to replace an <see cref="IQuote"/> with the same id.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quote"/> is <see langword="null"/>.</exception>
		public void CacheQuote(T quote, bool replace = false)
		{
			if (quote is null)
			{
				throw Error.Null(nameof(quote));
			}

			int id = quote.GetId();

			lock (_lockObject)
			{
				ScrapRemoved();

				int index = _lookup.Count;

				if (!_map.TryAdd(id, index))
				{
					if (replace)
					{
						index = _map[id];

						lock (_lookup)
						{
							_lookup[index] = quote;
						}

						// Replace old tags
					}

					return;
				}

				_lookup.Add(quote);
				CacheTags(quote.Tags, index);
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			lock (_lockObject)
			{
				_map.Clear();
				_lookup.Clear();
				_removed.Clear();
				_tagCache.Clear();
			}
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached()
		{
			lock (_lockObject)
			{
				ScrapRemoved();

				return _lookup.ToArray();
			}
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			lock (_lockObject)
			{
				if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry) || entry.IsEmpty)
				{
					return Array.Empty<T>();
				}

				ScrapRemoved();

				if (entry.IsEmpty)
				{
					return Array.Empty<T>();
				}

				List<T> quotes = new(entry.Count);

				foreach (int index in entry)
				{
					quotes.Add(_lookup[index]);
				}

				return quotes;
			}
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return Array.Empty<T>();
			}

			lock (_lockObject)
			{
				if (_tagCache.Count == 0)
				{
					return Array.Empty<T>();
				}

				ScrapRemoved();

				List<T> quotes = new(_lookup.Count);

				HashSet<int> included = new();

				foreach (string tag in tags)
				{
					if (string.IsNullOrWhiteSpace(tag) || !_tagCache.TryGetValue(tag, out TagCacheEntry? entry) || entry.IsEmpty)
					{
						continue;
					}

					foreach (int index in entry)
					{
						if (included.Add(index))
						{
							quotes.Add(_lookup[index]);
						}
					}
				}

				return quotes;
			}
		}

		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			lock (_lockObject)
			{
				ScrapRemoved();

				return _lookup.GetEnumerator();
			}
		}

		/// <inheritdoc/>
		public T GetRandomQuote(bool remove = false)
		{
			lock (_lockObject)
			{
				ScrapRemoved();

				if (IsEmpty)
				{
					throw new InvalidOperationException("Cannot return a quote from empty cache");
				}

				int index = Internals.RandomNumber(0, NumCached);

				T quote = _lookup[index];

				if (remove && _map.Remove(quote.GetId()))
				{
					_removed.Add(index);
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
			return IsCached(id);
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
			return RemoveQuote(id);
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
				if (!_map.Remove(id, out int index))
				{
					quote = _lookup[index];
					_removed.Add(index);

					return true;
				}
			}

			quote = default;
			return false;
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
				if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry) || entry.IsEmpty)
				{
					return false;
				}

				ScrapRemoved();

				if (entry.IsEmpty)
				{
					return false;
				}

				foreach (int key in entry)
				{
					_map.Remove(key, out int index);
					_removed.Add(index);
				}

				entry.Clear();
			}

			return true;
		}

		/// <inheritdoc/>
		public bool TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote, bool remove = false)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Error.NullOrEmpty(nameof(tag));
			}

			lock (_lockObject)
			{
				if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry) || entry.IsEmpty)
				{
					quote = default;
					return false;
				}

				ScrapRemoved();

				if (entry.IsEmpty)
				{
					quote = default;
					return false;
				}

				int randomIndex = Internals.RandomNumber(0, entry.Count);
				int lookupIndex = entry.GetLookupIndex(randomIndex);

				quote = _lookup[lookupIndex];

				if (remove)
				{
					_map.Remove(quote.GetId());
					_removed.Add(lookupIndex);
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

		private void CacheTags(string[] tags, int index)
		{
			if (tags is null || tags.Length == 0)
			{
				return;
			}

			lock (_lockObject)
			{
				foreach (string tag in tags)
				{
					if (string.IsNullOrWhiteSpace(tag))
					{
						continue;
					}

					TagCacheEntry entry = GetTagEntry(tag);

					entry.Add(index);
				}
			}
		}

		private TagCacheEntry GetTagEntry(string tag)
		{
			if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry))
			{
				entry = new();
				_tagCache.TryAdd(tag, entry);
			}

			return entry;
		}

		private void ScrapRemoved()
		{
			if (_removed.Count == 0)
			{
				return;
			}

			// from last to first
			_removed.Sort(_comparer);
			List<string> tags = new(_removed.Count * 2);

			foreach (int index in _removed)
			{
				T quote = _lookup[index];
				_lookup.RemoveAt(index);

				tags.AddRange(quote.Tags);
			}

			foreach (string tag in tags)
			{
				if (!_tagCache.TryGetValue(tag, out TagCacheEntry? entry) || entry.IsEmpty)
				{
					continue;
				}

				foreach (int index in _removed)
				{
					entry.Remove(index);
				}
			}

			_removed.Clear();
		}
	}
}
