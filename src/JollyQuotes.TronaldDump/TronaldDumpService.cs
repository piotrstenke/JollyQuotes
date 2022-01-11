using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <inheritdoc cref="ITronaldDumpService"/>
	public class TronaldDumpService : QuoteService, ITronaldDumpService
	{
		/// <inheritdoc/>
		public new IStreamResolver Resolver => (base.Resolver as IStreamResolver)!;

		/// <summary>
		/// <see cref="ITronaldDumpModelConverter"/> used to convert data received from the <c>Tronald Dump</c> API to usable objects.
		/// </summary>
		public ITronaldDumpModelConverter ModelConverter { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class.
		/// </summary>
		public TronaldDumpService() : this(TronaldDumpResources.CreateDefaultResolver())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public TronaldDumpService(HttpClient client) : this(client, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> and a <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="Resolver"/>.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert data received from the <c>Tronald Dump</c> API to usable objects.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public TronaldDumpService(HttpClient client, ITronaldDumpModelConverter? modelConverter) : this(new HttpResolver(client), modelConverter)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access requested <c>Tronald Dump</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpService(IStreamResolver resolver) : this(resolver, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with an underlaying <paramref name="resolver"/> and a <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access requested <c>Tronald Dump</c> resources.</param>
		/// <param name="modelConverter"><see cref="ITronaldDumpModelConverter"/> used to convert data received from the <c>Tronald Dump</c> API to usable objects.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpService(IStreamResolver resolver, ITronaldDumpModelConverter? modelConverter) : base(resolver)
		{
			ModelConverter = modelConverter ?? new TronaldDumpModelConverter();
		}

		/// <inheritdoc/>
		public Task<AuthorModel> GetAuthor(string id)
		{
			return Resolver.TryResolveAsync<AuthorModel>($"author/{id}").ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw Error.Quote($"Quote author with id '{id}' does not exist");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public Task<SearchResultModel<TagListModel>> GetAvailableTags()
		{
			return Resolver.ResolveAsync<SearchResultModel<TagListModel>>("tag");
		}

		/// <inheritdoc/>
		public Task<QuoteModel> GetQuote(string id)
		{
			return Resolver.TryResolveAsync<QuoteModel>($"quote/{id}").ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw Error.Quote($"Quote with id '{id}' does not exist");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public Task<Stream> GetRandomMeme()
		{
			return Resolver.ResolveStreamAsync("random/meme");
		}

		/// <inheritdoc/>
		public Task<QuoteModel> GetRandomQuote()
		{
			return Resolver.ResolveAsync<QuoteModel>("random/quote");
		}

		/// <inheritdoc/>
		public Task<QuoteSourceModel> GetSource(string id)
		{
			return Resolver.TryResolveAsync<QuoteSourceModel>($"quote-source/{id}").ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw Error.Quote($"Quote source with id '{id}' does not exist");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public Task<TagModel> GetTag(string tag)
		{
			return Resolver.TryResolveAsync<TagModel>($"tag/{tag}").ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw Error.Quote($"Unknown tag: '{tag}'");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public Task<SearchResultModel<QuoteListModel>> SearchQuotes(QuoteSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			string link = "search/quote";
			string query = ModelConverter.GetSearchQuery(searchModel);
			Internals.ApplyQuery(ref link, query);

			return Resolver.ResolveAsync<SearchResultModel<QuoteListModel>>(link);
		}
	}
}
