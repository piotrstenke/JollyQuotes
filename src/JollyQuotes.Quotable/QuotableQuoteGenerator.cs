using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using JollyQuotes.Quotable.Models;

using static JollyQuotes.Internals;
using static JollyQuotes.Quotable.QuotableResources;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// <see cref="IQuoteGenerator"/> that generates quotes using the <c>quotable</c> web API.
	/// </summary>
	public class QuotableQuoteGenerator : EnumerableQuoteClient<QuotableQuote>.WithCache
	{
		/// <inheritdoc/>
		public override string ApiName => QuotableResources.ApiName;

		/// <summary>
		/// <see cref="IQuotableModelConverter"/> used to convert models received from the <see cref="Service"/>.
		/// </summary>
		public IQuotableModelConverter ModelConverter { get; }

		/// <summary>
		/// Service that generates a value used to determine which random quote to return.
		/// </summary>
		public IRandomNumberGenerator RandomNumberGenerator { get; }

		/// <summary>
		/// <see cref="IQuotableService"/> used to perform actions using the <c>quotable</c> API.
		/// </summary>
		public IQuotableService Service { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class.
		/// </summary>
		public QuotableQuoteGenerator() : this(CreateDefaultResolver())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="ApiPage"/>.</exception>
		public QuotableQuoteGenerator(HttpClient client)
			: this(client, new QuotableModelConverter(), new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with an underlaying <paramref name="client"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="modelConverter"><see cref="IQuotableModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public QuotableQuoteGenerator(
			HttpClient client,
			IQuotableModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<QuotableQuote>? cache = default,
			IPossibility? possibility = default
		) : this(new HttpResolver(client), modelConverter, random, cache, possibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with a target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="service"><see cref="IQuotableService"/> used to perform actions using the <c>quotable</c> API</param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public QuotableQuoteGenerator(IQuotableService service) : base(service.GetResolverFromService(), BASE_ADDRESS)
		{
			Service = service;
			ModelConverter = CreateConverter(service);
			RandomNumberGenerator = new ThreadRandom();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with a target <paramref name="service"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="service"><see cref="IQuotableService"/> used to perform actions using the <c>quotable</c> API</param>
		/// <param name="modelConverter"><see cref="IQuotableModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public QuotableQuoteGenerator(
			IQuotableService service,
			IQuotableModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<QuotableQuote>? cache = default,
			IPossibility? possibility = default
		) : base(service.GetResolverFromService(), BASE_ADDRESS, cache, possibility)
		{
			Service = service;
			ModelConverter = modelConverter ?? CreateConverter(service);
			RandomNumberGenerator = random ?? new ThreadRandom();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>quotable</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public QuotableQuoteGenerator(IResourceResolver resolver) : base(resolver, BASE_ADDRESS)
		{
			ModelConverter = new QuotableModelConverter();
			Service = new QuotableService(resolver, ModelConverter);
			RandomNumberGenerator = new ThreadRandom();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="modelConverter"/> and <paramref name="random"/> number generator specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>quotable</c> resources.</param>
		/// <param name="modelConverter"><see cref="IQuotableModelConverter"/> used to convert models received from the <see cref="Service"/>.</param>
		/// <param name="random">Service that generates a value used to determine which random quote to return.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public QuotableQuoteGenerator(
			IResourceResolver resolver,
			IQuotableModelConverter? modelConverter = default,
			IRandomNumberGenerator? random = default,
			IQuoteCache<QuotableQuote>? cache = default,
			IPossibility? possibility = default
		) : this(new QuotableService(resolver, modelConverter), modelConverter, random, cache, possibility)
		{
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
				TryDispose(ModelConverter);
			}

			base.Dispose(disposing);
		}

		/// <inheritdoc/>
		protected override IEnumerable<QuotableQuote> DownloadAllQuotes()
		{
			SearchResultModel<QuoteModel> searchResult = Service.GetQuotes().Result;
			return searchResult.Results.Select(quote => ModelConverter.ConvertQuoteModel(quote));
		}

		/// <inheritdoc/>
		protected override IEnumerable<QuotableQuote> DownloadAllQuotes(string tag)
		{
			QuoteListSearchModel searchModel = new()
			{
				Tags = new(tag)
			};

			SearchResultModel<QuoteModel> searchResult = Service.GetQuotes(searchModel).Result;

			if (searchResult.Count > 0)
			{
				int current = 0;

				while (true)
				{
					foreach (QuoteModel quote in searchResult.Results)
					{
						yield return ModelConverter.ConvertQuoteModel(quote);
					}

					current += searchResult.Count;

					if (current >= searchResult.TotalCount)
					{
						break;
					}

					searchModel = searchModel with
					{
						Page = searchModel.Page + 1
					};

					searchResult = Service.GetQuotes(searchModel).Result;
				}
			}
		}

		/// <inheritdoc/>
		protected override IEnumerable<QuotableQuote> DownloadAllQuotes(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return DownloadAllQuotes();
			}

			return Yield();

			IEnumerable<QuotableQuote> Yield()
			{
				HashSet<string> quotes = new();

				foreach (string tag in tags)
				{
					if (string.IsNullOrWhiteSpace(tag))
					{
						continue;
					}

					foreach (QuotableQuote quote in DownloadAllQuotes(tag))
					{
						if (quotes.Add(quote.Id))
						{
							yield return quote;
						}
					}
				}
			}
		}

		/// <inheritdoc/>
		protected override QuotableQuote DownloadRandomQuote()
		{
			QuoteModel quote = Service.GetRandomQuote().Result;

			return ModelConverter.ConvertQuoteModel(quote);
		}

		/// <inheritdoc/>
		protected override QuotableQuote? DownloadRandomQuote(string tag)
		{
			QuoteSearchModel searchModel = new()
			{
				Tags = new(tag)
			};

			QuoteModel quote = Service.GetRandomQuote(searchModel).Result;

			return ModelConverter.ConvertQuoteModel(quote);
		}

		/// <inheritdoc/>
		protected override QuotableQuote? DownloadRandomQuote(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return default;
			}

			int randomIndex = RandomNumberGenerator.RandomNumber(0, tags.Length);
			string randomTag = tags[randomIndex];

			return DownloadRandomQuote(randomTag);
		}

		private static IQuotableModelConverter CreateConverter(IQuotableService service)
		{
			return service is QuotableService s
				? s.ModelConverter
				: new QuotableModelConverter();
		}
	}
}
