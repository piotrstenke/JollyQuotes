using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform an author search by his name or at least some part of it.
	/// </summary>
	[JsonObject]
	public sealed record AuthorNameSearchModel
	{
		private readonly string _query;
		private readonly MatchThreshold _matchTreshold;
		private readonly int _limit;
		private readonly int _page;

		/// <summary>
		/// Query containing at least some part an author's name.
		/// </summary>
		/// <remarks>If <see cref="AutoComplete"/> is <see langword="false"/>, the query has to contain full name of an author.</remarks>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("query", Order = 0, Required = Required.Always)]
		public string Query
		{
			get => _query;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_query = value;
			}
		}

		/// <summary>
		/// Determines whether to use auto-complete when sending the request.
		/// </summary>
		/// <remarks>If <see langword="false"/>, the <see cref="Query"/> has to contain full name of an author.
		/// <para>The default value is <see langword="true"/>.</para></remarks>
		[JsonProperty("autocomplete", Order = 1)]
		[DefaultValue(true)]
		public bool AutoComplete { get; init; }

		/// <summary>
		/// Minimum number of search terms (words) that must match for an author to be included in results.
		/// </summary>
		/// <remarks>The default value is <see cref="MatchThreshold.Default"/></remarks>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("matchTreshold", Order = 2)]
		[DefaultValue(MatchThreshold.Default)]
		public MatchThreshold MatchTreshold
		{
			get => _matchTreshold;
			init
			{
				if (!value.IsValid())
				{
					throw Error.InvalidEnumValue(value);
				}

				_matchTreshold = value;
			}
		}

		/// <summary>
		/// Maximum number of results on a single page.
		/// </summary>
		/// <remarks>The default value is equal to <see cref="QuotableResources.ResultsPerPageDefault"/>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.</exception>
		[JsonProperty("limit", Order = 3)]
		[DefaultValue(QuotableResources.ResultsPerPageDefault)]
		public int Limit
		{
			get => _limit;
			init
			{
				if (value <= 0)
				{
					throw Error.MustBeGreaterThan(nameof(value), 0);
				}

				if (value > QuotableResources.ResultsPerPageMax)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), QuotableResources.MAX_RESULTS_PER_PAGE_NAME);
				}

				_limit = value;
			}
		}

		/// <summary>
		/// Current page number.
		/// </summary>
		/// <remarks>The default value is <c>1</c>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>.</exception>
		[JsonProperty("page", Order = 4)]
		[DefaultValue(1)]
		public int Page
		{
			get => _page;
			init
			{
				if (value <= 0)
				{
					throw Error.MustBeGreaterThan(nameof(value), 0);
				}

				_page = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorNameSearchModel"/> class with a <paramref name="query"/> specified.
		/// </summary>
		/// <param name="query">Query containing at least some part an author's name.</param>
		/// <exception cref="ArgumentException"><paramref name="query"/> is <see langword="null"/> or empty. -or-</exception>
		public AuthorNameSearchModel(string query) : this(query, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorNameSearchModel"/> class with a <paramref name="query"/>, <paramref name="autoComplete"/>, <paramref name="matchTreshold"/>, current <paramref name="page"/> and <paramref name="limit"/> specified.
		/// </summary>
		/// <param name="query">Query containing at least some part an author's name.</param>
		/// <param name="autoComplete">Determines whether to use auto-complete when sending the request.</param>
		/// <param name="matchTreshold">Minimum number of search terms (words) that must match for an author to be included in results.</param>
		/// <param name="page">Current page number.</param>
		/// <param name="limit">Maximum number of results on a single page.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="query"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="matchTreshold"/> is not a valid enum value.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.
		/// </exception>
		[JsonConstructor]
		public AuthorNameSearchModel(
			string query,
			bool autoComplete = true,
			MatchThreshold matchTreshold = MatchThreshold.Default,
			int page = 1,
			int limit = QuotableResources.ResultsPerPageDefault
		)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw Error.NullOrEmpty(nameof(query));
			}

			if (!matchTreshold.IsValid())
			{
				throw Error.InvalidEnumValue(matchTreshold);
			}

			if (page <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(page), 0);
			}

			if (limit <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(limit), 0);
			}

			if (limit > QuotableResources.ResultsPerPageMax)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(limit), QuotableResources.MAX_RESULTS_PER_PAGE_NAME);
			}

			_query = query;
			_matchTreshold = matchTreshold;
			_page = page;
			_limit = limit;
			AutoComplete = autoComplete;
		}
	}
}
