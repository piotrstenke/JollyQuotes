using System;
using System.Collections;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IEnumerator{T}"/> that uses a <see cref="BlockableQuoteCache{T}"/> to cache the iterated quotes.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this enumerator can access.</typeparam>
	public struct CachingEnumerator<T> : IEnumerator<T> where T : IQuote
	{
		/// <summary>
		/// Storage of cached <see cref="IQuote"/>s.
		/// </summary>
		public BlockableQuoteCache<T> Cache { get; }

		/// <summary>
		/// The underlaying <see cref="IEnumerator{T}"/>.
		/// </summary>
		public IEnumerator<T> Enumerator { get; }

		/// <summary>
		/// Current <see cref="IQuote"/>.
		/// </summary>
		public T Current => Enumerator.Current;

		object IEnumerator.Current => Current;

		/// <summary>
		/// Initializes a new instance of the <see cref="CachingEnumerator{T}"/> struct with an underlaying <paramref name="enumerator"/> and target <paramref name="cache"/> specified.
		/// </summary>
		/// <param name="enumerator">The underlaying <see cref="IEnumerator{T}"/>.</param>
		/// <param name="cache">Storage of cached <see cref="IQuote"/>s.</param>
		/// <exception cref="ArgumentNullException"><paramref name="enumerator"/> is <see langword="null"/>. -or- <paramref name="cache"/> is <see langword="null"/>.</exception>
		public CachingEnumerator(IEnumerator<T> enumerator, BlockableQuoteCache<T> cache)
		{
			if (enumerator is null)
			{
				throw Error.Null(nameof(enumerator));
			}

			if (cache is null)
			{
				throw Error.Null(nameof(cache));
			}

			Enumerator = enumerator;
			Cache = cache;
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Enumerator.Dispose();
		}

		/// <inheritdoc/>
		public bool MoveNext()
		{
			bool moveNext = Enumerator.MoveNext();

			if (moveNext && !Cache.IsBlocked)
			{
				Cache.CacheQuote(Enumerator.Current);
			}

			return moveNext;
		}

		/// <inheritdoc/>
		public void Reset()
		{
			Enumerator.Reset();
		}
	}
}
