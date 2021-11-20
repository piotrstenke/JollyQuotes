using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace JollyQuotes.Tests
{
	public class CacheTests
	{
		private sealed class AuthorOnlyEqualityComparer : IEqualityComparer<Quote>
		{
			public bool Equals(Quote? x, Quote? y)
			{
				return x!.Author == y!.Author;
			}

			public int GetHashCode([DisallowNull] Quote obj)
			{
				return obj.Author.GetHashCode();
			}
		}

		private const string _tag1 = "Test";
		private const string _tag2 = "Other";
		private const string _tag3 = "X";

		private readonly QuoteCache<Quote> _cache;

		public CacheTests()
		{
			_cache = new();
		}

		[Fact]
		public void CacheQuote_Accepts_Quote()
		{
			Assert.True(_cache.CacheQuote(Quote.Unknown));
			Assert.True(_cache.Count == 1);
		}

		[Fact]
		public void CacheQuote_Accepts_Quotes_With_Different_Ids()
		{
			Quote first = Quote.Unknown.WithId(1);
			Quote second = first.WithId(2);

			Assert.True(_cache.CacheQuote(first));
			Assert.True(_cache.CacheQuote(second));

			Assert.True(_cache.Count == 2);
		}

		[Fact]
		public void CacheQuote_Replaces_Quote_With_Same_Id_When_Replace_Is_True()
		{
			Quote first = Quote.Unknown.WithId(1);
			Quote second = first with { Author = "Donald" };

			Assert.True(_cache.CacheQuote(first));
			Assert.False(_cache.CacheQuote(second));

			Assert.True(_cache.CacheQuote(second, true));

			Assert.True(_cache.Count == 1);
		}

		[Fact]
		public void CacheQuote_Returns_False_When_Tried_To_Cache_Quotes_With_Same_Ids()
		{
			Quote first = Quote.Unknown.WithId(1);
			Quote second = first with { Author = "Donald" };

			Assert.True(_cache.CacheQuote(first));
			Assert.False(_cache.CacheQuote(second));
		}

		[Fact]
		public void CacheQuote_Returns_False_When_Tried_To_Cache_Same_Instance_Again()
		{
			Quote quote = Quote.Unknown;

			Assert.True(_cache.CacheQuote(quote));
			Assert.False(_cache.CacheQuote(quote));
		}

		[Fact]
		public void CacheQuote_Throws_When_Quote_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => _cache.CacheQuote(null!));
		}

		[Fact]
		public void CacheQuotes_Accepts_All_Quotes_In_Collection()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.True(_cache.Count == quotes.Length);
			Assert.Contains(quotes, q => _cache.IsCached(q.Id));
		}

		[Fact]
		public void CacheQuotes_Replaces_Quotes_With_Same_Ids_When_Replace_Is_True()
		{
			const int id = 100;

			QuoteWithId[] quotes = GetExampleQuotes();
			quotes[1] = quotes[1].WithId(id);

			Quote old = Quote.Unknown.WithId(id);

			Assert.True(_cache.CacheQuote(old));

			_cache.CacheQuotes(quotes, true);

			Assert.True(_cache.Count == quotes.Length);
			Assert.Contains(quotes, q => _cache.IsCached(q.Id));

			Quote current = _cache.GetQuote(id);

			Assert.False(current.Equals(old));
		}

		[Fact]
		public void CacheQuotes_Skips_Empty_Collection()
		{
			_cache.CacheQuotes(Array.Empty<Quote>());

			Assert.True(_cache.IsEmpty);
		}

		[Fact]
		public void CacheQuotes_Skips_Existing_Quote_In_Collection()
		{
			QuoteWithId[] quotes = GetExampleQuotes();
			quotes[1] = quotes[2];

			_cache.CacheQuotes(quotes);

			Assert.True(_cache.Count == quotes.Length - 1);
			Assert.Contains(quotes, q => _cache.IsCached(q.Id));
		}

		[Fact]
		public void CacheQuotes_Skips_Null_Quotes_In_Collection()
		{
			QuoteWithId[] quotes = GetExampleQuotes();
			Quote replaced = quotes[1];
			quotes[1] = null!;

			_cache.CacheQuotes(quotes);

			Assert.True(_cache.Count == quotes.Length - 1);
			Assert.True(_cache.IsCached(quotes[0]));
			Assert.True(_cache.IsCached(quotes[2]));
			Assert.Contains(quotes[3..], q => _cache.IsCached(q.Id));
			Assert.False(_cache.IsCached(replaced));
		}

		[Fact]
		public void CacheQuotes_Skips_Same_Id_In_Collection()
		{
			QuoteWithId[] quotes = GetExampleQuotes();
			quotes[1] = quotes[1].WithId(2);
			quotes[2] = quotes[2].WithId(2);

			_cache.CacheQuotes(quotes);

			Assert.True(_cache.Count == quotes.Length - 1);
			Assert.Contains(quotes, q => _cache.IsCached(q.Id));
		}

		[Fact]
		public void CacheQuotes_Throws_When_Quote_Collection_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => _cache.CacheQuotes(null!));
		}

		[Fact]
		public void Clear_Removes_All_Quotes()
		{
			_cache.CacheQuote(Quote.Unknown);

			Assert.True(_cache.Count == 1);

			_cache.Clear();

			Assert.True(_cache.Count == 0);
		}

		[Fact]
		public void GetCached_Returns_All_Quotes()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Quote[] cached = _cache.GetCached().ToArray();

			Assert.True(quotes.Length == cached.Length);
			Assert.Contains(quotes, q => Array.IndexOf(cached, q) > -1);
		}

		[Fact]
		public void GetCached_Returns_All_Quotes_With_Any_Of_Specific_Tags()
		{
			Quote[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Quote[] expected = quotes.Where(q => Array.IndexOf(q.Tags, _tag1) > -1 || Array.IndexOf(q.Tags, _tag2) > -1).ToArray();
			Quote[] cached = _cache.GetCached(_tag1, _tag2).ToArray();

			Assert.True(expected.Length == cached.Length);
			Assert.Contains(expected, q => Array.IndexOf(cached, q) > -1);
		}

		[Fact]
		public void GetCached_Returns_All_Quotes_With_Specific_Tag()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Quote[] expected = quotes.Where(q => Array.IndexOf(q.Tags, _tag1) > -1).ToArray();
			Quote[] cached = _cache.GetCached(_tag1).ToArray();

			Assert.True(cached.Length == expected.Length);
			Assert.Contains(expected, q => Array.IndexOf(cached, q) > -1);
		}

		[Fact]
		public void GetCached_Returns_Empty_Collection_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.Empty(_cache.GetCached());
			Assert.Empty(_cache.GetCached(_tag1));
			Assert.Empty(_cache.GetCached(_tag1, _tag2));
		}

		[Fact]
		public void GetCached_Returns_Empty_Collection_When_Passed_Empty_Tag_Collection()
		{
			Quote[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.Empty(_cache.GetCached(tags: null));
			Assert.Empty(_cache.GetCached(Array.Empty<string>()));
		}

		[Fact]
		public void GetCached_Returns_Empty_Collection_When_Tag_Is_Unknown()
		{
			Quote[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.Empty(_cache.GetCached(_tag3));
		}

		[Fact]
		public void GetCached_Skips_Null_Or_Empty_Tag_In_Collection()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			string[] tags = { _tag1, null!, "   ", _tag2, string.Empty };

			Quote[] expected = quotes.Where(q => Array.IndexOf(q.Tags, _tag1) > -1 || Array.IndexOf(q.Tags, _tag2) > -1).ToArray();
			Quote[] cached = _cache.GetCached(tags).ToArray();

			Assert.Equal(expected, cached);
		}

		[Fact]
		public void GetCached_Throws_When_Tag_Is_Null_Or_Empty()
		{
			Assert.Throws<ArgumentException>(() => _cache.GetCached(tag: null!));
			Assert.Throws<ArgumentException>(() => _cache.GetCached(string.Empty));
			Assert.Throws<ArgumentException>(() => _cache.GetCached("     "));
		}

		[Fact]
		public void GetEnumerator_Uses_Same_Collection_As_GetCached()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Quote[] cached = _cache.GetCached().ToArray();
			Quote[] fromIterator = Yield().ToArray();

			Assert.Equal(cached, fromIterator);

			IEnumerable<Quote> Yield()
			{
				IEnumerator<Quote> it = _cache.GetEnumerator();

				while (it.MoveNext())
				{
					yield return it.Current;
				}
			}
		}

		[Fact]
		public void GetQuote_Returns_Quote()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.NotNull(_cache.GetQuote(1));
		}

		[Fact]
		public void GetQuote_Throws_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.Throws<InvalidOperationException>(() => _cache.GetQuote(0));
		}

		[Fact]
		public void GetQuote_Throws_When_Id_Is_Unknown()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.Throws<ArgumentException>(() => _cache.GetQuote(2));
		}

		[Fact]
		public void GetRandomQuote_Removes_Quote_When_Remove_Is_True()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			QuoteWithId random = (QuoteWithId)_cache.GetRandomQuote(true);

			Assert.True(_cache.Count == quotes.Length - 1);
			Assert.False(_cache.IsCached(random.Id));
		}

		[Fact]
		public void GetRandomQuote_Returns_Random_Quote()
		{
			const int maxTries = 1000;
			const int maxUniqueQuotes = 3;

			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			HashSet<Quote> set = new();
			set.Add(_cache.GetRandomQuote());

			for (int i = 0; i < maxTries; i++)
			{
				Quote random = _cache.GetRandomQuote();

				if (set.Add(random) && set.Count >= maxUniqueQuotes)
				{
					break;
				}
			}

			Assert.True(set.Count >= maxUniqueQuotes);
		}

		[Fact]
		public void GetRandomQuote_Returns_Same_Quote_When_Count_Is_1()
		{
			const int numTries = 50;

			Quote quote = Quote.Unknown;

			_cache.CacheQuote(quote);

			Assert.True(_cache.Count == 1);

			bool isAlwaysSame = true;

			for (int i = 0; i < numTries; i++)
			{
				Quote random = _cache.GetRandomQuote();

				if (random != quote)
				{
					isAlwaysSame = false;
					break;
				}
			}

			Assert.True(isAlwaysSame);
		}

		[Fact]
		public void GetRandomQuote_Throws_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.Throws<InvalidOperationException>(() => _cache.GetRandomQuote());
		}

		[Fact]
		public void IsCached_Returns_False_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.False(_cache.IsCached(2));
			Assert.False(_cache.IsCached(Quote.Unknown));
		}

		[Fact]
		public void IsCached_Returns_False_When_Id_Is_Unknown()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.False(_cache.IsCached(2));
		}

		[Fact]
		public void IsCached_Returns_False_When_Objects_Are_Equal_But_Id_Is_Unknown()
		{
			QuoteCache<Quote> cache = new(new AuthorOnlyEqualityComparer());

			QuoteWithId quote = Quote.Unknown.WithId(1);
			QuoteWithId other = quote with { Id = 2 };

			cache.CacheQuote(quote);

			Assert.True(cache.IsCached(quote));
			Assert.False(cache.IsCached(other.Id));
			Assert.False(cache.IsCached(other));
		}

		[Fact]
		public void IsCached_Returns_False_When_Objects_Have_Same_Id_But_Are_Not_Equal()
		{
			QuoteWithId quote = Quote.Unknown.WithId(1);
			QuoteWithId other = quote with { Author = "Donald" };

			_cache.CacheQuote(quote);

			Assert.True(_cache.IsCached(quote));
			Assert.True(_cache.IsCached(other.Id));
			Assert.False(_cache.IsCached(other));
		}

		[Fact]
		public void IsCached_Returns_True_When_Id_Is_Found()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.True(_cache.IsCached(1));
		}

		[Fact]
		public void IsCached_Returns_True_When_Objects_Are_Equal_And_Have_Same_Id()
		{
			QuoteWithId quote = Quote.Unknown.WithId(1);

			_cache.CacheQuote(quote);

			Assert.True(_cache.IsCached(quote.Id));
			Assert.True(_cache.IsCached(quote));
		}

		[Fact]
		public void IsCached_Throws_When_Quote_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => _cache.IsCached(null!));
		}

		[Fact]
		public void IsEmpty_Returns_False_When_Count_Is_Greater_Than_0()
		{
			_cache.CacheQuote(Quote.Unknown);

			Assert.True(_cache.Count > 0);
			Assert.False(_cache.IsEmpty);
		}

		[Fact]
		public void IsEmpty_Returns_True_When_Count_Is_0()
		{
			Assert.True(_cache.Count == 0);
			Assert.True(_cache.IsEmpty);
		}

		[Fact]
		public void RemoveQuote_Removes_Quote_By_Id()
		{
			const int id = 1;

			_cache.CacheQuote(Quote.Unknown.WithId(id));

			Assert.False(_cache.IsEmpty);
			Assert.True(_cache.RemoveQuote(id));
			Assert.True(_cache.IsEmpty);

			_cache.CacheQuote(Quote.Unknown.WithId(id));

			Assert.False(_cache.IsEmpty);
			Assert.True(_cache.RemoveQuote(id, out Quote? quote));
			Assert.NotNull(quote);
			Assert.True(_cache.IsEmpty);
		}

		[Fact]
		public void RemoveQuote_Returns_False_When_Cache_Is_Empty()
		{
			const int id = 1;

			Assert.True(_cache.IsEmpty);
			Assert.False(_cache.RemoveQuote(id));
			Assert.False(_cache.RemoveQuote(id, out Quote? quote));
			Assert.Null(quote);
			Assert.True(_cache.IsEmpty);
		}

		[Fact]
		public void RemoveQuote_Returns_False_When_Id_Is_Unknown()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.False(_cache.IsEmpty);
			Assert.False(_cache.RemoveQuote(2));
			Assert.False(_cache.RemoveQuote(2, out Quote? quote));
			Assert.Null(quote);
			Assert.False(_cache.IsEmpty);
		}

		[Fact]
		public void RemoveQuote_Returns_False_When_Objects_Are_Equal_But_Id_Is_Unknown()
		{
			QuoteCache<Quote> cache = new(new AuthorOnlyEqualityComparer());

			QuoteWithId quote = Quote.Unknown.WithId(1);

			cache.CacheQuote(quote);

			Assert.False(cache.RemoveQuote(quote with { Id = 2 }));
		}

		[Fact]
		public void RemoveQuote_Returns_False_When_Objects_Have_Same_Id_But_Are_Not_Equal()
		{
			QuoteWithId quote = Quote.Unknown.WithId(1);
			QuoteWithId other = quote with { Author = "Donald" };

			_cache.CacheQuote(quote);

			Assert.False(_cache.RemoveQuote(other));
		}

		[Fact]
		public void RemoveQuote_Returns_True_When_Objects_Are_Equal_And_Have_Same_Id()
		{
			QuoteWithId quote = Quote.Unknown.WithId(1);

			_cache.CacheQuote(quote);

			Assert.True(_cache.RemoveQuote(quote));
		}

		[Fact]
		public void RemoveQuote_Throws_When_Quote_Is_Null()
		{
			Assert.Throws<ArgumentNullException>(() => _cache.RemoveQuote(null!));
		}

		[Fact]
		public void RemoveQuotes_Removes_All_Quotes_With_Specific_Tag()
		{
			Quote[] quotes = GetExampleQuotes();

			Quote[] expected = quotes.Where(q => Array.IndexOf(q.Tags, _tag1) > -1).ToArray();

			_cache.CacheQuotes(quotes);

			Assert.NotEmpty(expected);
			Assert.False(_cache.IsEmpty);
			Assert.True(_cache.RemoveQuotes(_tag1, out Quote[]? removed));
			Assert.Equal(expected, removed);
		}

		[Fact]
		public void RemoveQuotes_Returns_False_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.False(_cache.RemoveQuotes(_tag1));
			Assert.False(_cache.RemoveQuotes(_tag1, out _));
		}

		[Fact]
		public void RemoveQuotes_Returns_False_When_Tag_Is_Unknown()
		{
			Quote[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.False(_cache.IsEmpty);
			Assert.False(_cache.RemoveQuotes(_tag3));
			Assert.True(_cache.Count == quotes.Length);

			Assert.False(_cache.RemoveQuotes(_tag3, out Quote[]? removed));
			Assert.Null(removed);
			Assert.True(_cache.Count == quotes.Length);
		}

		[Fact]
		public void RemoveQuotes_Throws_When_Tag_Is_Null_Or_Empty()
		{
			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes(null!));
			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes(string.Empty));
			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes("   "));

			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes(null!, out _));
			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes(string.Empty, out _));
			Assert.Throws<ArgumentException>(() => _cache.RemoveQuotes("   ", out _));
		}

		[Fact]
		public void TryGetQuote_Returns_False_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.False(_cache.TryGetQuote(1, out Quote? quote));
			Assert.Null(quote);
		}

		[Fact]
		public void TryGetQuote_Returns_False_When_Id_Is_Unknown()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.False(_cache.TryGetQuote(2, out Quote? quote));
			Assert.Null(quote);
		}

		[Fact]
		public void TryGetQuote_Returns_True_When_Quote_Is_Found()
		{
			_cache.CacheQuote(Quote.Unknown.WithId(1));

			Assert.True(_cache.TryGetQuote(1, out Quote? quote));
			Assert.NotNull(quote);
		}

		[Fact]
		public void TryGetRandomQuote_Removes_Quote_When_Remove_Is_True()
		{
			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.True(_cache.TryGetRandomQuote(_tag1, out Quote? quote, true));
			Assert.NotNull(quote);
			Assert.True(_cache.Count == quotes.Length - 1);
			Assert.False(_cache.IsCached(((QuoteWithId)quote!).Id));
		}

		[Fact]
		public void TryGetRandomQuote_Returns_False_When_Cache_Is_Empty()
		{
			Assert.True(_cache.IsEmpty);
			Assert.False(_cache.TryGetRandomQuote(_tag1, out Quote? quote));
			Assert.Null(quote);
		}

		[Fact]
		public void TryGetRandomQuote_Returns_False_When_Tag_Is_Unknown()
		{
			Quote[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			Assert.False(_cache.IsEmpty);
			Assert.False(_cache.TryGetRandomQuote(_tag3, out Quote? quote));
			Assert.Null(quote);
		}

		[Fact]
		public void TryGetRandomQuote_Returns_Random_Quote()
		{
			const int maxTries = 1000;
			const int maxUniqueQuotes = 3;

			QuoteWithId[] quotes = GetExampleQuotes();

			_cache.CacheQuotes(quotes);

			HashSet<Quote> set = new();

			if (_cache.TryGetRandomQuote(_tag1, out Quote? quote))
			{
				set.Add(quote);

				for (int i = 0; i < maxTries; i++)
				{
					if (!_cache.TryGetRandomQuote(_tag1, out quote) || (set.Add(quote) && set.Count >= maxUniqueQuotes))
					{
						break;
					}
				}
			}

			Assert.True(set.Count >= maxUniqueQuotes);
		}

		[Fact]
		public void TryGetRandomQuote_Returns_Same_Quote_When_Count_Is_1()
		{
			const int numTries = 50;

			Quote quote = Quote.Unknown with { Tags = new string[] { _tag1 } };

			_cache.CacheQuote(quote);

			Assert.True(_cache.Count == 1);

			bool isAlwaysSame = true;

			for (int i = 0; i < numTries; i++)
			{
				if (!_cache.TryGetRandomQuote(_tag1, out Quote? random) || random != quote)
				{
					isAlwaysSame = false;
					break;
				}
			}

			Assert.True(isAlwaysSame);
		}

		[Fact]
		public void TryGetRandomQuote_Throws_When_Tag_Is_Null_Or_Empty()
		{
			Assert.Throws<ArgumentException>(() => _cache.TryGetRandomQuote(null!, out _));
			Assert.Throws<ArgumentException>(() => _cache.TryGetRandomQuote(string.Empty, out _));
			Assert.Throws<ArgumentException>(() => _cache.TryGetRandomQuote("   ", out _));
		}

		private static QuoteWithId[] GetExampleQuotes()
		{
			return new QuoteWithId[]
			{
				new QuoteWithId(1, "ABC", "Donald", string.Empty, default, _tag1, _tag2),
				new QuoteWithId(2, "123", "Donald", string.Empty, default, _tag1),
				new QuoteWithId(3, "zaq1", "Donald", string.Empty, default, _tag2),
				new QuoteWithId(4, "qwerty", "Donald", string.Empty, default),
				new QuoteWithId(5, "xsw2", "Donald", string.Empty, default, _tag1)
			};
		}
	}
}
