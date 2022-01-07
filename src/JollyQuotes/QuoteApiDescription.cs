using System;

namespace JollyQuotes
{
	/// <summary>
	/// Describes properties of a quote API.
	/// </summary>
	public sealed record QuoteApiDescription
	{
		private readonly JollyQuotesApi _api;

		/// <summary>
		/// Name of the API.
		/// </summary>
		public string Name { get; init; }

		/// <summary>
		/// Determines whether the API is a custom, non <c>JollyQuotes</c> API.
		/// </summary>
		public bool IsCustom => _api == JollyQuotesApi.None;

		/// <summary>
		/// <see cref="Type"/> of main <see cref="IRandomNumberGenerator"/> implementation in the target API.
		/// </summary>
		public Type GeneratorType { get; init; }

		/// <summary>
		/// <see cref="Type"/> of main <see cref="IQuoteService"/> implementation in the target API.
		/// </summary>
		public Type ServiceType { get; init; }

		/// <summary>
		/// <see cref="Type"/> of main <see cref="IQuote"/> implementation in the target API.
		/// </summary>
		public Type QuoteType { get; init; }

		/// <summary>
		/// <see cref="Type"/> of a static class containing required resources such as links, paths or keys in the target API.
		/// </summary>
		public Type ResourcesType { get; init; }

		internal QuoteApiDescription(
			string name,
			Type generatorType,
			Type serviceType,
			Type quoteType,
			Type resourcesType,
			JollyQuotesApi api
		)
		{
			if(string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if(generatorType is null)
			{
				throw Error.Null(nameof(generatorType));
			}

			if (serviceType is null)
			{
				throw Error.Null(nameof(serviceType));
			}

			if (quoteType is null)
			{
				throw Error.Null(nameof(quoteType));
			}

			if (resourcesType is null)
			{
				throw Error.Null(nameof(resourcesType));
			}

			Name = name;
			GeneratorType = generatorType;
			ServiceType = serviceType;
			QuoteType = quoteType;
			ResourcesType = resourcesType;
			_api = api;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteApiDescription"/> class with its name and required class <see cref="Type"/>s specified.
		/// </summary>
		/// <param name="name">Name of the API.</param>
		/// <param name="generatorType"><see cref="Type"/> of main <see cref="IRandomNumberGenerator"/> implementation in the target API.</param>
		/// <param name="serviceType"><see cref="Type"/> of main <see cref="IQuoteService"/> implementation in the target API.</param>
		/// <param name="quoteType"><see cref="Type"/> of main <see cref="IQuote"/> implementation in the target API.</param>
		/// <param name="resourcesType"><see cref="Type"/> of a static class containing required resources such as links, paths or keys in the target API.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="generatorType"/> is <see langword="null"/>. -or-
		/// <paramref name="serviceType"/> is <see langword="null"/>. -or-
		/// <paramref name="quoteType"/> is <see langword="null"/>. -or-
		/// <paramref name="resourcesType"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
		public QuoteApiDescription(
			string name,
			Type generatorType,
			Type serviceType,
			Type quoteType,
			Type resourcesType
		) : this(name, generatorType, serviceType, quoteType, resourcesType, JollyQuotesApi.None)
		{
		}

		/// <summary>
		/// Attempts to return a value of the <see cref="JollyQuotesApi"/> enum associated with the target API.
		/// </summary>
		/// <param name="api">Value of the <see cref="JollyQuotesApi"/> enum associated with the target API</param>
		/// <remarks><see cref="IsCustom"/> will always return the same <see cref="bool"/> value as this method.</remarks>
		/// <returns><see langword="true"/> if the API is a <c>JollyQuotes</c> API, false otherwise.</returns>
		public bool TryGetEnumValue(out JollyQuotesApi api)
		{
			if(!IsCustom)
			{
				api = default;
				return false;
			}

			api = _api;
			return true;
		}
	}
}
