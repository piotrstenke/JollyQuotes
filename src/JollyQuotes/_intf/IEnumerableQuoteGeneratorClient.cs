using System;
using System.Collections.Generic;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s through an external web API accessed by a <see cref="HttpClient"/>..
	/// </summary>
	public interface IEnumerableQuoteGeneratorClient : IQuoteGeneratorClient, IEnumerableQuoteGenerator, IAsyncEnumerable<IQuote>
	{
		/// <summary>
		/// Returns an asynchronous collection of all possible quotes.
		/// </summary>
		IAsyncEnumerable<IQuote> GetAllQuotesAsync();

		/// <summary>
		/// Returns an asynchronous collection of all possible quotes associated with the specified <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to get all <see cref="IQuote"/>s associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		IAsyncEnumerable<IQuote> GetAllQuotesAsync(string tag);

		/// <summary>
		/// Returns an asynchronous collection of all possible quotes associated with any of the specified <paramref name="tags"/>.
		/// </summary>
		/// <param name="tags">Tags to get all <see cref="IQuote"/> associated with.</param>
		IAsyncEnumerable<IQuote> GetAllQuotesAsync(params string[]? tags);
	}
}
