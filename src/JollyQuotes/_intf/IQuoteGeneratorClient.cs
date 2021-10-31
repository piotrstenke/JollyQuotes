using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <summary>
	/// Generates quotes using an external web API accessed by a <see cref="HttpClient"/>.
	/// </summary>
	public interface IQuoteGeneratorClient : IRandomQuoteGenerator
	{
		/// <summary>
		/// Underlaying client that is used to access required resources.
		/// </summary>
		HttpClient BaseClient { get; }

		/// <summary>
		/// Asynchronously generates a random quote.
		/// </summary>
		Task<IQuote> GetRandomQuoteAsync();

		/// <summary>
		/// Asynchronously generates a random quote associated with any of the specified <paramref name="tags"/>.
		/// </summary>
		/// <param name="tags">Tags to generate a quote associated with.</param>
		Task<IQuote?> GetRandomQuoteAsync(params string[]? tags);

		/// <summary>
		/// Asynchronously generates a random quote associated with the specified tag.
		/// </summary>
		/// <param name="tag">Tag to generate a quote associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		Task<IQuote?> GetRandomQuoteAsync(string tag);
	}
}
