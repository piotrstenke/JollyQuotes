using System.Collections.Generic;
using System.Threading.Tasks;
using JollyQuotes.Quotable;
using JollyQuotes.Quotable.Models;
using Xunit;

using static JollyQuotes.Tests.TestHelpers;

namespace JollyQuotes.Tests
{
	public class QuotableTests
	{
		private readonly IResourceResolver _resolver;
		private readonly IQuotableService _service;

		public QuotableTests()
		{
			_resolver = GetResolver(QuotableResources.ApiPage);
			_service = new QuotableService(_resolver);
		}

		//[Fact]
		//public async Task Returns_Author_By_Slug()
		//{
		//	QuoteModel quote = await GetRandomQuote();

		//	AuthorModel expected = await _resolver.ResolveAsync<AuthorModel>($"author/{quote.AuthorSlug}");
		//	AuthorModel actual = await _service.GetAuthor(quote.AuthorSlug);

		//	Assert.Equal(expected, actual);
		//}

		[Fact]
		public async Task Returns_Author_By_Id()
		{
			QuoteModel quote = await GetRandomQuote();
			SearchResultModel<AuthorModel> author = await _resolver.ResolveAsync<SearchResultModel<AuthorModel>>($"authors?slug={quote.AuthorSlug}");

			string id = author.Results[0].Id;
			AuthorModel expected = await _resolver.ResolveAsync<AuthorModel>($"authors/{id}");
			AuthorModel actual = await _service.GetAuthor(id);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Authors()
		{
			SearchResultModel<AuthorModel> expected = await _resolver.ResolveAsync<SearchResultModel<AuthorModel>>("authors");
			SearchResultModel<AuthorModel> actual = await _service.GetAuthors();

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Authors_By_Search_Model()
		{
			AuthorSearchModel searchModel = new()
			{
				Limit = 4,
				Page = 3,
				Order = SortOrder.Ascending,
				SortBy = SortBy.QuoteCount
			};

			string query = $"authors?limit={searchModel.Limit}&page={searchModel.Page}&order={searchModel.Order.GetName()}&sortBy={searchModel.SortBy.GetName()}";

			SearchResultModel<AuthorModel> expected = await _resolver.ResolveAsync<SearchResultModel<AuthorModel>>(query);
			SearchResultModel<AuthorModel> actual = await _service.GetAuthors(searchModel);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Quote_By_Id()
		{
			QuoteModel quote = await GetRandomQuote();

			QuoteModel expected = await _resolver.ResolveAsync<QuoteModel>($"quotes/{quote.Id}");
			QuoteModel actual = await _service.GetQuote(quote.Id);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Quotes()
		{
			SearchResultModel<QuoteModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteModel>>("quotes");
			SearchResultModel<QuoteModel> actual = await _service.GetQuotes();

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Quotes_By_Search_Model()
		{
			QuoteModel quote = await GetRandomQuote();
			QuoteListSearchModel searchModel = new()
			{
				Tags = new(quote.Tags[0]),
				Author = quote.Author,
				Limit = 2,
				Order = SortOrder.Ascending,
				SortBy = QuoteSortBy.DateAddded,
				Page = 2,
				MinLength = 1
			};

			string query =
				$"quotes?tags={quote.Tags[0]}&author={searchModel.Author}&limit={searchModel.Limit}&" +
				$"page={searchModel.Page}&minLength={searchModel.MinLength}&" +
				$"order={searchModel.Order.GetName()}&sortBy={searchModel.SortBy.GetName()}";

			SearchResultModel<QuoteModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteModel>>(query);
			SearchResultModel<QuoteModel> actual = await _service.GetQuotes(searchModel);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Tags()
		{
			List<TagModel> expected = await _resolver.ResolveAsync<List<TagModel>>("tags");
 			List<TagModel> actual = await _service.GetTags();

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_All_Tags_By_Search_Model()
		{
			TagSearchModel searchModel = new()
			{
				Order = SortOrder.Ascending,
				SortBy = SortBy.QuoteCount
			};

			string query = $"tags?order={searchModel.Order.GetName()}&sortBy={searchModel.SortBy.GetName()}";

			List<TagModel> expected = await _resolver.ResolveAsync<List<TagModel>>(query);
			List<TagModel> actual = await _service.GetTags(searchModel);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Random_Quote()
		{
			QuoteModel actual = await _service.GetRandomQuote();
			QuoteModel expected = await _resolver.ResolveAsync<QuoteModel>($"quotes/{actual.Id}");

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Random_Quote_By_Search_Model()
		{
			QuoteModel quote = await GetRandomQuote();

			QuoteSearchModel searchModel = new()
			{
				Author = quote.Author,
				MinLength = 3,
				Tags = new(quote.Tags[0])
			};

			QuoteModel actual = await _service.GetRandomQuote(searchModel);
			QuoteModel expected = await _resolver.ResolveAsync<QuoteModel>($"quotes/{actual.Id}");

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Searches_Authors()
		{
			QuoteModel quote = await GetRandomQuote();
			string author = quote.Author[..3];

			AuthorNameSearchModel searchModel = new(author)
			{
				MatchTreshold = MatchThreshold.One,
				Limit = 2,
				Page = 2,
			};

			string query =
				$"search/authors?query={searchModel.Query}&limit={searchModel.Limit}&" +
				$"page={searchModel.Page}&matchThreshold={(int)searchModel.MatchTreshold}";

			SearchResultModel<AuthorModel> expected = await _resolver.ResolveAsync<SearchResultModel<AuthorModel>>(query);
			SearchResultModel<AuthorModel> actual = await _service.SearchAuthors(searchModel);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Searches_Quotes()
		{
			const string phrase = "looks like";

			QuoteContentSearchModel searchModel = new(phrase)
			{
				Fields = QuoteSearchFields.Content,
				Limit = 2,
				Page = 2,
				FuzzyMaxEdits = FuzzyMatchingTreshold.One,
				FuzzyMaxExpansions = 24
			};

			string query =
				$"search/quotes?query={searchModel.Query}&limit={searchModel.Limit}&page={searchModel.Page}&" +
				$"fields={searchModel.Fields.ToString().ToLower()}&fuzzyMaxEdits={(int)searchModel.FuzzyMaxEdits}&" +
				$"fuzzyMaxExpansions={searchModel.FuzzyMaxExpansions}";

			SearchResultModel<QuoteModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteModel>>(query);
			SearchResultModel<QuoteModel> actual = await _service.SearchQuotes(searchModel);

			Assert.Equal(expected, actual);
		}

		private Task<QuoteModel> GetRandomQuote()
		{
			return _resolver.ResolveAsync<QuoteModel>("random");
		}
	}
}
