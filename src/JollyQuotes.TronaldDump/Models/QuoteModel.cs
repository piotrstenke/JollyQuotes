using System;
using System.Text;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a quote that is returned by the <c>Tronald Dump</c> API.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteModel
	{
		/// <summary>
		/// Date the quote was said/written at.
		/// </summary>
		[JsonProperty("appeared_at", Order = 2, Required = Required.Always)]
		public DateTime AppearedAt { get; }

		/// <summary>
		/// Date the quote was added to the database at.
		/// </summary>
		[JsonProperty("created_at", Order = 3, Required = Required.Always)]
		public DateTime CreatedAt { get; }

		/// <summary>
		/// Date the quote was last updated at.
		/// </summary>
		[JsonProperty("updated_at", Order = 4)]
		public DateTime UpdatedAt { get; }

		/// <summary>
		/// Id of the quote.
		/// </summary>
		[JsonProperty("quote_id", Order = 0, Required = Required.Always)]
		public string Id { get; }

		/// <summary>
		/// Array of tags associated with this quote.
		/// </summary>
		[JsonProperty("tags", Order = 5, Required = Required.Always)]
		public string[] Tags { get; }

		/// <summary>
		/// Actual quote.
		/// </summary>
		[JsonProperty("value", Order = 1, Required = Required.Always)]
		public string Value { get; }

		/// <summary>
		/// Contains information about the quote's author and source.
		/// </summary>
		[JsonProperty("_embedded", Order = 6, Required = Required.Always)]
		public AuthorsAndSourcesModel Embedded { get; }

		/// <summary>
		/// Links to data of the quote.
		/// </summary>
		[JsonProperty("_links")]
		public SelfLinkModel Links { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteModel"/> class with an <paramref name="id"/>, <paramref name="value"/>, <paramref name="tags"/>, child <paramref name="links"/>, <paramref name="embedded"/> data and dates of first appearance and creation.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Actual quote.</param>
		/// <param name="tags">Array of tags associated with this quote.</param>
		/// <param name="links">Links to data of the quote..</param>
		/// <param name="embedded">Contains information about the quote's author and source.</param>
		/// <param name="appearedAt">Date the quote was said/written at.</param>
		/// <param name="createdAt">Date the quote was added to the database at.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="tags"/> is <see langword="null"/>. -or-
		/// <paramref name="links"/> is <see langword="null"/>. -or-
		/// <paramref name="embedded"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty. -or- <paramref name="value"/> is <see langword="null"/> or empty.</exception>
		public QuoteModel(
			string id,
			string value,
			string[] tags,
			SelfLinkModel links,
			AuthorsAndSourcesModel embedded,
			DateTime appearedAt,
			DateTime createdAt
		) : this(id, value, tags, links, embedded, appearedAt, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteModel"/> class with an <paramref name="id"/>, <paramref name="value"/>, <paramref name="tags"/>, child <paramref name="links"/>, <paramref name="embedded"/> data and dates of first appearance, creation and last update.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Actual quote.</param>
		/// <param name="tags">Array of tags associated with this quote.</param>
		/// <param name="links">Links to data of the quote..</param>
		/// <param name="embedded">Contains information about the quote's author and source.</param>
		/// <param name="appearedAt">Date the quote was said/written at.</param>
		/// <param name="createdAt">Date the quote was added to the database at.</param>
		/// <param name="updatedAt">Date the quote was last updated at.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="tags"/> is <see langword="null"/>. -or-
		/// <paramref name="links"/> is <see langword="null"/>. -or-
		/// <paramref name="embedded"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="tags"/> is empty.
		/// </exception>
		[JsonConstructor]
		public QuoteModel(
			string id,
			string value,
			string[] tags,
			SelfLinkModel links,
			AuthorsAndSourcesModel embedded,
			DateTime appearedAt,
			DateTime createdAt,
			DateTime updatedAt)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			if (tags is null)
			{
				throw Error.Null(nameof(tags));
			}

			if (links is null)
			{
				throw Error.Null(nameof(links));
			}

			if (embedded is null)
			{
				throw Error.Null(nameof(embedded));
			}

			if (tags.Length == 0)
			{
				throw Error.Empty(nameof(tags));
			}

			Id = id;
			Value = value;
			Tags = tags;
			Embedded = embedded;
			Links = links;
			AppearedAt = appearedAt;
			CreatedAt = createdAt;
			UpdatedAt = updatedAt;
		}

#pragma warning disable IDE0051 // Remove unused private members
		private bool PrintMembers(StringBuilder builder)
#pragma warning restore IDE0051 // Remove unused private members
		{
			builder.Append($"{nameof(Id)} = {Id}, ");
			builder.Append($"{nameof(Value)} = \"{Value}\", ");
			builder.Append($"{nameof(AppearedAt)} = {AppearedAt}, ");
			builder.Append($"{nameof(CreatedAt)} = {CreatedAt}, ");
			builder.Append($"{nameof(UpdatedAt)} = {UpdatedAt}, ");
			Internals.PrintArray(builder, nameof(Tags), Tags);
			builder.Append(", ");

			builder.Append($"{nameof(Links)} = {Links}, ");
			builder.Append($"{nameof(Embedded)} = {{ {Embedded} }}");

			return true;
		}
	}
}
