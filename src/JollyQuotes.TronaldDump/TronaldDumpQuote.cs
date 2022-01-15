using System;
using System.Linq;
using JollyQuotes.TronaldDump.Models;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Represents a quote that was created from data returned by the <c>Tronald Dump</c> API.
	/// </summary>
	[JsonObject]
	public sealed record TronaldDumpQuote : DatabaseModel, IQuote
	{
		private const string AUTHOR = "Donald Trump";

		private readonly string[] _tags;
		private readonly string _source;
		private readonly string _value;
		private readonly Id _id;

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
		[JsonProperty("tags", Order = 3, Required = Required.Always)]
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
		[JsonProperty("source", Order = 4, Required = Required.Always)]
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
		) : base(createdAt, updatedAt)
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

			_id = id;
			_value = value;
			_source = source;
			_tags = tags;
			AppearedAt = appearedAt;
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
				other._id == _id &&
				other._value == _value &&
				other._source == _source &&
				other.AppearedAt == AppearedAt &&
				other.UpdatedAt == UpdatedAt &&
				other.CreatedAt == CreatedAt &&
				other._tags.Length == _tags.Length &&
				other._tags.SequenceEqual(_tags);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(_id);
			hash.Add(_value);
			hash.Add(_source);
			hash.Add(AppearedAt);
			hash.Add(CreatedAt);
			hash.Add(UpdatedAt);
			hash.AddSequence(_tags);

			return hash.ToHashCode();
		}
	}
}
