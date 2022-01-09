using System;
using System.Collections.Generic;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Provides method for converting <c>Tronald Dump</c> models to more user-friendly classes.
	/// </summary>
	public interface ITronaldDumpModelConverter
	{
		/// <summary>
		/// Converts the specified <paramref name="model"/> to a <see cref="TronaldDumpQuote"/>.
		/// </summary>
		/// <param name="model"><see cref="QuoteModel"/> to convert.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
		TronaldDumpQuote ConvertQuoteModel(QuoteModel model);

		/// <summary>
		/// Returns the number of pages in the search result.
		/// </summary>
		/// <param name="model"><see cref="ISearchResultModel"/> to count the number of pages of.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
		int CountPages(ISearchResultModel model);

		/// <summary>
		/// Converts the specified <paramref name="model"/> to an <see cref="IEnumerable{T}"/> of <see cref="TronaldDumpQuote"/>s.
		/// </summary>
		/// <param name="model"><see cref="QuoteListModel"/> to convert.</param>
		/// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
		IEnumerable<TronaldDumpQuote> EnumerateQuotes(QuoteListModel model);

		/// <summary>
		/// Builds a search query from the specified <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(QuoteSearchModel searchModel);
	}
}
