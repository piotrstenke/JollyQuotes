using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data needed to perform a quote list search.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteListSearchModel : QuoteSearchModel
	{
		private readonly int _limit;
		private readonly int _page;

		/// <summary>
		/// Field to sort the results by.
		/// </summary>
		public QuoteSortBy SortBy { get; init; }

		/// <summary>
		/// Order in which the results are sorted.
		/// </summary>
		public SortOrder Order { get; init; }

		/// <summary>
		/// Maximum number of results on a single page.
		/// </summary>
		/// <remarks>The default value is equal to <see cref="QuotableResources.DefaultResultsPerPage"/>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="QuotableResources.MaxResultsPerPage"/>.</exception>
		public int Limit
		{
			get => _limit;
			init
			{
				if (value <= 0)
				{
					throw Error.MustBeGreaterThan(nameof(value), 0);
				}

				if (value > QuotableResources.MaxResultsPerPage)
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
			_limit = QuotableResources.DefaultResultsPerPage;
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
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be less than or equal to <paramref name="maxLength"/>. -or-
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.MaxResultsPerPage"/>.
		/// </exception>
		[JsonConstructor]
		public QuoteListSearchModel(
			int minLength = default,
			int? maxLength = default,
			int page = 1,
			int limit = QuotableResources.DefaultResultsPerPage,
			QuoteSortBy sortBy = default,
			SortOrder order = default,
			TagExpression? tags = default,
			params string[]? authors
		) : base(minLength, maxLength, tags, authors)
		{
			if (page <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(page), 0);
			}

			if (limit <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(limit), 0);
			}

			if (limit > QuotableResources.MaxResultsPerPage)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(limit), QuotableResources.MAX_RESULTS_PER_PAGE_NAME);
			}

			_page = page;
			_limit = limit;
			SortBy = sortBy;
			Order = order;
		}
	}
}
