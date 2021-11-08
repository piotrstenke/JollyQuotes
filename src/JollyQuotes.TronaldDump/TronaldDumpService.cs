using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <inheritdoc cref="ITronaldDumpService"/>
	public class TronaldDumpService : ITronaldDumpService, IQuoteService
	{
		/// <summary>
		/// <see cref="IStreamResolver"/> that is used to access requested <c>Tronald Dump</c> resources.
		/// </summary>
		public IStreamResolver Resolver { get; }

		IResourceResolver IQuoteService.Resolver => Resolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> must be <see langword="null"/> or equal to <see cref="TronaldDumpResources.APIPage"/>.</exception>
		public TronaldDumpService(HttpClient client)
		{
			SetAddress(client);

			Resolver = new HttpResolver(client);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IStreamResolver"/> that is used to access requested <c>Tronald Dump</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public TronaldDumpService(IStreamResolver resolver)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			Resolver = resolver;
		}

		/// <inheritdoc/>
		public Task<AuthorModel> GetAuthor(string id)
		{
			return Resolver.TryResolveAsync<AuthorModel>($"author/{id}").ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw new QuoteException($"No information about quote author with id '{id}' was found");
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
					throw new QuoteException($"No information about quote with id '{id}' was found");
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
					throw new QuoteException($"No information about quote source with id '{id}' was found");
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
					throw new QuoteException($"No information about tag '{tag}' was found");
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

			StringBuilder builder = new();
			builder.Append("search/quote?");

			bool hasParam = false;

			if (searchModel.Query is not null)
			{
				hasParam = true;
				builder.Append("query=");
				builder.Append(searchModel.Query);
			}

			if (searchModel.Tag is not null)
			{
				EnsureParameter();
				builder.Append("tag=");
				builder.Append(searchModel.Tag);
			}

			if (searchModel.Page >= 0)
			{
				EnsureParameter();
				builder.Append("page=");
				builder.Append(searchModel.Page);
			}

			return Resolver.ResolveAsync<SearchResultModel<QuoteListModel>>(builder.ToString());

			void EnsureParameter()
			{
				if (hasParam)
				{
					builder.Append('&');
				}
				else
				{
					hasParam = true;
				}
			}
		}

		internal static HttpClient SetAddress(HttpClient client)
		{
			return Internals.SetAddress(client, TronaldDumpResources.BaseAddress, TronaldDumpResources.BaseAddressSource);
		}
	}
}
