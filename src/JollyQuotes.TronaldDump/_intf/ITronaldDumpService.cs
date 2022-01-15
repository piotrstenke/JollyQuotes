using System;
using System.IO;
using System.Threading.Tasks;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Defines all actions available in the <c>Tronald Dump</c> web API.
	/// </summary>
	public interface ITronaldDumpService : IQuoteService
	{
		/// <summary>
		/// <see cref="IStreamResolver"/> that is used to access requested resources.
		/// </summary>
		new IStreamResolver Resolver { get; }

		/// <summary>
		/// Returns information about a quote author with the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote author to get information about.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote author with the specified <paramref name="id"/> does not exist.</exception>
		Task<AuthorModel> GetAuthor(string id);

		/// <summary>
		/// Returns a search result of all available tags.
		/// </summary>
		Task<SearchResultModel<TagListModel>> GetAvailableTags();

		/// <summary>
		/// Returns information about a quote with the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote to get information about.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote with the specified <paramref name="id"/> does not exist.</exception>
		Task<QuoteModel> GetQuote(string id);

		/// <summary>
		/// Returns byte data of an JPEG image with a random quote.
		/// </summary>
		Task<Stream> GetRandomMeme();

		/// <summary>
		/// Returns information about a random quote.
		/// </summary>
		Task<QuoteModel> GetRandomQuote();

		/// <summary>
		/// Returns information about a quote source with the specified <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote source to get information about.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote source with the specified <paramref name="id"/> does not exist.</exception>
		Task<QuoteSourceModel> GetSource(string id);

		/// <summary>
		/// Returns information about a specific <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to get the information about.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Unknown tag.</exception>
		Task<TagModel> GetTag(string tag);

		/// <summary>
		/// Searches for quotes that fulfill every prerequisite specified in the <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteSearchModel"/> that defines all prerequisites that all returned quotes must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		Task<SearchResultModel<QuoteListModel>> SearchQuotes(QuoteSearchModel searchModel);
	}
}
