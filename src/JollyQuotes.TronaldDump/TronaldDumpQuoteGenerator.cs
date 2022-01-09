using System;
using System.Collections.Generic;
using System.Net.Http;
using JollyQuotes.TronaldDump.Models;

using static JollyQuotes.Internals;
using static JollyQuotes.TronaldDump.TronaldDumpResources;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// <see cref="IQuoteGenerator"/> that generates quotes using the <c>Tronald Dump</c> web API.
	/// </summary>
	public partial class TronaldDumpQuoteGenerator : QuoteClient<TronaldDumpQuote>.WithCache
	{
		private readonly InternalGenerator _internalGenerator;

		/// <inheritdoc/>
		public override string ApiName => TronaldDumpResources.ApiName;

		/// <summary>
		/// <see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.
		/// </summary>
		public ITronaldDumpModelConverter ModelConverter { get; }

		/// <summary>
		/// Service that generates a value used to determine which random quote to return.
		/// </summary>
		public IRandomNumberGenerator RandomNumberGenerator { get; }

		/// <inheritdoc cref="ITronaldDumpService.Resolver"/>
		public new IStreamResolver Resolver => (base.Resolver as IStreamResolver)!;

		/// <summary>
		/// <see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API.
		/// </summary>
		public ITronaldDumpService Service { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class.
		/// </summary>
		public TronaldDumpQuoteGenerator() : this(CreateDefaultResolver())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="ApiPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client)
			: this(client, new TronaldDumpModelConverter(), new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(
			HttpClient client,
			ITronaldDumpModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<TronaldDumpQuote>? cache = default,
			IPossibility? possibility = default
		) : this(new HttpResolver(client), modelConverter, random, cache, possibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with a target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(ITronaldDumpService service) : base(GetResolverFromService(service), BaseAddress)
		{
			Service = service;
			ModelConverter = CreateConverter(service);
			RandomNumberGenerator = new ThreadRandom();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with a target <paramref name="service"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(
			ITronaldDumpService service,
			ITronaldDumpModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<TronaldDumpQuote>? cache = default,
			IPossibility? possibility = default
		) : base(GetResolverFromService(service), BaseAddress, cache, possibility)
		{
			Service = service;
			ModelConverter = modelConverter ?? CreateConverter(service);
			RandomNumberGenerator = random ?? new ThreadRandom();
			_internalGenerator = new(this);
		}

		private static ITronaldDumpModelConverter CreateConverter(ITronaldDumpService service)
		{
			return service is TronaldDumpService s
				? s.ModelConverter
				: new TronaldDumpModelConverter();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(IStreamResolver resolver) : base(resolver, BaseAddress)
		{
			ModelConverter = new TronaldDumpModelConverter();
			Service = new TronaldDumpService(resolver, ModelConverter);
			RandomNumberGenerator = new ThreadRandom();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(
			IStreamResolver resolver,
			ITronaldDumpModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<TronaldDumpQuote>? cache = default,
			IPossibility? possibility = default
		) : this(new TronaldDumpService(resolver, modelConverter), modelConverter, random, cache, possibility)
		{
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

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}

			if (disposing)
			{
				TryDispose(Service);
				TryDispose(RandomNumberGenerator);
				TryDispose(ModelConverter);
			}

			base.Dispose(disposing);
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
					if (string.IsNullOrWhiteSpace(tag))
					{
						continue;
					}

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
				int targetPage = RandomNumberGenerator.RandomNumber(0, numPages);

				// Page 0 was already returned at the beginning of the method, so there's no need to request it again.
				if (targetPage != 0)
				{
					search = search with { Page = targetPage };
					result = Service.SearchQuotes(search).Result;
				}
			}

			int targetQuote = RandomNumberGenerator.RandomNumber(0, result.Count);

			QuoteModel model = result.Embedded.Quotes[targetQuote];

			return ModelConverter.ConvertQuoteModel(model);
		}
	}
}
