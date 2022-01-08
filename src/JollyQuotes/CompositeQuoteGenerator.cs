using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace JollyQuotes
{
	/// <inheritdoc cref="ICompositeQuoteGenerator"/>
	public class CompositeQuoteGenerator : QuoteClient<IQuote>, ICompositeQuoteGenerator
	{
		private sealed class GeneratorEntry
		{
			private readonly IQuoteApiHandler _apiHandler;
			private QuoteApiDescription? _description;

			public string ApiName => Generator.ApiName;

			public bool Enabled { get; set; }

			public QuoteApiDescription Description => _description ??= _apiHandler.CreateDescription(ApiName);

			public IQuoteGenerator Generator { get; }

			public GeneratorEntry(IQuoteGenerator generator, IQuoteApiHandler apiHandler)
			{
				Generator = generator;
				_apiHandler = apiHandler;
				Enabled = true;
			}
		}

		private const string SOURCE = ApiNames.JollyQuotes;

		private readonly Dictionary<string, int> _map;
		private readonly List<GeneratorEntry> _generators;

		/// <inheritdoc/>
		public IQuoteApiHandler ApiHandler { get; }

		/// <summary>
		/// Determines possibility for a given <c>JollyQuotes</c> API to be used when generating new quote.
		/// </summary>
		public IOptionalPossibility ApiPossibility { get; }

		/// <inheritdoc/>
		public override string ApiName => ApiNames.JollyQuotes;

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeQuoteGenerator"/> class with target <paramref name="apiHandler"/> and <paramref name="apiPossibility"/> specified.
		/// </summary>
		/// <param name="apiHandler"><see cref="IQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given API to be used when generating new quote.</param>
		/// <exception cref="ArgumentNullException"><paramref name="apiHandler"/> is <see langword="null"/>.</exception>
		public CompositeQuoteGenerator(
			IQuoteApiHandler apiHandler,
			IOptionalPossibility? apiPossibility = default
		) : this(HttpResolver.CreateDefault(), apiHandler, apiPossibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeQuoteGenerator"/> class with an underlaying <paramref name="client"/>, <paramref name="apiHandler"/> and <paramref name="apiPossibility"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="apiHandler"><see cref="IQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given API to be used when generating new quote.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="apiHandler"/> is <see langword="null"/>.</exception>
		public CompositeQuoteGenerator(
			HttpClient client,
			IQuoteApiHandler apiHandler,
			IOptionalPossibility? apiPossibility = default
		) : this(new HttpResolver(client), apiHandler, apiPossibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CompositeQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="apiHandler"/> and <paramref name="apiPossibility"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="apiHandler"><see cref="IQuoteApiHandler"/> that is used to access API-specific objects.</param>
		/// <param name="apiPossibility">Determines possibility for a given API to be used when generating new quote.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>. -or- <paramref name="apiHandler"/> is <see langword="null"/>.</exception>
		public CompositeQuoteGenerator(
			IResourceResolver resolver,
			IQuoteApiHandler apiHandler,
			IOptionalPossibility? apiPossibility = default
		) : base(resolver, SOURCE)
		{
			if(apiHandler is null)
			{
				throw Error.Null(nameof(apiHandler));
			}

			_generators = new();
			_map = new();

			ApiHandler = apiHandler;

			if(apiPossibility is null)
			{
				string[] apis = ApiHandler.GetApis().ToArray();
				ApiPossibility = QuoteUtility.GetDefaultPossibility(apis);
				InitializeGenerators(apis);
			}
			else
			{
				ApiPossibility = apiPossibility;
				InitializeGenerators(ApiHandler.GetApis());
			}
		}

		private void InitializeGenerators(IEnumerable<string> apis)
		{
			foreach (string apiName in apis)
			{
				IQuoteGenerator generator = ApiHandler.CreateGenerator(apiName, Resolver);

				_map.Add(apiName, _generators.Count);
				_generators.Add(new(generator, ApiHandler));
			}
		}

		/// <inheritdoc/>
		public void ForceUpdate()
		{
			_generators.Clear();
			_map.Clear();

			InitializeGenerators(ApiHandler.GetApis());
		}

		/// <inheritdoc/>
		public void DisableAll()
		{
			foreach (GeneratorEntry entry in _generators)
			{
				entry.Enabled = false;
			}
		}

		/// <inheritdoc/>
		public void DisableApi(string apiName)
		{
			SetState(apiName, false);
		}

		private void SetState(string apiName, bool enabled)
		{
			GeneratorEntry entry = GetEntry(apiName);
			entry.Enabled = enabled;
		}

		/// <inheritdoc/>
		public void EnableAll()
		{
			foreach (GeneratorEntry entry in _generators)
			{
				entry.Enabled = true;
			}
		}

		/// <inheritdoc/>
		public void EnableApi(string apiName)
		{
			SetState(apiName, true);
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetApis()
		{
			return _generators.Select(g => g.Description);
		}

		/// <inheritdoc/>
		public QuoteApiDescription GetDescription(string apiName)
		{
			GeneratorEntry entry = GetEntry(apiName);
			return entry.Description;
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetDisabledApis()
		{
			return _generators
				.Where(g => !g.Enabled)
				.Select(g => g.Description);
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetDisabledGenerators()
		{
			return _generators
				.Where(g => !g.Enabled)
				.Select(g => g.Generator);
		}

		/// <inheritdoc/>
		public IEnumerable<QuoteApiDescription> GetEnabledApis()
		{
			return _generators
				.Where(g => g.Enabled)
				.Select(g => g.Description);
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetEnabledGenerators()
		{
			return _generators
				.Where(g => g.Enabled)
				.Select(g => g.Generator);
		}

		/// <inheritdoc/>
		public IQuoteGenerator GetGenerator(string apiName)
		{
			GeneratorEntry entry = GetEntry(apiName);
			return entry.Generator;
		}

		/// <inheritdoc/>
		public IEnumerable<IQuoteGenerator> GetGenerators()
		{
			return _generators.Select(g => g.Generator);
		}

		/// <inheritdoc/>
		public bool IsEnabled(string apiName)
		{
			GeneratorEntry entry = GetEntry(apiName);
			return entry.Enabled;
		}

		/// <inheritdoc/>
		public bool IsRegistered(string apiName)
		{
			return TryGetEntry(apiName, out _);
		}

		/// <inheritdoc/>
		public void SwitchTo(string apiName)
		{
			GeneratorEntry entry = GetEntry(apiName);
			DisableAll();

			entry.Enabled = true;
		}

		/// <inheritdoc/>
		public override IQuote GetRandomQuote()
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote();
		}

		/// <inheritdoc/>
		public override IQuote? GetRandomQuote(params string[]? tags)
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote(tags);
		}

		/// <inheritdoc/>
		public override IQuote? GetRandomQuote(string tag)
		{
			IQuoteGenerator generator = GetRandomGenerator();
			return generator.GetRandomQuote(tag);
		}

		/// <summary>
		/// Returns a randomly picked <see cref="IQuoteGenerator"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public IQuoteGenerator GetRandomGenerator()
		{
			GeneratorEntry entry = GetRandomApi();
			return entry.Generator;
		}

		/// <summary>
		/// Returns a randomly picked <see cref="QuoteApiDescription"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="IOptionalPossibility.Determine()"/> of <see cref="ApiPossibility"/> returned an unknown API.</exception>
		public QuoteApiDescription GetRandomDescription()
		{
			GeneratorEntry entry = GetRandomApi();
			return entry.Description;
		}

		private GeneratorEntry GetRandomApi()
		{
			NamedOption result = ApiPossibility.Determine();

			if (!TryGetEntry(result.Name, out GeneratorEntry? entry))
			{
				throw QuoteUtility.Exc_PossibilityReturnedUnknownApi(result.Name);
			}

			return entry;
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

				foreach (GeneratorEntry entry in _generators)
				{
					Internals.TryDispose(entry.Generator);
				}
			}

			base.Dispose(disposing);
		}

		IEnumerator<IQuoteGenerator> IEnumerable<IQuoteGenerator>.GetEnumerator()
		{
			return GetGenerators().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetGenerators().GetEnumerator();
		}

		private bool TryGetEntry(string apiName, [NotNullWhen(true)]out GeneratorEntry? entry)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			if (!_map.TryGetValue(apiName, out int i))
			{
				entry = default;
				return false;
			}

			entry = _generators[i];
			return true;
		}

		private GeneratorEntry GetEntry(string apiName)
		{
			if (!TryGetEntry(apiName, out GeneratorEntry? entry))
			{
				throw QuoteUtility.Exc_UnknownApiName(apiName);
			}

			return entry;
		}
	}
}
