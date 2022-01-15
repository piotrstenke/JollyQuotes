using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a hierarchy of pages.
	/// </summary>
	[JsonObject]
	public sealed record PageHierarchyModel
	{
		private readonly LinkModel _first;
		private readonly LinkModel _last;
		private readonly LinkModel _next;
		private readonly LinkModel _prev;
		private readonly LinkModel _self;

		/// <summary>
		/// Link to the first page in the hierarchy.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("first", Order = 1, Required = Required.Always)]
		public LinkModel First
		{
			get => _first;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_first = value;
			}
		}

		/// <summary>
		/// Link to the last page in the hierarchy.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("last", Order = 4, Required = Required.Always)]
		public LinkModel Last
		{
			get => _last;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_last = value;
			}
		}

		/// <summary>
		/// Link to the next page in the hierarchy.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("next", Order = 3, Required = Required.Always)]
		public LinkModel Next
		{
			get => _next;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_next = value;
			}
		}

		/// <summary>
		/// Link to the previous page in the hierarchy.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("prev", Order = 2, Required = Required.Always)]
		public LinkModel Prev
		{
			get => _prev;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_prev = value;
			}
		}

		/// <summary>
		/// Link to the current page in the hierarchy.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("self", Order = 0, Required = Required.Always)]
		public LinkModel Self
		{
			get => _self;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_self = value;
			}
		}

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

			_self = self;
			_prev = self;
			_next = self;
			_first = self;
			_last = self;
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

			_self = self;
			_prev = prev;
			_next = next;
			_first = first;
			_last = last;
		}
	}
}
