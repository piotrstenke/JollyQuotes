using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
			private readonly IBuiltInQuoteApiHandler _apiHandler;
			private QuoteApiDescription? _description;

			public JollyQuotesApi Api { get; }

			public QuoteApiDescription Description => _description ??= _apiHandler.CreateDescription(Api);

			public IQuoteGenerator Generator { get; }

			public GeneratorEntry(JollyQuotesApi api, IQuoteGenerator generator, IBuiltInQuoteApiHandler apiHandler)
			{
				Generator = generator;
				Api = api;
				_apiHandler = apiHandler;
			}
		}

		private const string SOURCE = ApiNames.JollyQuotes;

		private readonly GeneratorEntry[] _generators;

		/// <summary>
		/// <see cref="IBuiltInQuoteApiHandler"/> that is used to access API-specific objects.
		/// </summary>
		public IBuiltInQuoteApiHandler ApiHandler { get; }

		/// <inheritdoc/>
		public override string ApiName => ApiNames.JollyQuotes;

		/// <summary>
		/// Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.
		/// </summary>
		public IOptionalPossibility ApiPossibility { get; }

		/// <summary>
		/// Represents all currently enabled <c>JollyQuotes</c> APIs.
		/// </summary>
		public JollyQuotesApi EnabledApis { get; private set; }

		/// <summary>
		/// <see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.
		/// </summary>
		public IPossibility SharedPossibility { get; }

		IQuoteApiHandler ICompositeQuoteGenerator.ApiHandler => ApiHandler;

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class.
		/// </summary>
		public JollyQuotesGenerator() : this((IBuiltInQuoteApiHandler?)default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with target <paramref name="apiHandler"/>, <paramref name="apiPossibility"/> and <paramref name="sharedPossibility"/> specified.
		/// </summary>
		/// <param name="apiHandler"><see cref="IBuiltInQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.</param>
		/// <param name="sharedPossibility"><see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.</param>
		public JollyQuotesGenerator(
			IBuiltInQuoteApiHandler? apiHandler = default,
			IOptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : this(HttpResolver.CreateDefault(), apiHandler, apiPossibility, sharedPossibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public JollyQuotesGenerator(HttpClient client) : this(client, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with an underlaying <paramref name="client"/>, <paramref name="apiHandler"/>, <paramref name="apiPossibility"/> and <paramref name="sharedPossibility"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="apiHandler"><see cref="IBuiltInQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.</param>
		/// <param name="sharedPossibility"><see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public JollyQuotesGenerator(
			HttpClient client,
			IBuiltInQuoteApiHandler? apiHandler = default,
			IOptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : this(new HttpResolver(client), apiHandler, apiPossibility, sharedPossibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public JollyQuotesGenerator(IResourceResolver resolver) : this(resolver, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JollyQuotesGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="apiHandler"/>, <paramref name="apiPossibility"/> and <paramref name="sharedPossibility"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="apiHandler"><see cref="IBuiltInQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.</param>
		/// <param name="sharedPossibility"><see cref="IPossibility"/> that is shared by underlaying <c>JollyQuotes</c> <see cref="IQuoteGenerator"/>s.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public JollyQuotesGenerator(
			IResourceResolver resolver,
			IBuiltInQuoteApiHandler? apiHandler = default,
			IOptionalPossibility? apiPossibility = default,
			IPossibility? sharedPossibility = default
		) : base(resolver, SOURCE)
		{
			SharedPossibility = sharedPossibility ?? new Possibility();
			ApiHandler = apiHandler ?? new BuiltInQuoteApiHandler(SharedPossibility);
			ApiPossibility = apiPossibility ?? GetDefaultApiPossibility();

			_generators = CreateGeneratorArray();
			EnabledApis = JollyQuotesApi.All;
		}

		/// <inheritdoc/>
		public void DisableAll()
		{
			EnabledApis = JollyQuotesApi.None;
		}

		/// <summary>
		/// Disables the specified <c>JollyQuotes</c> APIs.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to disable.</param>
		public void DisableApi(JollyQuotesApi api)
		{
			if (api == EnabledApis)
			{
				EnabledApis = JollyQuotesApi.None;
				return;
			}

			if(TryDisable(api))
			{
				return;
			}

			foreach (JollyQuotesApi flag in api.GetFlags())
			{
				if (!TryDisable(flag))
				{
					throw Exc_InvalidApi(api);
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

		/// <summary>
		/// Enables the specified <c>JollyQuotes</c> APIs.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to enable.</param>
		/// <exception cref="ArgumentException"><paramref name="api"/> is not a valid <c>JollyQuotes</c> value.</exception>
		public void EnableApi(JollyQuotesApi api)
		{
			if (api == EnabledApis)
			{
				return;
			}

			if(TryEnable(api))
			{
				return;
			}

			foreach (JollyQuotesApi flag in api.GetFlags())
			{
				if(!TryEnable(flag))
				{
					throw Exc_InvalidApi(api);
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

		/// <inheritdoc/>
		public void ForceUpdate()
		{
			InitializeGenerators(_generators);
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetApis()
		{
			return _generators.Select(g => g.Description);
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

		/// <summary>
		/// Returns a randomly picked <see cref="QuoteApiDescription"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public QuoteApiDescription GetRandomDescription()
		{
			JollyQuotesApi api = GetRandomApi();
			return GetDescription(api);
		}

		/// <summary>
		/// Returns a randomly picked <see cref="IQuoteGenerator"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public IQuoteGenerator GetRandomGenerator()
		{
			JollyQuotesApi api = GetRandomApi();
			return GetGenerator(api);
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public override IQuote GetRandomQuote()
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote();
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public override IQuote? GetRandomQuote(params string[]? tags)
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote(tags);
		}

		/// <inheritdoc/>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public override IQuote? GetRandomQuote(string tag)
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote(tag);
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> API is enabled.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to check if is enabled.</param>
		public bool IsEnabled(JollyQuotesApi api)
		{
			return EnabledApis.HasFlag(api);
		}

		/// <summary>
		/// Enables the specified <c>JollyQuotes</c> APIs and disables all others.
		/// </summary>
		/// <param name="api">Represents a <c>JollyQuotes</c> API to enable.</param>
		public void SwitchTo(JollyQuotesApi api)
		{
			EnabledApis = JollyQuotesApi.None;
			EnableApi(api);
		}

		void ICompositeQuoteGenerator.DisableApi(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			DisableApi(api);
		}

		void ICompositeQuoteGenerator.EnableApi(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			EnableApi(api);
		}

		QuoteApiDescription ICompositeQuoteGenerator.GetDescription(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return GetDescription(api);
		}

		IEnumerator<IQuoteGenerator> IEnumerable<IQuoteGenerator>.GetEnumerator()
		{
			return GetGenerators().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IQuoteGenerator>)this).GetEnumerator();
		}

		IQuoteGenerator ICompositeQuoteGenerator.GetGenerator(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return GetGenerator(api);
		}

		bool ICompositeQuoteGenerator.IsEnabled(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			return IsEnabled(api);
		}

		bool ICompositeQuoteGenerator.IsRegistered(string apiName)
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

		void ICompositeQuoteGenerator.SwitchTo(string apiName)
		{
			JollyQuotesApi api = ParseApi(apiName);
			SwitchTo(api);
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}

			if (disposing)
			{
				Internals.TryDispose(ApiHandler);
				Internals.TryDispose(ApiPossibility);
				Internals.TryDispose(SharedPossibility);

				foreach (GeneratorEntry entry in _generators)
				{
					Internals.TryDispose(entry.Generator);
				}
			}

			base.Dispose(disposing);
		}

		[DebuggerStepThrough]
		private static ArgumentException Exc_InvalidApi(JollyQuotesApi api)
		{
			return new ArgumentException($"Invalid {nameof(JollyQuotesApi)} value: '{api}'", nameof(api));
		}

		private static IOptionalPossibility GetDefaultApiPossibility()
		{
			const int CAPACITY = QuoteUtility.NUM_QUOTE_APIS;
			const int MAX_POSSIBILITY = 800;
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
				.AddOption(ApiNames.KanyeRest, KANYE_POSSIBILITY)
				.AddOption(ApiNames.TronaldDump, TRONALD_POSSIBILITY)
				.AddOption(ApiNames.Quotable, QUOTABLE_POSSIBILITY);

			return possibility;
		}

		private static JollyQuotesApi ParseApi(string apiName)
		{
			if (!QuoteUtility.TryParseApi(apiName, out JollyQuotesApi api))
			{
				throw QuoteUtility.Exc_UnknownApiNameOrNull(apiName);
			}

			return api;
		}

		private GeneratorEntry[] CreateGeneratorArray()
		{
			GeneratorEntry[] generators = new GeneratorEntry[QuoteUtility.NUM_QUOTE_APIS];
			InitializeGenerators(generators);
			return generators;
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

		private JollyQuotesApi GetRandomApi()
		{
			NamedOption result = ApiPossibility.Determine();

			if (!QuoteUtility.TryParseApi(result.Name, out JollyQuotesApi api))
			{
				throw QuoteUtility.Exc_PossibilityReturnedUnknownApi(result.Name);
			}

			return api;
		}

		private void InitializeGenerators(GeneratorEntry[] generators)
		{
			JollyQuotesApi[] all = JollyQuotesApi.All.GetFlags();

			for (int i = 0; i < all.Length; i++)
			{
				JollyQuotesApi api = all[i];
				IQuoteGenerator generator = ApiHandler.CreateGenerator(api, Resolver);
				generators[i] = new(api, generator, ApiHandler);
			}
		}
	}
}
