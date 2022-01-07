using System;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s.
	/// </summary>
	public interface IEnumerableQuoteGenerator : IQuoteGenerator, IEnumerable<IQuote>
	{
		/// <summary>
		/// Returns a collection of all possible quotes.
		/// </summary>
		IEnumerable<IQuote> GetAllQuotes();

		/// <summary>
		/// Returns a collection of all possible quotes associated with the specified <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to get all <see cref="IQuote"/>s associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		IEnumerable<IQuote> GetAllQuotes(string tag);

		/// <summary>
		/// Returns a collection of all possible quotes associated with any of the specified <paramref name="tags"/>.
		/// </summary>
		/// <param name="tags">Tags to get all <see cref="IQuote"/> associated with.</param>
		IEnumerable<IQuote> GetAllQuotes(params string[]? tags);
	}
}
