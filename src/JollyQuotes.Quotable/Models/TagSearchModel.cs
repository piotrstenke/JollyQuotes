using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform a tag search.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TagSearchModel
	{
		/// <summary>
		/// Field to sort the results by.
		/// </summary>
		public SortBy SortBy { get; init; }

		/// <summary>
		/// Order in which the results are sorted.
		/// </summary>
		public SortOrder Order { get; init; }

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
		public TagSearchModel(SortBy sortBy, SortOrder order)
		{
			SortBy = sortBy;
			Order = order;
		}
	}
}
