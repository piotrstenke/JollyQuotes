using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a tag.
	/// </summary>
	[JsonObject]
	public sealed record TagModel : DatabaseModel
	{
		private readonly string _value;
		private readonly SelfLinkModel _links;

		/// <summary>
		/// Actual tag.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("value", Order = 0, Required = Required.Always)]
		public string Value
		{
			get => _value;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_value = value;
			}
		}

		/// <summary>
		/// Link that was used to retrieve this tag.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_links", Order = 1, Required = Required.Always)]
		public SelfLinkModel Links
		{
			get => _links;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_links = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagModel"/> class with actual tag <paramref name="value"/>, <paramref name="links"/> and creation date specified.
		/// </summary>
		/// <param name="value">Actual tag.</param>
		/// <param name="links">Link that was used to retrieve this tag.</param>
		/// <param name="createdAt">Date the tag was added to the database at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or empty.</exception>
		public TagModel(string value, SelfLinkModel links, DateTime createdAt) : this(value, links, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagModel"/> class with actual tag <paramref name="value"/>, <paramref name="links"/> and dates of creation and last update specified.
		/// </summary>
		/// <param name="value">Actual tag.</param>
		/// <param name="links">Link that was used to retrieve this tag.</param>
		/// <param name="createdAt">Date the tag was added to the database at.</param>
		/// <param name="updatedAt">Date the tag was updated at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater than or equal to <paramref name="createdAt"/>.</exception>
		[JsonConstructor]
		public TagModel(string value, SelfLinkModel links, DateTime createdAt, DateTime updatedAt) : base(createdAt, updatedAt)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			if (links is null)
			{
				throw Error.Null(nameof(links));
			}

			_value = value;
			_links = links;
		}
	}
}
