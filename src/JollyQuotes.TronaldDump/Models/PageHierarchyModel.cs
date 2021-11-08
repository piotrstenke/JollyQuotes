using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a hierarchy of pages.
	/// </summary>
	[Serializable]
	[JsonObject]
	public class PageHierarchyModel
	{
		/// <summary>
		/// Link to the first page in the hierarchy.
		/// </summary>
		[JsonProperty("first", Order = 1, Required = Required.Always)]
		public LinkModel First { get; }

		/// <summary>
		/// Link to the last page in the hierarchy.
		/// </summary>
		[JsonProperty("last", Order = 4, Required = Required.Always)]
		public LinkModel Last { get; }

		/// <summary>
		/// Link to the next page in the hierarchy.
		/// </summary>
		[JsonProperty("next", Order = 3, Required = Required.Always)]
		public LinkModel Next { get; }

		/// <summary>
		/// Link to the previous page in the hierarchy.
		/// </summary>
		[JsonProperty("prev", Order = 2, Required = Required.Always)]
		public LinkModel Prev { get; }

		/// <summary>
		/// Link to the current page in the hierarchy.
		/// </summary>
		[JsonProperty("self", Order = 0, Required = Required.Always)]
		public LinkModel Self { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PageHierarchyModel"/> class with a single <see cref="LinkModel"/> specified for all properties.
		/// </summary>
		/// <param name="self"><see cref="LinkModel"/> to set for all properties.</param>
		/// <exception cref="ArgumentNullException"><paramref name="self"/> is <see langword="null"/>.</exception>
		public PageHierarchyModel(LinkModel self)
		{
			if (self is null)
			{
				throw Error.Null(nameof(self));
			}

			Self = self;
			Prev = self;
			Next = self;
			First = self;
			Last = self;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PageHierarchyModel"/> class with links to the current, previous and next pages.
		/// The <paramref name="prev"/> and <paramref name="next"/> pages also act as the <see cref="First"/> and <see cref="Last"/> pages respectively.
		/// </summary>
		/// <param name="self">Link to the current page in the hierarchy.</param>
		/// <param name="prev">Link to the previous page in the hierarchy.</param>
		/// <param name="next">Link to the next page in the hierarchy.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="self"/> is <see langword="null"/>. -or-
		/// <paramref name="prev"/> is <see langword="null"/>. -or-
		/// <paramref name="next"/> is <see langword="null"/>.
		/// </exception>
		public PageHierarchyModel(LinkModel self, LinkModel prev, LinkModel next) : this(self, prev, next, prev, next)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PageHierarchyModel"/> class with links to the current, previous, next, first and last pages specified.
		/// </summary>
		/// <param name="self">Link to the current page in the hierarchy.</param>
		/// <param name="prev">Link to the previous page in the hierarchy.</param>
		/// <param name="next">Link to the next page in the hierarchy.</param>
		/// <param name="first">Link to the first page in the hierarchy.</param>
		/// <param name="last">Link to the last page in the hierarchy.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="self"/> is <see langword="null"/>. -or-
		/// <paramref name="prev"/> is <see langword="null"/>. -or-
		/// <paramref name="next"/> is <see langword="null"/>. -or-
		/// <paramref name="first"/> is <see langword="null"/>. -or-
		/// <paramref name="last"/> is <see langword="null"/>.
		/// </exception>
		[JsonConstructor]
		public PageHierarchyModel(LinkModel self, LinkModel prev, LinkModel next, LinkModel first, LinkModel last)
		{
			if (self is null)
			{
				throw Error.Null(nameof(self));
			}

			if (prev is null)
			{
				throw Error.Null(nameof(prev));
			}

			if (next is null)
			{
				throw Error.Null(nameof(next));
			}

			if (first is null)
			{
				throw Error.Null(nameof(first));
			}

			if (last is null)
			{
				throw Error.Null(nameof(last));
			}

			Self = self;
			Prev = prev;
			Next = next;
			First = first;
			Last = last;
		}
	}
}
