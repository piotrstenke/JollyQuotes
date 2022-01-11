using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JollyQuotes.TronaldDump;
using JollyQuotes.TronaldDump.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using Xunit;
using static JollyQuotes.Tests.TestHelpers;

namespace JollyQuotes.Tests
{
	public class TronaldDumpTests
	{
		private readonly IStreamResolver _resolver;
		private readonly ITronaldDumpService _service;

		public TronaldDumpTests()
		{
			_resolver = GetResolver(TronaldDumpResources.ApiPage);
			_service = new TronaldDumpService(_resolver);
		}

		[Fact]
		public async Task Returns_All_Tags()
		{
			SearchResultModel<TagListModel> expected = await _resolver.ResolveAsync<SearchResultModel<TagListModel>>("tag");
			SearchResultModel<TagListModel> actual = await _service.GetAvailableTags();

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Author_By_Id()
		{
			QuoteModel randomQuote = await GetRandomQuote();

			AuthorModel expected = randomQuote.Embedded.Authors.First();
			AuthorModel actual = await _service.GetAuthor(expected.Id);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Quote_By_Id()
		{
			QuoteModel expected = await GetRandomQuote();
			QuoteModel actual = await _service.GetQuote(expected.Id);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Quote_Source_By_Id()
		{
			QuoteModel randomQuote = await GetRandomQuote();

			QuoteSourceModel expected = randomQuote.Embedded.Sources[0];
			QuoteSourceModel actual = await _service.GetSource(expected.Id);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Random_Meme()
		{
			Stream bytes = await _service.GetRandomMeme();

			Image image = Image.Load(bytes, out IImageFormat format);

			Assert.True(image.Height == TronaldDumpResources.MemeHeight);
			Assert.True(image.Width == TronaldDumpResources.MemeWidth);
			Assert.True(format.Name.Equals(TronaldDumpResources.MemeFormat, StringComparison.OrdinalIgnoreCase));
		}

		[Fact]
		public async Task Returns_Random_Quote()
		{
			QuoteModel actual = await _service.GetRandomQuote();
			QuoteModel expected = await _resolver.ResolveAsync<QuoteModel>($"quote/{actual.Id}");

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Returns_Tag_By_Value()
		{
			TagModel expected = await GetRandomTag();
			TagModel actual = await _service.GetTag(expected.Value);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_By_Query_With_Multiple_Phrases()
		{
			string[] phrases = new string[] { "Crooked", "Hillary", "is" };

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?query={string.Join('+', phrases)}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(phrases, null));

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_By_Query_With_Single_Phrase()
		{
			const string phrase = "when";

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?query={phrase}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(new string[] { phrase }, null));

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_By_Tag()
		{
			TagModel randomTag = await GetRandomTag();

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?tag={randomTag.Value}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(null, randomTag.Value));

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_By_Tag_And_Multiple_Phrases()
		{
			string[] phrases = new string[] { "Crooked", "Hillary", "is" };

			TagModel randomTag = await GetRandomTag();

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?tag={randomTag.Value}&query={string.Join('+', phrases)}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(phrases, randomTag.Value));

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_By_Tag_And_Single_Phrase()
		{
			const string phrase = "when";

			TagModel randomTag = await GetRandomTag();

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?tag={randomTag.Value}&query={phrase}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(new string[] { phrase }, randomTag.Value));

			Assert.Equal(expected, actual);
		}

		[Fact]
		public async Task Search_Quote_With_Next_Page()
		{
			const string phrase = "when";
			const int page = 1;

			SearchResultModel<QuoteListModel> expected = await _resolver.ResolveAsync<SearchResultModel<QuoteListModel>>($"search/quote?query={phrase}&page={page}");

			SearchResultModel<QuoteListModel> actual = await _service.SearchQuotes(new(new string[] { phrase }, null, page));

			Assert.Equal(expected, actual);
		}

		private Task<QuoteModel> GetRandomQuote()
		{
			return _resolver.ResolveAsync<QuoteModel>("random/quote");
		}

		private async Task<TagModel> GetRandomTag()
		{
			SearchResultModel<TagListModel> searchResult = await _resolver.ResolveAsync<SearchResultModel<TagListModel>>("tag");
			TagModel[] allTags = searchResult.Embedded.Tags;
			int randomIndex = ThreadRandom.Random.Next(0, allTags.Length);

			return allTags[randomIndex];
		}
	}
}
