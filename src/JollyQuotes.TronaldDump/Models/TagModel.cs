using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a tag.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TagModel
	{
		/// <summary>
		/// Date the tag was added to the database at.
		/// </summary>
		[JsonProperty("created_at", Order = 1, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Date the tag was updated at.
		/// </summary>
		[JsonProperty("updated_at", Order = 2)]
		public DateTime UpdatedAt { get; init; }

		/// <summary>
		/// Actual tag.
		/// </summary>
		[JsonProperty("value", Order = 0, Required = Required.Always)]
		public string Value { get; init; }

		/// <summary>
		/// Link that was used to retrieve this tag.
		/// </summary>
		[JsonProperty("_links", Order = 3, Required = Required.Always)]
		public SelfLinkModel Links { get; init; }

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
		[JsonConstructor]
		public TagModel(string value, SelfLinkModel links, DateTime createdAt, DateTime updatedAt)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			if (links is null)
			{
				throw Error.Null(nameof(links));
			}

			Value = value;
			Links = links;
			CreatedAt = createdAt;
			UpdatedAt = updatedAt;
		}
	}
}
