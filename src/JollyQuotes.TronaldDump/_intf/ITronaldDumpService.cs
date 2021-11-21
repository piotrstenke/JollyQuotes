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
		/// <exception cref="QuoteException">No information about quote author with <paramref name="id"/> was found.</exception>
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
		/// <exception cref="QuoteException">No information about quote with <paramref name="id"/> was found.</exception>
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
		/// <exception cref="QuoteException">No information about quote source with <paramref name="id"/> was found.</exception>
		Task<QuoteSourceModel> GetSource(string id);

		/// <summary>
		/// Returns information about a specific <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to get the information about.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">No information about <paramref name="tag"/> was found.</exception>
		Task<TagModel> GetTag(string tag);

		/// <summary>
		/// Searches for quotes that satisfy the conditions specified in the <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel">A set of condition a quote must satisfy in order to be included in the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		Task<SearchResultModel<QuoteListModel>> SearchQuotes(QuoteSearchModel searchModel);
	}
}
