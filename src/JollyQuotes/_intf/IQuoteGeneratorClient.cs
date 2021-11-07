using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <summary>
	/// Provides mechanism for generating quotes through an external web API accessed by a <see cref="HttpClient"/>.
	/// </summary>
	public interface IQuoteGeneratorClient : IRandomQuoteGenerator
	{
		/// <summary>
		/// Underlaying client that is used to access required resources.
		/// </summary>
		HttpClient BaseClient { get; }
	}
}
