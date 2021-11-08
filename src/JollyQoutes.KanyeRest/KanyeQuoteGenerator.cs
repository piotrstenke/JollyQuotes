using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that generates real-life quotes of everybody's favorite rapper, Kanye West, using the <c>kanye.rest</c> API.
	/// </summary>
	public class KanyeQuoteGenerator : EnumerableQuoteClient<KanyeQuote>.WithCache
	{
		private const string _baseAddress = KanyeResources.MainPage;
		private const string ERROR_TAGS_NOT_SUPPORTED = "kanye.rest does not support tags";

		/// <summary>
		/// <see cref="IKanyeRestService"/> used to perform actions using the <c>kanye.rest</c> API.
		/// </summary>
		public IKanyeRestService Service { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(HttpClient client) : base(Internals.GetResolverFromClient(client, out IResourceResolver resolver), _baseAddress)
		{
			Service = new KanyeRestService(resolver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="client"/> and target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(HttpClient client, IKanyeRestService service) : base(client, _baseAddress)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="client"/>,
		/// target <paramref name="service"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used as the target <see cref="QuoteResolver{T}.Resolver"/>.</param>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(
			HttpClient client,
			IKanyeRestService service,
			IQuoteCache<KanyeQuote>? cache = null,
			IPossibility? possibility = null
		) : base(client, _baseAddress, cache, possibility)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>kanye.rest</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(IResourceResolver resolver) : base(resolver, _baseAddress)
		{
			Service = new KanyeRestService(resolver);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="resolver"/> and target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>kanye.rest</c> resources.</param>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(IResourceResolver resolver, IKanyeRestService service) : base(resolver, _baseAddress)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <paramref name="resolver"/>,
		/// target <paramref nae="service"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested <c>kanye.rest</c> resources.</param>
		/// <param name="service"><see cref="IKanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>. -or- <paramref name="service"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(
			IResourceResolver resolver,
			IKanyeRestService service,
			IQuoteCache<KanyeQuote>? cache = null,
			IPossibility? possibility = null
		) : base(resolver, _baseAddress, cache, possibility)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			Service = service;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with a target <paramref name="service"/> specified.
		/// </summary>
		/// <param name="service">
		/// <see cref="KanyeRestService"/> that is used to perform actions using the <c>kanye.rest</c> API.
		/// <para>The value of <see cref="KanyeRestService.Resolver"/> is used as the target <see cref="QuoteResolver{T}.Resolver"/></para>.
		/// </param>
		public KanyeQuoteGenerator(KanyeRestService service) : base(Internals.GetResolverFromService(service), _baseAddress)
		{
			Service = service;
		}

		/// <inheritdoc/>
		protected override IEnumerable<KanyeQuote> DownloadAllQuotes()
		{
			return Resolver.Resolve<KanyeQuote[]>(KanyeResources.Database);
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override IEnumerable<KanyeQuote> DownloadAllQuotes(params string[]? tags)
		{
			return Array.Empty<KanyeQuote>();
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override IEnumerable<KanyeQuote> DownloadAllQuotes(string tag)
		{
			return Array.Empty<KanyeQuote>();
		}

		/// <inheritdoc/>
		protected override KanyeQuote DownloadRandomQuote()
		{
			return Resolver.Resolve<KanyeQuote>(KanyeResources.APIPage);
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override KanyeQuote? DownloadRandomQuote(params string[]? tags)
		{
			return default;
		}

		/// <inheritdoc/>
		[Obsolete(ERROR_TAGS_NOT_SUPPORTED)]
		protected override KanyeQuote? DownloadRandomQuote(string tag)
		{
			return default;
		}
	}
}
