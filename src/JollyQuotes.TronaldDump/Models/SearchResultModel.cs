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
		/// <inheritdoc/>
		[JsonProperty("count", Order = 0, Required = Required.Always)]
		public int Count { get; init; }

		/// <summary>
		/// Values present in the current page of the search result.
		/// </summary>
		[JsonProperty("_embedded", Order = 2, Required = Required.Always)]
		public T Embedded { get; init; }

		/// <inheritdoc/>
		[JsonProperty("_links", Order = 3)]
		public PageHierarchyModel? Links { get; init; }

		/// <inheritdoc/>
		[JsonProperty("total", Order = 1, Required = Required.Always)]
		public int Total { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with <paramref name="embedded"/> data,
		/// number of its elements and their <paramref name="total"/> number.
		/// </summary>
		/// <param name="count">Number of values contained on the current page.</param>
		/// <param name="total">Total number of values contained in the search result.</param>
		/// <param name="embedded">Values present in the current page of the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="embedded"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="total"/> cannot be less than <c>0</c>. -or-
		/// <paramref name="count"/> cannot be less than <c>0</c>. -or-
		/// <paramref name="count"/> cannot be greater than <paramref name="total"/>.
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
		/// <paramref name="total"/> cannot be less than <c>0</c>. -or-
		/// <paramref name="count"/> cannot be less than <c>0</c>. -or-
		/// <paramref name="count"/> cannot be greater than <paramref name="total"/>.
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
				throw new ArgumentOutOfRangeException(nameof(total), $"{total} cannot be less than 0");
			}

			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), $"{count} cannot be less than 0");
			}

			if (count > total)
			{
				throw new ArgumentOutOfRangeException(nameof(count), $"{nameof(count)} cannot be greater than '{total}'");
			}

			Count = count;
			Total = total;
			Links = links;
			Embedded = embedded;
		}
	}
}
