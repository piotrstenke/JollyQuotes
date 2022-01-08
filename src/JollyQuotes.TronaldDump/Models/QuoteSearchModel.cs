using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Contains data needed to perform a quote search.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteSearchModel
	{
		/// <summary>
		/// A search query build from phrases separated by a plus '+'.
		/// </summary>
		[JsonProperty("query", Order = 0)]
		public string? Query { get; init; }

		/// <summary>
		/// Tag to search associated quotes with.
		/// </summary>
		[JsonProperty("tag", Order = 1)]
		public string? Tag { get; init; }

		/// <summary>
		/// The current page of the search result.
		/// </summary>
		/// <remarks>The lowest possible value is <c>0</c>.</remarks>
		[JsonProperty("page", Order = 2)]
		public int Page { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with query <paramref name="phrases"/> and associated <paramref name="tag"/> specified.
		/// </summary>
		/// <param name="phrases">An array of <see cref="string"/>s that will be joined with plus '+' sign to create a proper query.</param>
		/// <param name="tag">Tag to search associated quotes with.</param>
		/// <exception cref="ArgumentException">
		/// Either created query or <paramref name="tag"/> must be not <see langword="null"/>. -or-
		/// Created query must be either <see langword="null"/> or not empty. -or
		/// <paramref name="tag"/> must be either <see langword="null"/> or not empty.
		/// </exception>
		public QuoteSearchModel(string[]? phrases, string? tag) : this(JoinPhrases(phrases), tag, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with query <paramref name="phrases"/>, associated <paramref name="tag"/> and current <paramref name="page"/> specified.
		/// </summary>
		/// <param name="phrases">An array of <see cref="string"/>s that will be joined with plus '+' sign to create a proper query.</param>
		/// <param name="tag">Tag to search associated quotes with.</param>
		/// <param name="page">The current page of the search result.</param>
		/// <exception cref="ArgumentException">
		/// Either created query or <paramref name="tag"/> must be not <see langword="null"/>. -or-
		/// Created query must be either <see langword="null"/> or not empty. -or
		/// <paramref name="tag"/> must be either <see langword="null"/> or not empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="page"/> must be greater than or equal to<c>0</c>.</exception>
		public QuoteSearchModel(string[]? phrases, string? tag, int page) : this(JoinPhrases(phrases), tag, page)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with a <paramref name="query"/> and associated <paramref name="tag"/> specified.
		/// </summary>
		/// <param name="query">A search query build from phrases separated by a plus '+'.</param>
		/// <param name="tag">Tag to search associated quotes with.</param>
		/// <exception cref="ArgumentException">
		/// Either <paramref name="query"/> or <paramref name="tag"/> must be not <see langword="null"/>. -or-
		/// <paramref name="query"/> must be either <see langword="null"/> or not empty. -or
		/// <paramref name="tag"/> must be either <see langword="null"/> or not empty.
		/// </exception>
		public QuoteSearchModel(string? query, string? tag) : this(query, tag, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with a <paramref name="query"/>, associated <paramref name="tag"/> and current <paramref name="page"/> specified.
		/// </summary>
		/// <param name="query"> A search query build from phrases separated by a plus '+'.</param>
		/// <param name="tag">Tag to search associated quotes with.</param>
		/// <param name="page">The current page of the search result.</param>
		/// <exception cref="ArgumentException">
		/// Either <paramref name="query"/> or <paramref name="tag"/> must be not <see langword="null"/>. -or-
		/// <paramref name="query"/> must be either <see langword="null"/> or not empty. -or
		/// <paramref name="tag"/> must be either <see langword="null"/> or not empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="page"/> must be greater than or equal to <c>0</c>.</exception>
		[JsonConstructor]
		public QuoteSearchModel(string? query, string? tag, int page)
		{
			if (query is null && tag is null)
			{
				throw Error.Arg($"Either {nameof(query)} or {nameof(tag)} must be not null");
			}

			if (query is not null && string.IsNullOrWhiteSpace(query))
			{
				throw Exc_CannotBeEmpty(nameof(query));
			}

			if (tag is not null && string.IsNullOrWhiteSpace(tag))
			{
				throw Exc_CannotBeEmpty(nameof(tag));
			}

			if (page < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(page), 0);
			}

			Query = query;
			Tag = tag;
			Page = page;
		}

		private static ArgumentException Exc_CannotBeEmpty(string paramName)
		{
			return new ArgumentException($"{paramName} must be either null or not empty", paramName);
		}

		private static string? JoinPhrases(string[]? phrases)
		{
			if (phrases is null || phrases.Length == 0)
			{
				return null;
			}

			return string.Join('+', phrases);
		}
	}
}
