using System;
using System.Linq;
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
		private readonly string _id;
		private readonly string _value;
		private readonly string[] _tags;
		private readonly AuthorsAndSourcesModel _embedded;
		private readonly SelfLinkModel _links;

		/// <summary>
		/// Date the quote was said/written at.
		/// </summary>
		[JsonProperty("appeared_at", Order = 2, Required = Required.Always)]
		public DateTime AppearedAt { get; init; }

		/// <summary>
		/// Date the quote was added to the database at.
		/// </summary>
		[JsonProperty("created_at", Order = 3, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Date the quote was last updated at.
		/// </summary>
		[JsonProperty("updated_at", Order = 4)]
		public DateTime UpdatedAt { get; init; }

		/// <summary>
		/// Id of the quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("quote_id", Order = 0, Required = Required.Always)]
		public string Id
		{
			get => _id;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_id = value;
			}
		}

		/// <summary>
		/// Array of tags associated with this quote.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("tags", Order = 5, Required = Required.DisallowNull)]
		public string[] Tags
		{
			get => _tags;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_tags = value;
			}
		}

		/// <summary>
		/// Actual quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("value", Order = 1, Required = Required.Always)]
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
		/// Contains information about the quote's author and source.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_embedded", Order = 6, Required = Required.Always)]
		public AuthorsAndSourcesModel Embedded
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

		/// <summary>
		/// Links to data of the quote.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_links")]
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
		/// <paramref name="value"/> is <see langword="null"/> or empty.
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

			_id = id;
			_value = value;
			_tags = tags;
			_embedded = embedded;
			_links = links;
			AppearedAt = appearedAt;
			CreatedAt = createdAt;
			UpdatedAt = updatedAt;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(Id);
			hash.Add(Value);
			hash.Add(AppearedAt);
			hash.Add(CreatedAt);
			hash.Add(UpdatedAt);
			hash.Add(Links);
			hash.Add(Embedded);

			foreach (string tag in Tags)
			{
				hash.Add(tag);
			}

			return hash.ToHashCode();
		}

		/// <inheritdoc/>
		public bool Equals(QuoteModel? other)
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
				other.Id == Id &&
				other.AppearedAt == AppearedAt &&
				other.CreatedAt == CreatedAt &&
				other.UpdatedAt == UpdatedAt &&
				other.Value == Value &&
				other.Links == Links &&
				other.Tags.Length == Tags.Length &&
				other.Tags.SequenceEqual(Tags) &&
				other.Embedded == Embedded;
		}
	}
}
