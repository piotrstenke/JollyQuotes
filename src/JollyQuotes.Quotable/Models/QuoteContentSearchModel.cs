using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform a quote search by its contents.
	/// </summary>
	[JsonObject]
	public sealed record QuoteContentSearchModel
	{
		private readonly string _query;
		private readonly QuoteSearchFields _fields;
		private readonly FuzzyMatchingTreshold _fuzzyMaxEdits;
		private readonly int _fuzzyMaxExpansions;
		private readonly int _page;
		private readonly int _limit;

		/// <summary>
		/// Search query.
		/// </summary>
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
		/// Fields to search by.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("fields", Order = 1)]
		[DefaultValue(QuoteSearchFields.All)]
		public QuoteSearchFields Fields
		{
			get => _fields;
			init
			{
				if (!value.HasAnyFlag())
				{
					throw Error.InvalidEnumValue(value);
				}

				_fields = value;
			}
		}

		/// <summary>
		/// Maximum number of single-character edits required to match a given search term.
		/// </summary>
		/// <remarks>The default value is <see cref="FuzzyMatchingTreshold.Default"/>.</remarks>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("fuzzyMaxEdits", Order = 2)]
		[DefaultValue(FuzzyMatchingTreshold.Default)]
		public FuzzyMatchingTreshold FuzzyMaxEdits
		{
			get => _fuzzyMaxEdits;
			init
			{
				if (!value.IsValid())
				{
					throw Error.InvalidEnumValue(value);
				}

				_fuzzyMaxEdits = value;
			}
		}

		/// <summary>
		/// When fuzzy search is enabled, this is the maximum number of variations to generate and search for.
		/// This limit applies on a per-token basis.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="QuotableResources.FuzzyMaxExpansions"/>.
		/// </exception>
		[JsonProperty("fuzzyMaxExpansions", Order = 3)]
		[DefaultValue(QuotableResources.FuzzyDefaultExpansions)]
		public int FuzzyMaxExpansions
		{
			get => _fuzzyMaxExpansions;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if (value > QuotableResources.ResultsPerPageMax)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), QuotableResources.MAX_FUZZY_EXPANSIONS_NAME);
				}

				_fuzzyMaxExpansions = value;
			}
		}

		/// <summary>
		/// Maximum number of results on a single page.
		/// </summary>
		/// <remarks>The default value is equal to <see cref="QuotableResources.ResultsPerPageDefault"/>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.</exception>
		[JsonProperty("limit", Order = 4)]
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
		[JsonProperty("page", Order = 5)]
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
		/// Initializes a new instance of the <see cref="QuoteContentSearchModel"/> class with a <paramref name="query"/> specified.
		/// </summary>
		/// <param name="query">Search query.</param>
		/// <exception cref="ArgumentException"><paramref name="query"/> is <see langword="null"/> or empty.</exception>
		public QuoteContentSearchModel(string query) : this(query, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteContentSearchModel"/> class with a <paramref name="query"/>, target <paramref name="fields"/>,
		/// <paramref name="fuzzyMaxEdits"/>, <paramref name="fuzzyMaxExpansions"/>, current <paramref name="page"/> and <paramref name="limit"/> specified.
		/// </summary>
		/// <param name="query">Search query.</param>
		/// <param name="fields">Fields to search by.</param>
		/// <param name="fuzzyMaxEdits">Maximum number of single-character edits required to match a given search term.</param>
		/// <param name="fuzzyMaxExpansions">When fuzzy search is enabled, this is the maximum number of variations to generate and search for. This limit applies on a per-token basis.</param>
		/// <param name="page">Current page number.</param>
		/// <param name="limit">Maximum number of results on a single page.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="query"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="fields"/> is not a valid enum value. -or-
		/// <paramref name="fuzzyMaxEdits"/> is not a valid enum value.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="fuzzyMaxExpansions"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="fuzzyMaxExpansions"/> must be less than or equal to <see cref="QuotableResources.FuzzyMaxExpansions"/>. -or-
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.
		/// </exception>
		[JsonConstructor]
		public QuoteContentSearchModel(
			string query,
			QuoteSearchFields fields = QuoteSearchFields.All,
			FuzzyMatchingTreshold fuzzyMaxEdits = FuzzyMatchingTreshold.Default,
			int fuzzyMaxExpansions = QuotableResources.FuzzyDefaultExpansions,
			int page = 1,
			int limit = QuotableResources.ResultsPerPageDefault
		)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				throw Error.NullOrEmpty(nameof(query));
			}

			if (!fields.HasAnyFlag())
			{
				throw Error.InvalidEnumValue(fields);
			}

			if (!fuzzyMaxEdits.IsValid())
			{
				throw Error.InvalidEnumValue(fuzzyMaxEdits);
			}

			if (fuzzyMaxExpansions < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(fuzzyMaxExpansions), 0);
			}

			if (fuzzyMaxExpansions > QuotableResources.ResultsPerPageMax)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(fuzzyMaxExpansions), QuotableResources.MAX_FUZZY_EXPANSIONS_NAME);
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
			_page = page;
			_limit = limit;
			_fuzzyMaxExpansions = fuzzyMaxExpansions;
			_fuzzyMaxEdits = fuzzyMaxEdits;
			_fields = fields;
		}
	}
}
