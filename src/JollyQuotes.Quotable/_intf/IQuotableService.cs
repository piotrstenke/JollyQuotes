using System.Threading.Tasks;
using JollyQuotes.Quotable.Models;
using System;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Defines all actions available in the <c>quotable</c> web API.
	/// </summary>
	public interface IQuotableService : IQuoteService
	{
		/// <summary>
		/// Returns information about a random quote.
		/// </summary>
		public Task<QuoteModel> GetRandomQuote();

		/// <summary>
		/// Returns a quote by its <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Id of quote to return.</param>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="QuoteException">Quote with the specified <paramref name="id"/> does not exist.</exception>
		public Task<QuoteModel> GetQuote(string id);
	}
}
