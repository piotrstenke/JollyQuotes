using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes
{
	/// <summary>
	/// Contains a quote, its author, source, date and related tags.
	/// </summary>
	/// <remarks>This class implements the <see cref="IEquatable{T}"/> interface - two instances are compare by their value, not reference.</remarks>
	[Serializable]
	[JsonObject]
	public record Quote : IQuote
	{
		/// <summary>
		/// <see cref="JsonConverter"/> that formats <see cref="DateTime"/> and <see cref="DateTimeOffset"/> values to use only the <c>date</c> part.
		/// </summary>
		public sealed class DateOnlyConverter : IsoDateTimeConverter
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="DateOnlyConverter"/> class.
			/// </summary>
			public DateOnlyConverter()
			{
				DateTimeFormat = Quote.DateTimeFormat;
			}
		}

		private readonly string _value;
		private readonly string _author;
		private readonly string _source;
		private readonly string[] _tags;

		/// <summary>
		/// Format of date time values used by <c>JollyQuotes</c> libraries.
		/// </summary>
		public const string DateTimeFormat = "yyyy-MM-dd";

		/// <summary>
		/// <see cref="JsonSerializerSettings"/> used to internally deserialize json objects.
		/// </summary>
		public static JsonSerializerSettings JsonSettings { get; } = new()
		{
			DateFormatString = DateTimeFormat
		};

		/// <summary>
		/// Value used when an author of a quote is unknown.
		/// </summary>
		public const string UnknownAuthor = "Unknown";

		/// <summary>
		/// Value of <see cref="Value"/> used in <see cref="Unknown"/>.
		/// </summary>
		public const string NoContent = "No Content";

		/// <summary>
		/// Returns a new instance of the <see cref="Quote"/> class representing an unknown quote (can be used e.g. when quote with tag was not found).
		/// </summary>
		public static Quote Unknown => new(NoContent, UnknownAuthor);

		Id IQuote.Id => GetId();

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("author", Order = 1, Required = Required.Always)]
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

		/// <inheritdoc/>
		[JsonProperty("date", Order = 2, Required = Required.DisallowNull)]
		[JsonConverter(typeof(DateOnlyConverter))]
		public DateTime? Date { get; init; }

		/// <inheritdoc/>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("source", Order = 3, Required = Required.DisallowNull)]
		public string Source
		{
			get => _source;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_source = value;
			}
		}

		/// <summary>
		/// Tags associated with the quote.
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

		/// <inheritdoc/>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("quote", Order = 0, Required = Required.Always)]
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
		/// Initializes a new instance of the <see cref="Quote"/> class with a <paramref name="quote"/> and an <paramref name="author"/> specified.
		/// </summary>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> is <see langword="null"/> or empty. -or- <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		public Quote(string quote, string author) : this(quote, author, null, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quote"/> class with a <paramref name="quote"/>, <paramref name="author"/> and a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <param name="source">Source of the quote, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> is <see langword="null"/> or empty. -or- <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		public Quote(string quote, string author, string? source) : this(quote, author, source, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quote"/> class with a <paramref name="quote"/>, <paramref name="author"/>, <paramref name="source"/>, <paramref name="date"/> and related <paramref name="tags"/> specified.
		/// </summary>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <param name="source">Source of the quote, e.g. a link, file name or raw text.</param>
		/// <param name="date">Date at which the quote was said/written.</param>
		/// <param name="tags">Tags associated with the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> is <see langword="null"/> or empty. -or- <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		[JsonConstructor]
		public Quote(string quote, string author, string? source, DateTime? date, params string[]? tags)
		{
			if (string.IsNullOrWhiteSpace(quote))
			{
				throw Error.NullOrEmpty(nameof(quote));
			}

			if (string.IsNullOrWhiteSpace(author))
			{
				throw Error.NullOrEmpty(nameof(author));
			}

			_value = quote;
			_author = author;
			_source = source ?? string.Empty;
			Date = date;
			_tags = tags ?? Array.Empty<string>();
		}

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public virtual bool Equals(Quote? other)
		{
			return
				other is not null &&
				other.Value == Value &&
				other.Author == Author &&
				other.Source == Source &&
				other.Date == Date &&
				other.Tags.Length == Tags.Length &&
				other.Tags.SequenceEqual(Tags);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(Author);
			hash.Add(Value);
			hash.Add(Source);
			hash.Add(Date);

			for (int i = 0; i < Tags.Length; i++)
			{
				hash.Add(Tags[i]);
			}

			return hash.ToHashCode();
		}

		/// <summary>
		/// Returns an unique id of the quote.
		/// </summary>
		protected virtual Id GetId()
		{
			return new Id(Value);
		}
	}
}
