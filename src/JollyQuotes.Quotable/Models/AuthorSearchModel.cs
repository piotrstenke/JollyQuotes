using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform an author search.
	/// </summary>
	[JsonObject]
	public sealed record AuthorSearchModel
	{
		private readonly string[]? _slugs;
		private readonly int _page;
		private readonly int _limit;
		private readonly SortBy _sortBy;
		private readonly SortOrder _order;

		/// <summary>
		/// Slug to filter the authors by.
		/// </summary>
		/// <remarks>This property returns the first element of the <see cref="Slugs"/> array.
		/// <para>If value of this property is set to <see langword="null"/> or an empty <see cref="string"/>, <see cref="Slugs"/> is automatically set to <see langword="null"/> as well.</para></remarks>
		[JsonIgnore]
		public string? Slug
		{
			get
			{
				if (_slugs is null || _slugs.Length == 0)
				{
					return null;
				}

				return _slugs[0];
			}
			init => Internals.SetFirstElement(value, ref _slugs);
		}

		/// <summary>
		/// Collection of author slugs to filter by.
		/// </summary>
		[JsonProperty("slugs", Order = 0)]
		public string[]? Slugs { get; init; }

		/// <summary>
		/// Determines whether a <see cref="Slug"/> is included in the search query.
		/// </summary>
		[MemberNotNullWhen(true, nameof(Slug), nameof(Slugs))]
		[JsonIgnore]
		public bool HasSlug => _slugs is not null && _slugs.Length > 0;

		/// <summary>
		/// Field to sort the results by.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("sortBy", Order = 1)]
		public SortBy SortBy
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
		[JsonProperty("order", Order = 2)]
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
		/// Initializes a new instance of the <see cref="AuthorSearchModel"/> class.
		/// </summary>
		public AuthorSearchModel()
		{
			_page = 1;
			_limit = QuotableResources.ResultsPerPageDefault;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorSearchModel"/> class with a current <paramref name="page"/>, <paramref name="limit"/>, <paramref name="sortBy"/>, <paramref name="order"/> and <paramref name="slugs"/> specified.
		/// </summary>
		/// <param name="page">Current page number.</param>
		/// <param name="limit">Maximum number of results on a single page.</param>
		/// <param name="sortBy">Field to sort the results by.</param>
		/// <param name="order">Order in which the results are sorted.</param>
		/// <param name="slugs">Collection of author slugs to filter by.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="sortBy"/> is not a valid enum value. -or-
		/// <paramref name="order"/> is not a valid enum value.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="limit"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.
		/// </exception>
		[JsonConstructor]
		public AuthorSearchModel(
			int page = 1,
			int limit = QuotableResources.ResultsPerPageDefault,
			SortBy sortBy = default,
			SortOrder order = default,
			params string[]? slugs
		)
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
			_slugs = slugs;
		}

		/// <inheritdoc/>
		public bool Equals(AuthorSearchModel? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(other, this))
			{
				return true;
			}

			return
				other._limit == _limit &&
				other._page == _page &&
				other._sortBy == _sortBy &&
				other._order == _order &&
				Internals.SequenceEqual(other._slugs, _slugs);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(_limit);
			hash.Add(_page);
			hash.Add(_sortBy);
			hash.Add(_order);
			hash.AddSequence(_slugs);

			return hash.ToHashCode();
		}
	}
}
