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
		/// Returns a <see cref="string"/> representation of the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		string GetName(SortBy value);

		/// <summary>
		/// Returns a <see cref="string"/> representation of the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		string GetName(QuoteSortBy value);

		/// <summary>
		/// Returns a <see cref="string"/> representation of the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		string GetName(SortOrder value);

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
		/// <param name="searchModel"><see cref="QuoteListSearchModel"/> to build the query from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		string GetSearchQuery(TagSearchModel searchModel);

		/// <summary>
		/// Builds a tag query from the specified <paramref name="expression"/>.
		/// </summary>
		/// <param name="expression"><see cref="TagExpression"/> model to create the actual tag expression from.</param>
		/// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
		string GetTagQuery(TagExpression expression);
	}
}
