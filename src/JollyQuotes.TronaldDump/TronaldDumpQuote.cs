using System;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Represents a quote that was created from data returned by the <c>Tronald Dump</c> API.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TronaldDumpQuote : IQuote
	{
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
		/// Date the quote was said/written at.
		/// </summary>
		[JsonProperty("appeared_at", Order = 2, Required = Required.Always)]
		public DateTime AppearedAt { get; init; }

		/// <summary>
		/// Id of the quote.
		/// </summary>
		[JsonProperty("quote_id", Order = 0, Required = Required.Always)]
		public string Id { get; init; }

		/// <summary>
		/// Array of tags associated with this quote.
		/// </summary>
		[JsonProperty("tags", Order = 5, Required = Required.Always)]
		public string[] Tags { get; init; }

		/// <inheritdoc/>
		[JsonProperty("quote", Order = 1, Required = Required.Always)]
		public string Value { get; init; }

		/// <inheritdoc/>
		[JsonProperty("source", Order = 6, Required = Required.Always)]
		public string Source { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuote"/> class with an <paramref name="id"/>,
		/// actual <paramref name="value"/>, <paramref name="source"/>, <paramref name="tags"/>
		/// and dates of appearance and creation specified.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Actual quote.</param>
		/// <param name="source">Source of the quote (e.g. a link, file name or raw text).</param>
		/// <param name="tags">Array of tags associated with this quote.</param>
		/// <param name="appearedAt">Date the quote was said/written at.</param>
		/// <param name="createdAt">Date the quote was added to the database at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> is <see langword="null"/> or empty.
		/// </exception>
		public TronaldDumpQuote(
			string id,
			string value,
			string source,
			string[] tags,
			DateTime appearedAt,
			DateTime createdAt
		) : this(id, value, source, tags, appearedAt, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpQuote"/> class with an <paramref name="id"/>,
		/// actual <paramref name="value"/>, <paramref name="source"/>, <paramref name="tags"/>
		/// and dates of appearance, creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Actual quote.</param>
		/// <param name="source">Source of the quote (e.g. a link, file name or raw text).</param>
		/// <param name="tags">Array of tags associated with this quote.</param>
		/// <param name="appearedAt">Date the quote was said/written at.</param>
		/// <param name="createdAt">Date the quote was added to the database at.</param>
		/// <param name="updatedAt">Date the quote was last updated at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> is <see langword="null"/> or empty.
		/// </exception>
		[JsonConstructor]
		public TronaldDumpQuote(
			string id,
			string value,
			string source,
			string[] tags,
			DateTime appearedAt,
			DateTime createdAt,
			DateTime updatedAt
		)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			if (tags is null)
			{
				throw Error.Null(nameof(tags));
			}

			Id = id;
			Value = value;
			Source = source;
			Tags = tags;
			AppearedAt = appearedAt;
			CreatedAt = createdAt;
			UpdatedAt = updatedAt;
		}

		string IQuote.Author => "Donald Trump";
		DateTime? IQuote.Date => AppearedAt;

		/// <inheritdoc/>
		public bool Equals(TronaldDumpQuote? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return
				other.Id == Id &&
				other.Value == Value &&
				other.Source == Source &&
				other.AppearedAt == AppearedAt &&
				other.UpdatedAt == UpdatedAt &&
				other.CreatedAt == CreatedAt &&
				other.Tags.Length == Tags.Length &&
				other.Tags.SequenceEqual(Tags);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(Id);
			hash.Add(Value);
			hash.Add(Source);
			hash.Add(AppearedAt);
			hash.Add(CreatedAt);
			hash.Add(UpdatedAt);

			foreach (string tag in Tags)
			{
				hash.Add(tag);
			}

			return hash.ToHashCode();
		}

		int IQuote.GetId()
		{
			return Id.GetHashCode();
		}
	}
}
