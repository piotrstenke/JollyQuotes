using System.Collections.Generic;
using System.Threading.Tasks;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// Defines all actions available in the <c>kanye.rest</c> web API.
	/// </summary>
	public interface IKanyeRestService
	{
		/// <summary>
		/// Returns all available <see cref="KanyeQuote"/>s.
		/// </summary>
		Task<List<KanyeQuote>> GetAllQuotes();

		/// <summary>
		/// Returns a random <see cref="KanyeQuote"/>.
		/// </summary>
		Task<KanyeQuote> GetRandomQuote();
	}
}
