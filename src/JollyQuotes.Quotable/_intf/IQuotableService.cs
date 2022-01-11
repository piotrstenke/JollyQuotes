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
		/// Returns a quote by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote to return.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote with the specified <paramref name="id"/> does not exist.</exception>
		Task<QuoteModel> GetQuote(string id);

		/// <summary>
		/// Returns a random quote.
		/// </summary>
		Task<QuoteModel> GetRandomQuote();

		/// <summary>
		/// Returns a random quote that fulfills all prerequisites specified in the given <paramref name="searchModel"/>.
		/// </summary>
		/// <param name="searchModel"><see cref="QuoteSearchModel"/> that defines all prerequisites that the returned quote must fulfill.</param>
		/// <exception cref="ArgumentNullException"><paramref name="searchModel"/> is <see langword="null"/>.</exception>
		Task<QuoteModel> GetRandomQuote(QuoteSearchModel searchModel);
	}
}
