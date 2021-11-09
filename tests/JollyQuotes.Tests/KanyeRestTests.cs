using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using JollyQuotes.KanyeRest;

using static JollyQuotes.Tests.Internals;

namespace JollyQuotes.Tests
{
	public class KanyeRestTests
	{
		private readonly IKanyeRestService _service;

		public KanyeRestTests()
		{
			_service = new KanyeRestService(GlobalClient);
		}

		[Fact]
		public async Task Returns_All_Quotes()
		{
			List<KanyeQuote> quotes = await _service.GetAllQuotes();

			List<KanyeQuote> all = (await GlobalResolver
				.ResolveAsync<List<string>>(KanyeResources.Database))
				.ConvertAll(q => new KanyeQuote(q))
				.ToList();

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
