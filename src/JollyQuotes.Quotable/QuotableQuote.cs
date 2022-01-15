using System;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Represents a quote created by the <c>quotable</c> API.
	/// </summary>
	[JsonObject]
	public sealed record QuotableQuote : IQuote
	{
		private readonly string[] _tags;
		private readonly Id _id;
		private readonly string _value;
		private readonly string _author;
		private readonly string _authorSlug;
		private readonly DateTime _updatedAt;

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

		/// <summary>
		/// Full name of the quote's author.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("author", Order = 2, Required = Required.Always)]
		public string Author
		{
			get => _author;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_author = value;
			}
		}

		/// <summary>
		/// Slug version of the <see cref="Author"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("authorSlug", Order = 3, Required = Required.Always)]
		public string AuthorSlug
		{
			get => _authorSlug;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_authorSlug = value;
			}
		}

		/// <summary>
		/// Tags associated with this quote.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("tags", Order = 4, Required = Required.DisallowNull)]
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
		/// Date the quote was added at.
		/// </summary>
		[JsonProperty("createdAt", Order = 5, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Date of the quote's most recent update.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="CreatedAt"/>.</exception>
		[JsonProperty("updatedAt", Order = 6)]
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

		string IQuote.Author => AuthorSlug;
		DateTime? IQuote.Date => default;
		string IQuote.Source => QuotableResources.ApiPage + $"/quotes/{Id}";

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuote"/> class with an <paramref name="id"/>, actual <paramref name="value"/>, <paramref name="author"/>, <paramref name="authorSlug"/>, associated <paramref name="tags"/> and date of the quote's creation.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Text of the quote.</param>
		/// <param name="author">Full name of the quote's author.</param>
		/// <param name="authorSlug">Slug version of the <paramref name="author"/>.</param>
		/// <param name="tags">Tags associated with this quote.</param>
		/// <param name="createdAt">Date the quote was added at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> must be initialized. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty.
		/// <paramref name="author"/> is <see langword="null"/> or empty.
		/// <paramref name="authorSlug"/> is <see langword="null"/> or empty.
		/// </exception>
		public QuotableQuote(
			Id id,
			string value,
			string author,
			string authorSlug,
			string[] tags,
			DateTime createdAt
		) : this(id, value, author, authorSlug, tags, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableQuote"/> class with an <paramref name="id"/>, actual <paramref name="value"/>, <paramref name="author"/>, <paramref name="authorSlug"/>, associated <paramref name="tags"/> and dates of creation and last update.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="value">Text of the quote.</param>
		/// <param name="author">Full name of the quote's author.</param>
		/// <param name="authorSlug">Slug version of the <paramref name="author"/>.</param>
		/// <param name="tags">Tags associated with this quote.</param>
		/// <param name="createdAt">Date the quote was added at.</param>
		/// <param name="updatedAt">Date of the quote's most recent update.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> must be initialized. -or-
		/// <paramref name="value"/> is <see langword="null"/> or empty.
		/// <paramref name="author"/> is <see langword="null"/> or empty.
		/// <paramref name="authorSlug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater than or equal to <paramref name="createdAt"/>.</exception>
		[JsonConstructor]
		public QuotableQuote(
			Id id,
			string value,
			string author,
			string authorSlug,
			string[] tags,
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

			if (string.IsNullOrWhiteSpace(author))
			{
				throw Error.NullOrEmpty(nameof(author));
			}

			if (string.IsNullOrWhiteSpace(authorSlug))
			{
				throw Error.NullOrEmpty(nameof(authorSlug));
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
			_author = author;
			_authorSlug = authorSlug;
			_tags = tags;
			_updatedAt = updatedAt;
			CreatedAt = createdAt;
		}

		/// <inheritdoc/>
		public bool Equals(QuotableQuote? other)
		{
			if (other is null)
			{
				return false;
			}

			return
				other._id == _id &&
				other._author == _author &&
				other._authorSlug == _authorSlug &&
				other._value == _value &&
				other.CreatedAt == CreatedAt &&
				other._updatedAt == _updatedAt &&
				other._tags.Length == _tags.Length &&
				other._tags.SequenceEqual(_tags);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(_id);
			hash.Add(_author);
			hash.Add(_authorSlug);
			hash.Add(_value);
			hash.Add(CreatedAt);
			hash.Add(_updatedAt);
			hash.AddSequence(_tags);

			return hash.ToHashCode();
		}
	}
}
