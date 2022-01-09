using System;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// Provides methods for creation of quote-related objects.
	/// </summary>
	public interface IQuoteApiHandler
	{
		/// <summary>
		/// Creates a new <see cref="QuoteApiDescription"/> for API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to create the <see cref="QuoteApiDescription"/> for.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		QuoteApiDescription CreateDescription(string apiName);

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> based on the specified API <paramref name="description"/>.
		/// </summary>
		/// <param name="description">Provides information about the API the <see cref="IQuoteGenerator"/> is to be created for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="description"/> is <see langword="null"/>. -or- <paramref name="resolver"/> is <see langword="null"/>.</exception>
		IQuoteGenerator CreateGenerator(QuoteApiDescription description, IResourceResolver resolver);

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		IQuoteGenerator CreateGenerator(string apiName, IResourceResolver resolver);

		/// <summary>
		/// Returns a collection of names of all supported APIs.
		/// </summary>
		IEnumerable<string> GetApis();

		/// <summary>
		/// Determines whether the handler contains information about API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to check for.</param>
		/// <exception cref="ArgumentException"><paramref name="apiName"/> is <see langword="null"/> or empty.</exception>
		bool HasApi(string apiName);
	}
}
