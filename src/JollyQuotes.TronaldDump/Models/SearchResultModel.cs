using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <inheritdoc cref="ISearchResultModel"/>
	/// <typeparam name="T">Type of data searched for.</typeparam>
	[Serializable]
	[JsonObject]
	public sealed record SearchResultModel<T> : ISearchResultModel where T : class
	{
		private readonly int _total;
		private readonly int _count;
		private readonly T _embedded;

		/// <inheritdoc/>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or- Value must be less than or equal to <see cref="Total"/>.</exception>
		[JsonProperty("count", Order = 0, Required = Required.Always)]
		public int Count
		{
			get => _count;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if(_count > _total)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), nameof(Total));
				}

				_count = value;
			}
		}

		/// <summary>
		/// Values present in the current page of the search result.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_embedded", Order = 2, Required = Required.Always)]
		public T Embedded
		{
			get => _embedded;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_embedded = value;
			}
		}

		/// <inheritdoc/>
		[JsonProperty("_links", Order = 3)]
		public PageHierarchyModel? Links { get; init; }

		/// <inheritdoc/>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>.</exception>
		[JsonProperty("total", Order = 1, Required = Required.Always)]
		public int Total
		{
			get => _total;
			init
			{
				if(value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_total = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with <paramref name="embedded"/> data,
		/// number of its elements and their <paramref name="total"/> number.
		/// </summary>
		/// <param name="count">Number of values contained on the current page.</param>
		/// <param name="total">Total number of values contained in the search result.</param>
		/// <param name="embedded">Values present in the current page of the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="embedded"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="total"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be less than or equal to <paramref name="total"/>.
		/// </exception>
		public SearchResultModel(int count, int total, T embedded) : this(count, total, embedded, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with <paramref name="embedded"/> data,
		/// number of its elements and their <paramref name="total"/> number and <paramref name="links"/> to all pages in the search result.
		/// </summary>
		/// <param name="count">Number of values contained on the current page.</param>
		/// <param name="total">Total number of values contained in the search result.</param>
		/// <param name="embedded">Values present in the current page of the search result.</param>
		/// <param name="links">Links to all the pages in the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="embedded"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="total"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be less than or equal <paramref name="total"/>.
		/// </exception>
		[JsonConstructor]
		public SearchResultModel(int count, int total, T embedded, PageHierarchyModel? links)
		{
			if (embedded is null)
			{
				throw Error.Null(nameof(embedded));
			}

			if (total < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(total), 0);
			}

			if (count < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(count), 0);
			}

			if (count > total)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(count), nameof(total));
			}

			_count = count;
			_total = total;
			Links = links;
			_embedded = embedded;
		}
	}
}
