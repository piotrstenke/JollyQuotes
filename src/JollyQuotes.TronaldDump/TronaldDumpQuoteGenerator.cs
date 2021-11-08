using System;
using System.Collections.Generic;
using System.Net.Http;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that generates quotes using the <c>Tronald Dump</c> web API.
	/// </summary>
	public partial class TronaldDumpQuoteGenerator : QuoteClient<TronaldDumpQuote>.WithCache
	{
		private readonly InternalGenerator _internalGenerator;

		/// <summary>
		/// <see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.
		/// </summary>
		public ITronaldDumpModelConverter ModelConverter { get; }

		/// <summary>
		/// <see cref="System.Random"/> used to pick random quotes.
		/// </summary>
		public Random Random { get; }

		/// <summary>
		/// <see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API.
		/// </summary>
		public ITronaldDumpService Service { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="TronaldDumpResources.APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client)
			: base(Internals.GetResolverFromClient(TronaldDumpService.SetAddress(client), out IStreamResolver resolver), TronaldDumpResources.BaseAddress)
		{
			ModelConverter = new TronaldDumpModelConverter();
			Service = new TronaldDumpService(resolver);
			Random = new();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/> and target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="TronaldDumpResources.APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client, ITronaldDumpService service)
			: this(client, service, new TronaldDumpModelConverter(), new Random())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/>, target <paramref name="service"/> and model <paramref name="converter"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="converter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="client"/> is <see langword="null"/>. -or-
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="converter"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="TronaldDumpResources.APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client, ITronaldDumpService service, ITronaldDumpModelConverter converter)
			: this(client, service, converter, new Random())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/>,
		/// target <paramref name="service"/>, model <paramref name="converter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="converter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random"><see cref="System.Random"/> used to pick random quotes.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="client"/> is <see langword="null"/>. -or-
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="converter"/> is <see langword="null"/>. -or-
		/// <paramref name="random"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="TronaldDumpResources.APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(
			HttpClient client,
			ITronaldDumpService service,
			ITronaldDumpModelConverter converter,
			Random random,
			IQuoteCache<TronaldDumpQuote>? cache = null,
			IPossibility? possibility = null
		) : this(new HttpResolver(TronaldDumpService.SetAddress(client)), service, converter, random, cache, possibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(IStreamResolver resolver) : base(resolver, TronaldDumpResources.BaseAddress)
		{
			Service = new TronaldDumpService(resolver);
			ModelConverter = new TronaldDumpModelConverter();
			Random = new();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> and target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(IResourceResolver resolver, ITronaldDumpService service)
			: this(resolver, service, new TronaldDumpModelConverter(), new Random())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, target <paramref name="service"/> and model <paramref name="converter"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="converter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="resolver"/> is <see langword="null"/>. -or-
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="converter"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(IResourceResolver resolver, ITronaldDumpService service, ITronaldDumpModelConverter converter)
			: this(resolver, service, converter, new Random())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>,
		/// target <paramref name="service"/>, model <paramref name="converter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="converter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random"><see cref="System.Random"/> used to pick random quotes.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="resolver"/> is <see langword="null"/>. -or-
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="converter"/> is <see langword="null"/>. -or-
		/// <paramref name="random"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(
			IResourceResolver resolver,
			ITronaldDumpService service,
			ITronaldDumpModelConverter converter,
			Random random,
			IQuoteCache<TronaldDumpQuote>? cache = null,
			IPossibility? possibility = null
		) : base(resolver, TronaldDumpResources.BaseAddress, cache, possibility)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			if (converter is null)
			{
				throw Error.Null(nameof(converter));
			}

			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			Service = service;
			ModelConverter = converter;
			Random = random;
			_internalGenerator = new(this);
		}

		/// <inheritdoc cref="EnumerableQuoteGenerator{T}.WithCache.GetAllQuotes(string, QuoteInclude)"/>
		public IEnumerable<TronaldDumpQuote> GetAllQuotes(string tag, QuoteInclude which = QuoteInclude.All)
		{
			return _internalGenerator.GetAllQuotes(tag, which);
		}

		/// <inheritdoc cref="EnumerableQuoteGenerator{T}.WithCache.GetAllQuotes(string[], QuoteInclude)"/>
		public IEnumerable<TronaldDumpQuote> GetAllQuotes(string[]? tags, QuoteInclude which = QuoteInclude.All)
		{
			return _internalGenerator.GetAllQuotes(tags, which);
		}

		/// <inheritdoc cref="EnumerableQuoteGenerator{T}.WithCache.DownloadAllQuotes(string)"/>
		protected virtual IEnumerable<TronaldDumpQuote> DownloadAllQuotes(string tag)
		{
			QuoteSearchModel search = new(null as string, tag);
			SearchResultModel<QuoteListModel> result = Service.SearchQuotes(search).Result;

			if (result.Count == 0)
			{
				return Array.Empty<TronaldDumpQuote>();
			}

			return ModelConverter.EnumerateQuotes(result.Embedded);
		}

		/// <inheritdoc cref="EnumerableQuoteGenerator{T}.WithCache.DownloadAllQuotes(string[])"/>
		protected virtual IEnumerable<TronaldDumpQuote> DownloadAllQuotes(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return Array.Empty<TronaldDumpQuote>();
			}

			return Yield();

			IEnumerable<TronaldDumpQuote> Yield()
			{
				foreach (string tag in tags)
				{
					foreach (TronaldDumpQuote quote in DownloadAllQuotes(tag))
					{
						yield return quote;
					}
				}
			}
		}

		/// <inheritdoc/>
		protected override TronaldDumpQuote DownloadRandomQuote()
		{
			QuoteModel model = Service.GetRandomQuote().Result;

			return ModelConverter.ConvertQuoteModel(model);
		}

		/// <inheritdoc/>
		protected override TronaldDumpQuote? DownloadRandomQuote(string tag)
		{
			QuoteSearchModel search = new(null as string, tag);
			SearchResultModel<QuoteListModel> result = Service.SearchQuotes(search).Result;

			if (result.Count == 0)
			{
				return null;
			}

			int numPages = ModelConverter.CountPages(result);

			if (numPages > 1)
			{
				int targetPage = Random.Next(0, numPages);

				if (targetPage > 0)
				{
					search = new(null as string, tag, targetPage);
					result = Service.SearchQuotes(search).Result;
				}
			}

			int targetQuote = Random.Next(0, TronaldDumpResources.MaxItemsPerPage);

			QuoteModel model = result.Embedded.Quotes[targetQuote];

			return ModelConverter.ConvertQuoteModel(model);
		}
	}
}
