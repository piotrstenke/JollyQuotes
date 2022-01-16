using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data needed to perform a quote list search.
	/// </summary>
	[JsonObject]
	public sealed record QuoteListSearchModel : QuoteSearchModel
	{
		private readonly int _limit;
		private readonly int _page;
		private readonly QuoteSortBy _sortBy;
		private readonly SortOrder _order;

		/// <summary>
		/// Field to sort the results by.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("sortBy", Order = 6)]
		public QuoteSortBy SortBy
		{
			get => _sortBy;
			init
			{
				if (!value.IsValid())
				{
					throw Error.InvalidEnumValue(value);
				}

				_sortBy = value;
			}
		}

		/// <summary>
		/// Order in which the results are sorted.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("order", Order = 7)]
		public SortOrder Order
		{
			get => _order;
			init
			{
				if (!value.IsValid())
				{
					throw Error.InvalidEnumValue(value);
				}

				_order = value;
			}
		}

		/// <summary>
		/// Maximum number of results on a single page.
		/// </summary>
		/// <remarks>The default value is equal to <see cref="QuotableResources.ResultsPerPageDefault"/>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.</exception>
		[JsonProperty("limit", Order = 8)]
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
		[JsonProperty("page", Order = 9)]
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
		/// Initializes a new instance of the <see cref="QuoteListSearchModel"/> class.
		/// </summary>
		public QuoteListSearchModel()
		{
			_page = 1;
			_limit = QuotableResources.ResultsPerPageDefault;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteListSearchModel"/> class with a <paramref name="minLength"/>, <paramref name="maxLength"/>, <paramref name="tags"/> and <paramref name="authors"/> specified.
		/// </summary>
		/// <param name="minLength">Minimum length in characters of the quote.</param>
		/// <param name="maxLength">Maximum length in characters of the quote.</param>
		/// <param name="page">Current page number.</param>
		/// <param name="limit">Maximum number of results on a single page.</param>
		/// <param name="sortBy">Field to sort the results by.</param>
		/// <param name="order">Order in which the results are sorted.</param>
		/// <param name="tags">Tags associated with the quote searched for.</param>
		/// <param name="authors">Collection of all possible authors of the returned quote.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="sortBy"/> is not a valid enum value. -or-
		/// <paramref name="order"/> is not a valid enum value.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be less than or equal to <paramref name="maxLength"/>. -or-
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.
		/// </exception>
		public QuoteListSearchModel(
			int minLength = default,
			int? maxLength = default,
			int page = 1,
			int limit = QuotableResources.ResultsPerPageDefault,
			QuoteSortBy sortBy = default,
			SortOrder order = default,
			TagExpression? tags = default,
			params string[]? authors
#pragma warning disable CS0618 // Type or member is obsolete
		) : this(minLength, maxLength, page, limit, sortBy, order, tags, authors, default)
#pragma warning restore CS0618 // Type or member is obsolete
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteListSearchModel"/> class with a <paramref name="minLength"/>, <paramref name="maxLength"/>, <paramref name="tags"/> and <paramref name="authors"/> specified.
		/// </summary>
		/// <param name="minLength">Minimum length in characters of the quote.</param>
		/// <param name="maxLength">Maximum length in characters of the quote.</param>
		/// <param name="page">Current page number.</param>
		/// <param name="limit">Maximum number of results on a single page.</param>
		/// <param name="sortBy">Field to sort the results by.</param>
		/// <param name="order">Order in which the results are sorted.</param>
		/// <param name="tags">Tags associated with the quote searched for.</param>
		/// <param name="authors">Collection of all possible authors of the returned quote.</param>
		/// <param name="authorIds">Collection of ids of all possible authors of the returned quote.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="sortBy"/> is not a valid enum value. -or-
		/// <paramref name="order"/> is not a valid enum value.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be less than or equal to <paramref name="maxLength"/>. -or-
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.
		/// </exception>
		[JsonConstructor]
		public QuoteListSearchModel(
			int minLength = default,
			int? maxLength = default,
			int page = 1,
			int limit = QuotableResources.ResultsPerPageDefault,
			QuoteSortBy sortBy = default,
			SortOrder order = default,
			TagExpression? tags = default,
			string[]? authors = default,
			string[]? authorIds = default
		) : base(minLength, maxLength, tags, authors, authorIds)
		{
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

			if (!sortBy.IsValid())
			{
				throw Error.InvalidEnumValue(sortBy);
			}

			if (!order.IsValid())
			{
				throw Error.InvalidEnumValue(order);
			}

			_page = page;
			_limit = limit;
			_sortBy = sortBy;
			_order = order;
		}
	}
}
