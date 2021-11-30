using System;
using System.Collections.Generic;
using System.Net.Http;
using JollyQuotes.TronaldDump.Models;

using static JollyQuotes.Internals;
using static JollyQuotes.TronaldDump.TronaldDumpResources;

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
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client)
			: this(client, new TronaldDumpModelConverter(), new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="client"/> and <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="client"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(HttpClient client, ITronaldDumpModelConverter modelConverter)
			: this(client, modelConverter, new ThreadRandom())
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
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="client"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>. -or-
		/// <paramref name="random"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="APIPage"/>.</exception>
		public TronaldDumpQuoteGenerator(
			HttpClient client,
			ITronaldDumpModelConverter modelConverter,
			IRandomNumberGenerator random,
			IQuoteCache<TronaldDumpQuote>? cache = null,
			IPossibility? possibility = null
		) : this(new HttpResolver(SetAddress(client)), modelConverter, random, cache, possibility)
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
			ModelConverter = new TronaldDumpModelConverter();
			RandomNumberGenerator = new ThreadRandom();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with a target <paramref name="service"/> and <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(ITronaldDumpService service, ITronaldDumpModelConverter modelConverter) : this(service, modelConverter, new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with a target <paramref name="service"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="service"><see cref="ITronaldDumpService"/> used to perform actions using the <c>Tronald Dump</c> API</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="service"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>. -or-
		/// <paramref name="random"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(
			ITronaldDumpService service,
			ITronaldDumpModelConverter modelConverter,
			IRandomNumberGenerator random,
			IQuoteCache<TronaldDumpQuote>? cache = null,
			IPossibility? possibility = null
		) : base(GetResolverFromService(service), BaseAddress, cache, possibility)
		{
			if (modelConverter is null)
			{
				throw Error.Null(nameof(modelConverter));
			}

			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			Service = service;
			ModelConverter = modelConverter;
			RandomNumberGenerator = random;
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpQuoteGenerator(IStreamResolver resolver) : base(resolver, BaseAddress)
		{
			Service = new TronaldDumpService(resolver);
			ModelConverter = new TronaldDumpModelConverter();
			RandomNumberGenerator = new ThreadRandom();
			_internalGenerator = new(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> and <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="resolver"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(IStreamResolver resolver, ITronaldDumpModelConverter modelConverter)
			: this(resolver, modelConverter, new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access the requested <c>Tronald Dump</c> resources.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="resolver"/> is <see langword="null"/>. -or-
		/// <paramref name="modelConverter"/> is <see langword="null"/>. -or-
		/// <paramref name="random"/> is <see langword="null"/>.
		/// </exception>
		public TronaldDumpQuoteGenerator(
			IStreamResolver resolver,
			ITronaldDumpModelConverter modelConverter,
			IRandomNumberGenerator random,
			IQuoteCache<TronaldDumpQuote>? cache = null,
			IPossibility? possibility = null
		) : this(new TronaldDumpService(resolver), modelConverter, random, cache, possibility)
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
			if (!Disposed)
			{
				if (Service is IDisposable d)
				{
					d.Dispose();
				}

				if (RandomNumberGenerator is IDisposable r)
				{
					r.Dispose();
				}

				if (ModelConverter is IDisposable m)
				{
					m.Dispose();
				}

				base.Dispose(disposing);
			}
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
