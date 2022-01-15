using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform a tag search.
	/// </summary>
	[JsonObject]
	public sealed record TagSearchModel
	{
		private readonly SortBy _sortBy;
		private readonly SortOrder _order;

		/// <summary>
		/// Field to sort the results by.
		/// </summary>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		[JsonProperty("sortBy", Order = 0)]
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
		[JsonProperty("order", Order = 1)]
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
		/// Initializes a new instance of the <see cref="TagSearchModel"/> class.
		/// </summary>
		public TagSearchModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagSearchModel"/> class with sort method and <paramref name="order"/> specified.
		/// </summary>
		/// <param name="sortBy">Field to sort the results by.</param>
		/// <param name="order">Order in which the results are sorted.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="sortBy"/> is not a valid enum value. -or-
		/// <paramref name="order"/> is not a valid enum value.
		/// </exception>
		public TagSearchModel(SortBy sortBy, SortOrder order)
		{
			if (!sortBy.IsValid())
			{
				throw Error.InvalidEnumValue(sortBy);
			}

			if (!order.IsValid())
			{
				throw Error.InvalidEnumValue(order);
			}

			_sortBy = sortBy;
			_order = order;
		}
	}
}
