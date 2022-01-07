using System;
using System.Collections.Generic;
using System.Net.Http;

using static JollyQuotes.Internals;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// <see cref="IQuoteGenerator"/> that generates real-life quotes of everybody's favorite rapper, Kanye West, using the <c>kanye.rest</c> API.
	/// </summary>
	public class KanyeRestQuoteGenerator : EnumerableQuoteClient<KanyeRestQuote>.WithCache
	{
		private const string BASE_ADDRESS = KanyeRestResources.MainPage;
		private const string ERROR_TAGS_NOT_SUPPORTED = "kanye.rest does not support tags";

		/// <summary>
		/// <see cref="IKanyeRestService"/> used to perform actions using the <c>kanye.rest</c> API.
		/// </summary>
		public IKanyeRestService Service { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class.
		/// </summary>
		public KanyeRestQuoteGenerator() : base(BASE_ADDRESS, false)
		{
			Service = new KanyeRestService(Resolver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(HttpClient client) : this(client, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with an underlaying <paramref name="client"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(
			HttpClient client,
			IQuoteCache<KanyeRestQuote>? cache = default,
			IPossibility? possibility = default
		) : base(client, BASE_ADDRESS, cache, possibility)
		{
			Service = new KanyeRestService(Resolver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with a target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(IKanyeRestService service) : this(service, null, null)
		{
			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with a target <paramref name="service"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(
			IKanyeRestService service,
			IQuoteCache<KanyeRestQuote>? cache = default,
			IPossibility? possibility = default
		) : base(GetResolverFromService(service), BASE_ADDRESS, cache, possibility)
		{
			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>kanye.rest</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(IResourceResolver resolver) : this(resolver, null, null)
		{
			Service = new KanyeRestService(resolver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>kanye.rest</c> resources.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public KanyeRestQuoteGenerator(
			IResourceResolver resolver,
			IQuoteCache<KanyeRestQuote>? cache = default,
			IPossibility? possibility = default
		) : base(resolver, BASE_ADDRESS, cache, possibility)
		{
			Service = new KanyeRestService(Resolver);
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (!Disposed)
			{
				if (disposing && Service is IDisposable d)
				{
					d.Dispose();
				}

				base.Dispose(disposing);
			}
		}

		/// <inheritdoc/>
		protected override IEnumerable<KanyeRestQuote> DownloadAllQuotes()
		{
			return Service.GetAllQuotes().Result;
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override IEnumerable<KanyeRestQuote> DownloadAllQuotes(params string[]? tags)
		{
			return Array.Empty<KanyeRestQuote>();
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override IEnumerable<KanyeRestQuote> DownloadAllQuotes(string tag)
		{
			return Array.Empty<KanyeRestQuote>();
		}

		/// <inheritdoc/>
		protected override KanyeRestQuote DownloadRandomQuote()
		{
			return Service.GetRandomQuote().Result;
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override KanyeRestQuote? DownloadRandomQuote(params string[]? tags)
		{
			return default;
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override KanyeRestQuote? DownloadRandomQuote(string tag)
		{
			return default;
		}
	}
}
