using System;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// Provides methods for creation of quote-related objects for all built-in <c>JollyQuotes</c> APIs.
	/// </summary>
	public sealed class BuiltInQuoteApiHandler : IBuiltInQuoteApiHandler
	{
		/// <summary>
		/// <see cref="IPossibility"/> that will be passed to the constructor of each <see cref="IQuoteGenerator"/> created using the <c>CreateGenerator</c> method.
		/// </summary>
		public IPossibility? Possibility { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BuiltInQuoteApiHandler"/> class.
		/// </summary>
		public BuiltInQuoteApiHandler()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BuiltInQuoteApiHandler"/> class with a target <see cref="IPossibility"/> specified.
		/// </summary>
		/// <param name="possibility">Implementation of <see cref="IPossibility"/> that will be passed to the constructor of every <see cref="IQuoteGenerator"/> created using the <c>CreateGenerator</c> method.</param>
		public BuiltInQuoteApiHandler(IPossibility? possibility)
		{
			Possibility = possibility;
		}

		/// <inheritdoc/>
		public QuoteApiDescription CreateDescription(string apiName)
		{
			if (QuoteUtility.TryParseApi(apiName, out JollyQuotesApi api))
			{
				return CreateDescription(api);
			}

			throw QuoteUtility.Exc_UnknownApiNameOrNull(apiName);
		}

		/// <inheritdoc/>
		public QuoteApiDescription CreateDescription(JollyQuotesApi api)
		{
			return QuoteUtility.CreateDescription(api);
		}

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found. -or-
		/// The <c>Tronald Dump</c> API requires the <paramref name="resolver"/> to implement the <see cref="IStreamResolver"/> interface.
		/// </exception>
		public IQuoteGenerator CreateGenerator(string apiName, IResourceResolver resolver)
		{
			if (QuoteUtility.TryParseApi(apiName, out JollyQuotesApi api))
			{
				return CreateGenerator(api, resolver);
			}

			throw QuoteUtility.Exc_UnknownApiNameOrNull(apiName);
		}

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> based on the specified API <paramref name="description"/>.
		/// </summary>
		/// <param name="description">Provides information about the API the <see cref="IQuoteGenerator"/> is to be created for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="description"/> is <see langword="null"/>. -or- <paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">The <c>Tronald Dump</c> API requires the <paramref name="resolver"/> to implement the <see cref="IStreamResolver"/> interface.</exception>
		public IQuoteGenerator CreateGenerator(QuoteApiDescription description, IResourceResolver resolver)
		{
			if (description is null)
			{
				throw Error.Null(nameof(description));
			}

			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			if (!description.TryGetEnumValue(out JollyQuotesApi api))
			{
				throw Error.Arg("Only built-in JollyQuotes APIs are supported", nameof(description));
			}

			return CreateGenerator(api, resolver);
		}

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API. -or-
		/// The <c>Tronald Dump</c> API requires the <paramref name="resolver"/> to implement the <see cref="IStreamResolver"/> interface.
		/// </exception>
		public IQuoteGenerator CreateGenerator(JollyQuotesApi api, IResourceResolver resolver)
		{
			return QuoteUtility.CreateGenerator(api, resolver, Possibility);
		}

		IEnumerable<string> IQuoteApiHandler.GetApis()
		{
			return ApiNames.GetAll();
		}

		bool IQuoteApiHandler.HasApi(string apiName)
		{
			if (QuoteUtility.TryParseApi(apiName, out _))
			{
				return true;
			}

			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			return false;
		}
	}
}
