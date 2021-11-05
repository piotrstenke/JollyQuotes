using System;
using System.Collections;
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
		private readonly Dictionary<int, int> _cache;
		private readonly List<T> _lookup;
		private readonly Random _random;
		private readonly HashSet<int> _removed;
		private readonly Dictionary<string, List<int>> _tagCache;
		private int _recentlyAdded;

		/// <inheritdoc/>
		public bool IsEmpty => NumCached == 0;

		/// <summary>
		/// Number of cached values.
		/// </summary>
		public int NumCached => _lookup.Count;

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteCache{T}"/> class.
		/// </summary>
		public QuoteCache()
		{
			_lookup = new List<T>();
			_cache = new Dictionary<int, int>();
			_removed = new HashSet<int>();
			_random = new Random();
			_tagCache = new Dictionary<string, List<int>>();
		}

		/// <inheritdoc/>
		public void CacheQuote(T quote)
		{
			if (quote is null)
			{
				throw Internals.Null(nameof(quote));
			}

			ScrapRemovedValues();

			int id = quote.GetId();
			int index = _lookup.Count;

			if (!_cache.TryAdd(id, index))
			{
				return;
			}

			_lookup.Add(quote);
			++_recentlyAdded;
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_lookup.Clear();
			_cache.Clear();
			_removed.Clear();
			_tagCache.Clear();
			_recentlyAdded = 0;
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached()
		{
			ScrapRemovedValues();

			return _lookup.ToArray();
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			if (!InitializeCacheForTag(tag, out List<int>? quoteIndices))
			{
				return Array.Empty<T>();
			}

			T[] quotes = new T[quoteIndices.Count];

			for (int i = 0; i < quoteIndices.Count; i++)
			{
				quotes[i] = _lookup[quoteIndices[i]];
			}

			return quotes;
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return Array.Empty<T>();
			}

			return Yield();

			IEnumerable<T> Yield()
			{
				HashSet<int> quotes = new();

				foreach (string tag in tags)
				{
					if (string.IsNullOrWhiteSpace(tag) || !InitializeCacheForTag(tag, out List<int>? quoteIndices))
					{
						continue;
					}

					foreach (int index in quoteIndices)
					{
						if (quotes.Add(index))
						{
							yield return _lookup[index];
						}
					}
				}
			}
		}

		/// <summary>
		/// Enumerates through all <see cref="IQuote"/>s contained within the cache.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			ScrapRemovedValues();

			return _lookup.GetEnumerator();
		}

		/// <inheritdoc/>
		public T GetRandomQuote(bool remove = false)
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException("Cannot return a quote from empty cache");
			}

			ScrapRemovedValues();

			int index = _random.Next(0, _lookup.Count);
			T quote = _lookup[index];

			if (remove)
			{
				_cache.Remove(quote.GetId());
				_removed.Add(index);
			}

			return quote;
		}

		/// <inheritdoc/>
		public bool IsCached(T quote)
		{
			if (quote is null)
			{
				throw Internals.Null(nameof(quote));
			}

			return _cache.ContainsKey(quote.GetId());
		}

		/// <inheritdoc/>
		public bool RemoveQuote(T quote)
		{
			if (quote is null)
			{
				throw Internals.Null(nameof(quote));
			}

			return RemoveQuoteInternal(quote);
		}

		/// <inheritdoc/>
		public bool RemoveQuotes(string tag)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			if (!InitializeCacheForTag(tag, out List<int>? quoteIndices))
			{
				return false;
			}

			foreach (int index in quoteIndices)
			{
				RemoveQuoteInternal(_lookup[index]);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote, bool remove = false)
		{
			if (string.IsNullOrEmpty(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			if (!InitializeCacheForTag(tag, out List<int>? quoteIndices))
			{
				quote = default;
				return false;
			}

			int randomIndex = _random.Next(0, quoteIndices.Count);
			int quoteIndex = quoteIndices[randomIndex];

			quote = _lookup[quoteIndex];

			if (remove)
			{
				_cache.Remove(quote.GetId());
				_removed.Add(quoteIndex);
			}

			return true;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void AddIfHasTag(List<int> list, int index, string tag)
		{
			T quote = _lookup[index];

			if (quote.HasTag(tag))
			{
				list.Add(index);
			}
		}

		private int GetFirstScrapingIndex()
		{
			int index = 0;

			// Indices in _cache don't need to be updated until the first removed index is detected.
			while (!_removed.Contains(index))
			{
				++index;
			}

			return index;
		}

		private bool InitializeCacheForTag(string tag, [NotNullWhen(true)] out List<int>? quoteIndices)
		{
			int startRemoveRange = ScrapRemovedValues();
			return InitializeCacheForTag(tag, startRemoveRange, out quoteIndices);
		}

		private bool InitializeCacheForTag(string tag, int startRemoveRange, [NotNullWhen(true)] out List<int>? quoteIndices)
		{
			if (startRemoveRange > -1 && UpdateTagCache(tag, startRemoveRange, out quoteIndices))
			{
				InitializeTagsForRecentlyAdded(quoteIndices, tag);

				if (quoteIndices.Count > 0)
				{
					return true;
				}
			}
			else if (_lookup.Count > 0)
			{
				quoteIndices = InitializeTagList(tag);

				if (quoteIndices.Count > 0)
				{
					_tagCache[tag] = quoteIndices;
					return true;
				}
			}

			quoteIndices = null;
			return false;
		}

		private List<int> InitializeTagList(string tag)
		{
			List<int> list = new(_lookup.Count);

			for (int i = 0; i < _lookup.Count; i++)
			{
				AddIfHasTag(list, i, tag);
			}

			list.TrimExcess();
			return list;
		}

		private void InitializeTagsForRecentlyAdded(List<int> quoteIndices, string tag)
		{
			if (_recentlyAdded == 0)
			{
				return;
			}

			for (int i = _lookup.Count - _recentlyAdded; i < _lookup.Count; i++)
			{
				AddIfHasTag(quoteIndices, i, tag);
			}

			_recentlyAdded = 0;
		}

		private bool RemoveQuoteInternal(T quote)
		{
			bool removed = _cache.Remove(quote.GetId(), out int index);

			if (removed)
			{
				_removed.Add(index);

				if (index >= _lookup.Count - _recentlyAdded)
				{
					--_recentlyAdded;
				}
			}

			return removed;
		}

		private int ScrapRemovedValues()
		{
			if (_removed.Count == 0)
			{
				return -1;
			}

			int startIndex = GetFirstScrapingIndex();
			int startRemoveRange = ScrapRemovedValuesWithoutClearing(startIndex);

			_lookup.RemoveRange(startRemoveRange, _lookup.Count - startRemoveRange);
			_removed.Clear();

			return startRemoveRange;
		}

		private int ScrapRemovedValuesWithoutClearing(int startIndex)
		{
			int newIndex = startIndex;

			for (int i = startIndex + 1; i < _lookup.Count; i++)
			{
				if (!_removed.Contains(i))
				{
					T quote = _lookup[i];
					_lookup[newIndex] = quote;
					_cache[quote.GetId()] = newIndex;

					++newIndex;
				}
			}

			return newIndex;
		}

		private bool UpdateTagCache(string tag, int startRemoveRange, [NotNullWhen(true)] out List<int>? quoteIndices)
		{
			if (_tagCache.TryGetValue(tag, out quoteIndices))
			{
				int index = quoteIndices.FindIndex(q => q >= startRemoveRange);

				if (index > -1)
				{
					quoteIndices.RemoveRange(index, quoteIndices.Count - index);
				}

				return true;
			}

			return false;
		}
	}
}
