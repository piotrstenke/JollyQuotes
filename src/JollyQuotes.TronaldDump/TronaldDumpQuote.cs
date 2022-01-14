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
		private const string AUTHOR = "Donald Trump";

		private readonly string[] _tags;
		private readonly string _source;
		private readonly string _value;
		private readonly Id _id;
		private readonly DateTime _updatedAt;

		/// <summary>
		/// Date the quote was added to the database at.
		/// </summary>
		[JsonProperty("createdAt", Order = 3, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Date the quote was last updated at.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="CreatedAt"/>.</exception>
		[JsonProperty("updatedAt", Order = 4)]
		public DateTime UpdatedAt
		{
			get => _updatedAt;
			init
			{
				if (value < CreatedAt)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(CreatedAt));
				}

				_updatedAt = value;
			}
		}

		/// <summary>
		/// Date the quote was said/written at.
		/// </summary>
		[JsonProperty("appearedAt", Order = 2, Required = Required.Always)]
		public DateTime AppearedAt { get; init; }

		/// <inheritdoc/>
		/// <exception cref="ArgumentException">Value must be initialized.</exception>
		[JsonProperty("id", Order = 0, Required = Required.Always)]
		public Id Id
		{
			get => _id;
			init
			{
				if (value == default)
				{
					throw Error.NotInitialized(nameof(value));
				}

				_id = value;
			}
		}

		/// <summary>
		/// Array of tags associated with this quote.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("tags", Order = 5, Required = Required.Always)]
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("source", Order = 6, Required = Required.Always)]
		public string Source
		{
			get => _source;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_source = value;
			}
		}

		string IQuote.Author => AUTHOR;
		DateTime? IQuote.Date => AppearedAt;

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
		/// <paramref name="id"/> must be initialized. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> is <see langword="null"/> or empty.
		/// </exception>
		public TronaldDumpQuote(
			Id id,
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
		/// <paramref name="id"/> must be initialized. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater than or equal to <paramref name="createdAt"/>.</exception>
		[JsonConstructor]
		public TronaldDumpQuote(
			Id id,
			string value,
			string source,
			string[] tags,
			DateTime appearedAt,
			DateTime createdAt,
			DateTime updatedAt
		)
		{
			if (id == default)
			{
				throw Error.NotInitialized(nameof(id));
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

			if (updatedAt < createdAt)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(updatedAt), nameof(createdAt));
			}

			_id = id;
			_value = value;
			_source = source;
			_tags = tags;
			_updatedAt = updatedAt;
			AppearedAt = appearedAt;
			CreatedAt = createdAt;
		}

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
	}
}
