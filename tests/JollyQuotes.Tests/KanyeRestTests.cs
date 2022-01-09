using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JollyQuotes.KanyeRest;
using Xunit;
using static JollyQuotes.Tests.TestHelpers;

namespace JollyQuotes.Tests
{
	public class KanyeRestTests
	{
		private readonly IKanyeRestService _service;

		public KanyeRestTests()
		{
			_service = new KanyeRestService(HttpResolver.CreateDefault());
		}

		[Fact]
		public async Task Returns_All_Quotes()
		{
			List<KanyeRestQuote> quotes = await _service.GetAllQuotes();

			List<KanyeRestQuote> all = (await GlobalResolver
				.ResolveAsync<List<string>>(KanyeRestResources.Database))
				.ConvertAll(q => new KanyeRestQuote(q))
				.ToList();

			Assert.Equal(quotes, all);
		}

		[Fact]
		public async Task Returns_Random_Quote()
		{
			KanyeRestQuote quote = await _service.GetRandomQuote();

			Assert.NotNull(quote);
			Assert.False(string.IsNullOrWhiteSpace(quote.Value));
		}
	}
}
