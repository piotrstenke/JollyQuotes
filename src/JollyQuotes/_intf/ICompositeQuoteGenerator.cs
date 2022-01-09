using System;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// Combines behavior of multiple <see cref="IQuoteGenerator"/>s.
	/// </summary>
	public interface ICompositeQuoteGenerator : IQuoteGenerator, IEnumerable<IQuoteGenerator>
	{
		/// <summary>
		/// <see cref="IQuoteApiHandler"/> that is used to access API-specific objects.
		/// </summary>
		IQuoteApiHandler ApiHandler { get; }

		/// <summary>
		/// Disables all APIs.
		/// </summary>
		void DisableAll();

		/// <summary>
		/// Disables an API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to disable.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		void DisableApi(string apiName);

		/// <summary>
		/// Enables all APIs.
		/// </summary>
		void EnableAll();

		/// <summary>
		/// Enables an API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to enable.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		void EnableApi(string apiName);

		/// <summary>
		/// Forces the generator to update its internal container of <see cref="IQuoteGenerator"/>s.
		/// </summary>
		void ForceUpdate();

		/// <summary>
		/// Returns a collection of <see cref="QuoteApiDescription"/>s representing all APIs known by the generator.
		/// </summary>
		IEnumerable<QuoteApiDescription> GetApis();

		/// <summary>
		/// Returns a <see cref="QuoteApiDescription"/> associated with API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to get the <see cref="QuoteApiDescription"/> associated with.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		QuoteApiDescription GetDescription(string apiName);

		/// <summary>
		/// Returns a collection of <see cref="QuoteApiDescription"/>s representing all currently disabled APIs.
		/// </summary>
		IEnumerable<QuoteApiDescription> GetDisabledApis();

		/// <summary>
		/// Returns a collection of <see cref="IQuoteGenerator"/>s associated with all currently disabled APIs.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IQuoteGenerator> GetDisabledGenerators();

		/// <summary>
		/// Returns a collection of <see cref="QuoteApiDescription"/>s representing all currently enabled APIs.
		/// </summary>
		IEnumerable<QuoteApiDescription> GetEnabledApis();

		/// <summary>
		/// Returns a collection of <see cref="IQuoteGenerator"/>s associated with all currently enabled APIs.
		/// </summary>
		IEnumerable<IQuoteGenerator> GetEnabledGenerators();

		/// <summary>
		/// Returns a <see cref="IQuoteGenerator"/> associated with API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName">Name of API to get the <see cref="IQuoteGenerator"/> associated with.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		IQuoteGenerator GetGenerator(string apiName);

		/// <summary>
		/// Returns a collection of <see cref="IQuoteGenerator"/>s associated with all APIs known by the generator.
		/// </summary>
		IEnumerable<IQuoteGenerator> GetGenerators();

		/// <summary>
		/// Determines whether API with the specified <paramref name="apiName"/> is enabled.
		/// </summary>
		/// <param name="apiName">Name of API to check whether is enabled.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		bool IsEnabled(string apiName);

		/// <summary>
		/// Determines whether API with the specified <paramref name="apiName"/> is registered in the current generator.
		/// </summary>
		/// <param name="apiName">Name of API to check whether is registered.</param>
		/// <exception cref="ArgumentException"><paramref name="apiName"/> is <see langword="null"/> or empty.</exception>
		bool IsRegistered(string apiName);

		/// <summary>
		/// Enables an API with the specified <paramref name="apiName"/> and disables all others.
		/// </summary>
		/// <param name="apiName">Name of API to enable.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		void SwitchTo(string apiName);
	}
}
