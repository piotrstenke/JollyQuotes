using System;
using System.Collections.Generic;
using System.Net.Http;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that generates real-life quotes of everybody's favorite rapper, Kanye West, using the <c>kanye.rest</c> API.
	/// </summary>
	public class KanyeQuoteGenerator : EnumerableQuoteClient<KanyeQuote>.WithCache, IKanyeRestService
	{
		private const string ERROR_TAGS_NOT_SUPPORTED = "kanye.rest does not support tags";

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class.
		/// </summary>
		public KanyeQuoteGenerator() : base(new HttpClient(), KanyeResources.MainPage)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <see cref="HttpClient"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access the <c>kanye.rest</c> API.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(HttpClient client) : base(client, KanyeResources.MainPage)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		public KanyeQuoteGenerator(BlockableQuoteCache<KanyeQuote>? cache, IPossibility? possibility) : base(new HttpClient(), cache, possibility)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeQuoteGenerator"/> class with an underlaying <see cref="HttpClient"/>, <paramref name="cache"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access the <c>kanye.rest</c> API.</param>
		/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeQuoteGenerator(HttpClient client, BlockableQuoteCache<KanyeQuote>? cache, IPossibility? possibility) : base(client, cache, possibility)
		{
		}

		IEnumerable<KanyeQuote> IKanyeRestService.GetAllQuotes()
		{
			return GetAllQuotes();
		}

		KanyeQuote IKanyeRestService.GetRandomQuote()
		{
			return GetRandomQuote()!;
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
