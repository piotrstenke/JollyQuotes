using System;

namespace JollyQuotes
{
	/// <summary>
	/// Provides mechanism for generating random quotes.
	/// </summary>
	public interface IRandomQuoteGenerator
	{
		/// <summary>
		/// Source of the quotes, e.g. a link, file name or raw text.
		/// </summary>
		string Source { get; }

		/// <summary>
		/// Generates a random quote.
		/// </summary>
		IQuote GetRandomQuote();

		/// <summary>
		/// Generates a random quote associated with the specified tag.
		/// </summary>
		/// <param name="tag">Tag to generate a quote associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		IQuote? GetRandomQuote(string tag);

		/// <summary>
		/// Generates a random quote associated with any of the specified <paramref name="tags"/>.
		/// </summary>
		/// <param name="tags">Tags to generate a quote associated with.</param>
		IQuote? GetRandomQuote(params string[]? tags);
	}
}
