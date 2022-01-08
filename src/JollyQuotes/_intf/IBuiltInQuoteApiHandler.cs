using System;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IQuoteApiHandler"/> that handles all built-in <c>JollyQuotes</c> APIs.
	/// </summary>
	public interface IBuiltInQuoteApiHandler : IQuoteApiHandler
	{
		/// <summary>
		/// Creates a new <see cref="QuoteApiDescription"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="QuoteApiDescription"/> for.</param>
		/// <exception cref="ArgumentException"><paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API.</exception>
		QuoteApiDescription CreateDescription(JollyQuotesApi api);

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API.</exception>
		IQuoteGenerator CreateGenerator(JollyQuotesApi api, IResourceResolver resolver);
	}
}
