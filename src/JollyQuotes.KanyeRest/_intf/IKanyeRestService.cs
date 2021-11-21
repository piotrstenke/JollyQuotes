using System.Collections.Generic;
using System.Threading.Tasks;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// Defines all actions available in the <c>kanye.rest</c> web API.
	/// </summary>
	public interface IKanyeRestService : IQuoteService
	{
		/// <summary>
		/// Returns all available <see cref="KanyeRestQuote"/>s.
		/// </summary>
		Task<List<KanyeRestQuote>> GetAllQuotes();

		/// <summary>
		/// Returns a random <see cref="KanyeRestQuote"/>.
		/// </summary>
		Task<KanyeRestQuote> GetRandomQuote();
	}
}
