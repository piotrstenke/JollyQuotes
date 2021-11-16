using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JollyQuotes
{
	/// <summary>
	/// A cache of quotes that can be temporary blocked from receiving new values.
	/// </summary>
	/// <remarks>Methods of this class are thread-safe only if the underlaying <see cref="IQuoteCache{T}"/> is also thread-safe.</remarks>
	/// <typeparam name="T">Type of quotes this class can store.</typeparam>
	public class BlockableQuoteCache<T> : IQuoteCache<T> where T : class, IQuote
	{
		private readonly IQuoteCache<T> _cache;
		private bool _preserveState = true;

		/// <summary>
		/// Determines whether the cache is blocked from modification.
		/// </summary>
		public bool IsBlocked { get; private set; }

		/// <inheritdoc/>
		public bool IsEmpty => _cache.IsEmpty;

		/// <summary>
		/// Determines whether state of the cache should be preserved when <see cref="IsBlocked"/> is <see langword="false"/> and restored when <see cref="IsBlocked"/> is set back to <see langword="true"/>.
		/// </summary>
		/// <remarks>Defaults to <see langword="true"/>.</remarks>
		public bool PreserveState
		{
			get => _preserveState;
			set
			{
				if (!value && IsBlocked)
				{
					ForceClear();
				}

				_preserveState = value;
			}
		}

		/// <summary>
		/// Determines whether to throw an <see cref="InvalidOperationException"/> when a method
		/// that modifies the state of the cache is called while <see cref="IsBlocked"/> is <see langword="true"/>.
		/// </summary>
		public bool ThrowIfBlocked { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockableQuoteCache{T}"/> class with the underlaying <paramref name="cache"/> specified.
		/// </summary>
		/// <param name="cache"><see cref="IQuoteCache{T}"/> to use when caching <see cref="IQuote"/>s.</param>
		/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
		public BlockableQuoteCache(IQuoteCache<T> cache)
		{
			if (cache is null)
			{
				throw Error.Null(nameof(cache));
			}

			_cache = cache;
		}

		/// <summary>
		/// Prohibits the cache from being modified until <see cref="Unblock"/> is called.
		/// </summary>
		public void Block()
		{
			IsBlocked = true;

			if (_preserveState)
			{
				ForceClear();
			}
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Blocked cache cannot be modified.</exception>
		public void CacheQuote(T quote)
		{
			if (CanBeModified())
			{
				_cache.CacheQuote(quote);
			}
		}

		/// <summary>
		/// Removes all cached values if <see cref="IsBlocked"/> is <see langword="true"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException">Blocked cache cannot be modified.</exception>
		public void Clear()
		{
			if (CanBeModified())
			{
				ForceClear();
			}
		}

		/// <summary>
		/// Removes all cached values, regardless of whether <see cref="IsBlocked"/> is <see langword="true"/> or not.
		/// </summary>
		public void ForceClear()
		{
			_cache.Clear();
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached()
		{
			return _cache.GetCached();
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(string tag)
		{
			return _cache.GetCached(tag);
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetCached(params string[]? tags)
		{
			return _cache.GetCached(tags);
		}

		/// <summary>
		/// Enumerates through all <see cref="IQuote"/>s contained within the cache.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return _cache.GetEnumerator();
		}

		/// <inheritdoc/>
		public T GetRandomQuote()
		{
			return _cache.GetRandomQuote();
		}

		/// <inheritdoc/>
		public bool IsCached(T quote)
		{
			return _cache.IsCached(quote);
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Blocked cache cannot be modified.</exception>
		public bool RemoveQuote(T quote)
		{
			if (CanBeModified())
			{
				return _cache.RemoveQuote(quote);
			}

			return false;
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException">Blocked cache cannot be modified.</exception>
		public bool RemoveQuotes(string tag)
		{
			if (CanBeModified())
			{
				return _cache.RemoveQuotes(tag);
			}

			return false;
		}

		/// <inheritdoc/>
		public bool TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote)
		{
			return _cache.TryGetRandomQuote(tag, out quote);
		}

		/// <summary>
		/// Allows the cache to be modified until <see cref="Block"/> is called.
		/// </summary>
		public void Unblock()
		{
			IsBlocked = false;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private bool CanBeModified()
		{
			if (!IsBlocked)
			{
				return true;
			}

			if (ThrowIfBlocked)
			{
				throw new InvalidOperationException("Blocked cache cannot be modified");
			}

			return false;
		}
	}
}
