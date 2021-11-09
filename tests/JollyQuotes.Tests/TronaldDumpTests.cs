using Xunit;
using JollyQuotes.TronaldDump;
using JollyQuotes.TronaldDump.Models;
using System.Threading.Tasks;
using SixLabors.ImageSharp;

using static JollyQuotes.Tests.Internals;

using System.Linq;
using System.IO;
using SixLabors.ImageSharp.Formats;
using System;

namespace JollyQuotes.Tests
{
	public class TronaldDumpTests
	{
		private readonly IStreamResolver _resolver;
		private readonly ITronaldDumpService _service;

		public TronaldDumpTests()
		{
			_resolver = GetResolver(TronaldDumpResources.APIPage);
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
			SearchResultModel<TagListModel> searchResult = await _resolver.ResolveAsync<SearchResultModel<TagListModel>>("tag");
			TagModel[] allTags = searchResult.Embedded.Tags;
			int randomIndex = RandomNumber.Next(0, allTags.Length);

			TagModel expected = allTags[randomIndex];
			TagModel actual = await _service.GetTag(expected.Value);

			Assert.Equal(expected, actual);
		}

		private Task<QuoteModel> GetRandomQuote()
		{
			return _resolver.ResolveAsync<QuoteModel>("random/quote");
		}
	}
}
