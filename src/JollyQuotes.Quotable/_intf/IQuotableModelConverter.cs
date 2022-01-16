using System;
using JollyQuotes.Quotable.Models;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Provides method for converting raw <c>quotable</c> models to more user-friendly objects.
	/// </summary>
	public interface IQuotableModelConverter
	{
		/// <summary>
		/// Converts the specified <paramref name="model"/> to a <see cref="QuotableQuote"/>.
		/// </summary>
		/// <param name="model"><see cref="QuoteModel"/> to convert.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
		QuotableQuote ConvertQuoteModel(QuoteModel model);

		/// <summary>
		/// Builds a quote fields query that includes the specified <paramref name="fields"/>.
		/// </summary>
		/// <param name="fields"><see cref="QuoteSearchFields"/> to include in the query.</param>
		string GetFieldQuery(QuoteSearchFields fields);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(QuoteSearchModel searchModel);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(QuoteListSearchModel searchModel);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="TagSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(TagSearchModel searchModel);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="AuthorSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(AuthorSearchModel searchModel);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="AuthorNameSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(AuthorNameSearchModel searchModel);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteContentSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(QuoteContentSearchModel searchModel);
	}
}
