using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JollyQuotes.Quotable.Models;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Defines all actions available in the <c>quotable</c> web API.
	/// </summary>
	public interface IQuotableService : IQuoteService
	{
		/// <summary>
		/// Returns an author by the associated <paramref name="slug"/>.
		/// </summary>
		/// <param name="slug">Slug to get the author associated with.</param>
		/// <exception cref="ArgumentException"><paramref name="slug"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Author with the specified <paramref name="slug"/> does not exist.</exception>
		Task<AuthorModel> GetAuthor(string slug);

		/// <summary>
		/// Returns an author by his <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of author to return.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Author with the specified <paramref name="id"/> does not exist.</exception>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use GetAuthor(string) instead.")]
		Task<AuthorModel> GetAuthorById(string id);

		/// <summary>
		/// Returns all existing authors.
		/// </summary>
		Task<SearchResultModel<AuthorModel>> GetAuthors();

		/// <summary>
		/// Returns all authors that fulfill every prerequisite specified in the given <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="AuthorSearchModel"/> that defines all prerequisites that all returned authors must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="QuoteException">Could not find any matching authors.</exception>
		Task<SearchResultModel<AuthorModel>> GetAuthors(AuthorSearchModel searchModel);

		/// <summary>
		/// Returns a quote by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote to return.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote with the specified <paramref name="id"/> does not exist.</exception>
		Task<QuoteModel> GetQuote(string id);

		/// <summary>
		/// Returns all existing quotes.
		/// </summary>
		Task<SearchResultModel<QuoteModel>> GetQuotes();

		/// <summary>
		/// Returns all quotes that fulfill every prerequisite specified in the given <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> that defines all prerequisites that all returned quotes must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="QuoteException">Could not find any matching quotes.</exception>
		Task<SearchResultModel<QuoteModel>> GetQuotes(QuoteListSearchModel searchModel);

		/// <summary>
		/// Returns a random quote.
		/// </summary>
		Task<QuoteModel> GetRandomQuote();

		/// <summary>
		/// Returns a random quote that fulfills every prerequisite specified in the given <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteSearchModel"/> that defines all prerequisites that the returned quote must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="QuoteException">Could not find any matching quotes.</exception>
		Task<QuoteModel> GetRandomQuote(QuoteSearchModel searchModel);

		/// <summary>
		/// Returns all existing tags.
		/// </summary>
		Task<TagModel> GetTags();

		/// <summary>
		/// Returns all tags that fulfill every prerequisite specified in the given <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> that defines all prerequisites that all returned tags must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		/// <exception cref="QuoteException">Could not find any matching tags.</exception>
		Task<List<TagModel>> GetTags(TagSearchModel searchModel);

		/// <summary>
		/// Searches for authors that fulfill every prerequisite specified in the <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> that defines all prerequisites that all returned authors must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		Task<SearchResultModel<AuthorModel>> SearchAuthors(AuthorNameSearchModel searchModel);

		/// <summary>
		/// Searches for quotes that fulfill every prerequisite specified in the <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> that defines all prerequisites that all returned quotes must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		Task<SearchResultModel<QuoteModel>> SearchQuotes(QuoteContentSearchModel searchModel);
	}
}
