using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// Combines behavior of all existing built-in <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.
	/// </summary>
	public class JollyQuotesGenerator : QuoteClient<IQuote>, ICompositeQuoteGenerator
	{
		private sealed class GeneratorEntry
		{
			private readonly IQuoteApiHandler _apiHandler;
			private QuoteApiDescription? _description;

			public IQuoteGenerator Generator { get; }
			public JollyQuotesApi Api { get; }
			public QuoteApiDescription Description => _description ??= _apiHandler.CreateDescription(Api);

			public GeneratorEntry(JollyQuotesApi api, IQuoteGenerator generator, IQuoteApiHandler apiHandler)
			{
				Generator = generator;
				Api = api;
				_apiHandler = apiHandler;
			}
		}

		private const string SOURCE = "JollyQuotes";

		/// <inheritdoc/>
		public IQuoteApiHandler ApiHandler { get; }

		/// <summary>
		/// <see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.
		/// </summary>
		public IPossibility SharedPossibility { get; }

		/// <summary>
		/// Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.
		/// </summary>
		public OptionalPossibility ApiPossibility { get; }

		/// <summary>
		/// Represents all currently enabled <c>JollyQuotes</c> APIs.
		/// </summary>
		public JollyQuotesApi EnabledApis { get; private set; }

		private readonly GeneratorEntry[] _generators;

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class.
		/// </summary>
		public JollyQuotesGenerator() : this(default, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with underlaying <paramref name="apiHandler"/> and <paramref name="apiPossibility"/> specified.
		/// </summary>
		/// <param name="apiHandler">Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.</param>
		/// <param name="apiPossibility">Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.</param>
		/// <param name="sharedPossibility"><see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.</param>
		public JollyQuotesGenerator(
			IQuoteApiHandler? apiHandler = default,
			OptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : base(SOURCE, false)
		{
		}

		public JollyQuotesGenerator(HttpClient client) : this(client, default, default)
		{

		}

		public JollyQuotesGenerator(
			HttpClient client,
			IQuoteApiHandler? apiHandler = default,
			OptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : base(client, SOURCE)
		{

		}

		public JollyQuotesGenerator(IResourceResolver resolver) : this(resolver, default, default)
		{

		}

		public JollyQuotesGenerator(
			IResourceResolver resolver,
			IQuoteApiHandler? apiHandler = default,
			OptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : base(resolver, SOURCE)
		{
			SharedPossibility = sharedPossibility ?? new Possibility();
			ApiHandler = apiHandler ?? new QuoteApiHandler(SharedPossibility);
			ApiPossibility = apiPossibility ?? GetDefaultApiPossibility();
		}

		private static OptionalPossibility GetDefaultApiPossibility()
		{
			const int CAPACITY = 3;
			const int MAX_POSSIBILITY = 1000;
			const int DEFAULT_POSSIBILITY = 1;
			const int KANYE_POSSIBILITY = DEFAULT_POSSIBILITY;
			const int TRONALD_POSSIBILITY = DEFAULT_POSSIBILITY;
			const int QUOTABLE_POSSIBILITY = MAX_POSSIBILITY - KANYE_POSSIBILITY - TRONALD_POSSIBILITY;

			OptionalPossibility possibility = new()
			{
				Capacity = CAPACITY,
				Max = MAX_POSSIBILITY
			};

			possibility
				.AddOption(ApiNames.KanyeRest,		KANYE_POSSIBILITY)
				.AddOption(ApiNames.TronaldDump,	TRONALD_POSSIBILITY)
				.AddOption(ApiNames.Quotable,		QUOTABLE_POSSIBILITY);

			return possibility;
		}

		private GeneratorEntry[] InitializeGenerators()
		{
			JollyQuotesApi[] all = JollyQuotesApi.All.GetFlags();
			int length = all.Length;

			GeneratorEntry[] generators = new GeneratorEntry[length];

			for (int i = 0; i < length; i++)
			{
				JollyQuotesApi api = all[i];
				IQuoteGenerator generator = ApiHandler.CreateGenerator(api, Resolver);
				generators[i] = new(api, generator, ApiHandler);
			}

			return generators;
		}

		/// <summary>
		/// Enables the specified <c>JollyQuotes</c> APIs and disables all others.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to enable.</param>
		public void Switch(JollyQuotesApi api)
		{
			EnabledApis = JollyQuotesApi.None;
			EnableApi(api);
		}

		/// <summary>
		/// Enables the specified <c>JollyQuotes</c> APIs.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to enable.</param>
		public void EnableApi(JollyQuotesApi api)
		{
			if(api == EnabledApis)
			{
				return;
			}

			if (!TryEnable(api))
			{
				foreach (JollyQuotesApi flag in api.GetFlags())
				{
					TryEnable(flag);
				}
			}

			bool TryEnable(JollyQuotesApi api)
			{
				int index = api.EnumToIndex();

				if (index >= 0)
				{
					if (!EnabledApis.HasFlag(api))
					{
						EnabledApis |= api;
					}

					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Disables the specified <c>JollyQuotes</c> APIs.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to disable.</param>
		public void DisableApi(JollyQuotesApi api)
		{
			if(api == EnabledApis)
			{
				EnabledApis = JollyQuotesApi.None;
				return;
			}

			if (!TryDisable(api))
			{
				foreach (JollyQuotesApi flag in api.GetFlags())
				{
					TryDisable(flag);
				}
			}

			bool TryDisable(JollyQuotesApi api)
			{
				int index = api.EnumToIndex();

				if (index >= 0)
				{
					if (EnabledApis.HasFlag(api))
					{
						EnabledApis &= ~api;
					}

					return true;
				}

				return false;
			}
		}

		/// <inheritdoc/>
		public void EnableAll()
		{
			EnabledApis = JollyQuotesApi.All;
		}

		/// <inheritdoc/>
		public void DisableAll()
		{
			EnabledApis = JollyQuotesApi.None;
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> API is enabled.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to check if is enabled.</param>
		public bool IsEnabled(JollyQuotesApi api)
		{
			return EnabledApis.HasFlag(api);
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetApis()
		{
			return _generators.Select(g => g.Description);
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetEnabledApis()
		{
			for (int i = 0; i < _generators.Length; i++)
			{
				JollyQuotesApi api = QuoteUtility.IndexToEnum(i);

				if (EnabledApis.HasFlag(api))
				{
					yield return _generators[i].Description;
				}
			}
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetDisabledApis()
		{
			for (int i = 0; i < _generators.Length; i++)
			{
				JollyQuotesApi api = QuoteUtility.IndexToEnum(i);

				if (!EnabledApis.HasFlag(api))
				{
					yield return _generators[i].Description;
				}
			}
		}

		private GeneratorEntry GetEntry(JollyQuotesApi api)
		{
			int index = api.EnumToIndex();

			if (index == -1)
			{
				throw QuoteUtility.Exc_InvalidEnum(nameof(api));
			}

			return _generators[index];
		}

		/// <summary>
		/// Returns a <see cref="QuoteApiDescription"/> associated with the specified <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> api to get the <see cref="QuoteApiDescription"/> associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API.</exception>
		public QuoteApiDescription GetDescription(JollyQuotesApi api)
		{
			GeneratorEntry entry = GetEntry(api);
			return entry.Description;
		}

		/// <summary>
		/// Returns a <see cref="IQuoteGenerator"/> associated with the specified <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> api to get the <see cref="IQuoteGenerator"/> associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API.</exception>
		public IQuoteGenerator GetGenerator(JollyQuotesApi api)
		{
			GeneratorEntry entry = GetEntry(api);
			return entry.Generator;
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetGenerators()
		{
			return _generators.Select(g => g.Generator);
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetEnabledGenerators()
		{
			for (int i = 0; i < _generators.Length; i++)
			{
				JollyQuotesApi api = QuoteUtility.IndexToEnum(i);

				if (EnabledApis.HasFlag(api))
				{
					yield return _generators[i].Generator;
				}
			}
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetDisabledGenerators()
		{
			for (int i = 0; i < _generators.Length; i++)
			{
				JollyQuotesApi api = QuoteUtility.IndexToEnum(i);

				if (!EnabledApis.HasFlag(api))
				{
					yield return _generators[i].Generator;
				}
			}
		}

		private static JollyQuotesApi ParseApi(string apiName)
		{
			if(!QuoteUtility.TryParseApi(apiName, out JollyQuotesApi api))
			{
				throw QuoteUtility.Exc_UnknownApiNameOrNull(apiName);
			}

			return api;
		}

		void ICompositeQuoteGenerator.EnableApi(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			EnableApi(api);
		}

		void ICompositeQuoteGenerator.DisableApi(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			DisableApi(api);
		}

		void ICompositeQuoteGenerator.Switch(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			Switch(api);
		}

		bool ICompositeQuoteGenerator.IsEnabled(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return IsEnabled(api);
		}

		QuoteApiDescription ICompositeQuoteGenerator.GetDescription(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return GetDescription(api);
		}

		IQuoteGenerator ICompositeQuoteGenerator.GetGenerator(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return GetGenerator(api);
		}

		IEnumerator<IQuoteGenerator> IEnumerable<IQuoteGenerator>.GetEnumerator()
		{
			return GetGenerators().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetGenerators().GetEnumerator();
		}
	}
}
