using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static JollyQuotes.KanyeRest.Tests.Internals;

namespace JollyQuotes.KanyeRest.Tests
{
	public class ServiceTests
	{
		private readonly KanyeRestService _service;

		public ServiceTests()
		{
			_service = new KanyeRestService(HttpClient);
		}

		[Fact]
		public async Task Returns_All_Quotes()
		{
			List<KanyeQuote> quotes = await _service.GetAllQuotes();

			HttpResolver resolver = new(HttpClient);
			List<KanyeQuote> all = resolver.Resolve<List<string>>(KanyeResources.Database).ConvertAll(q => new KanyeQuote(q)).ToList();

			Assert.Equal(quotes, all);
		}

		[Fact]
		public async Task Returns_Random_Quote()
		{
			KanyeQuote quote = await _service.GetRandomQuote();

			Assert.NotNull(quote);
			Assert.False(string.IsNullOrWhiteSpace(quote.Quote));
		}
	}
}
