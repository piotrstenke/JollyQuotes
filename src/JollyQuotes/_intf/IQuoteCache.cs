using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JollyQuotes
{
	/// <summary>
	/// Represents a cache of quotes.
	/// </summary>
	/// <typeparam name="T">Type of quotes this class can store.</typeparam>
	public interface IQuoteCache<T> : IEnumerable<T> where T : IQuote
	{
		/// <summary>
		/// Determines whether the cache is empty.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Adds the specified <paramref name="quote"/> to the cache.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to cache.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quote"/> is <see langword="null"/>.</exception>
		void CacheQuote(T quote);

		/// <summary>
		/// Removes all cached values.
		/// </summary>
		void Clear();

		/// <summary>
		/// Returns a collection of all cached <see cref="IQuote"/>s.
		/// </summary>
		IEnumerable<T> GetCached();

		/// <summary>
		/// Returns a collection of all cached <see cref="IQuote"/>s associated with the specified <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to get all the cached <see cref="IQuote"/>s associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		IEnumerable<T> GetCached(string tag);

		/// <summary>
		/// Returns a collection of all cached <see cref="IQuote"/>s associated with any of the specified <paramref name="tags"/>.
		/// </summary>
		/// <param name="tags">Tags to get all the cached <see cref="IQuote"/>s associated with.</param>
		IEnumerable<T> GetCached(params string[]? tags);

		/// <summary>
		/// Returns a random <see cref="IQuote"/> from the cache.
		/// </summary>
		/// <param name="remove">Determines whether to remove the returned <see cref="IQuote"/> from the cache.</param>
		/// <exception cref="InvalidOperationException">Cannot return a quote from empty cache.</exception>
		T GetRandomQuote(bool remove = false);

		/// <summary>
		/// Determines whether the specified <paramref name="quote"/> is cached.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to check if is cached.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quote"/> is <see langword="null"/>.</exception>
		bool IsCached(T quote);

		/// <summary>
		/// Removes the specified <paramref name="quote"/> from the cache.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to remove from the cache.</param>
		/// <returns><see langword="true"/> if the <paramref name="quote"/> was successfully removed from the cache, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="quote"/> is <see langword="null"/>.</exception>
		bool RemoveQuote(T quote);

		/// <summary>
		/// Removes all quotes with the specified <paramref name="tag"/> from the cache.
		/// </summary>
		/// <param name="tag">Tag remove all quotes associated with.</param>
		/// <returns><see langword="true"/> if any quote associated with the specified <paramref name="tag"/> was successfully removed, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		bool RemoveQuotes(string tag);

		/// <summary>
		/// Attempts to return a random <see cref="IQuote"/> from the cache associated with the specified <paramref name="tag"/> or
		/// <see langword="null"/> if there is no such <see cref="IQuote"/> in the cache.
		/// </summary>
		/// <param name="tag">Tag to generate a quote associated with.</param>
		/// <param name="quote">Returned <see cref="IQuote"/>.</param>
		/// <param name="remove">Determines whether to remove the returned <see cref="IQuote"/> from the cache.</param>
		/// <returns><see langword="true"/> if a <see cref="IQuote"/> associated with the specified <paramref name="tag"/> was found, <see langword="false"/> otherwise.</returns>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		bool TryGetRandomQuote(string tag, [NotNullWhen(true)] out T? quote, bool remove = false);
	}
}
