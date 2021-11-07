using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// Contains a quote, its author, source, date and related tags.
	/// </summary>
	/// <remarks>This class implements the <see cref="IEquatable{T}"/> interface - two instances are compare by their value, not reference.</remarks>
	[Serializable]
	[JsonObject]
	public sealed record Quote : IQuoteWithTags
	{
		/// <summary>
		/// Value used when an author of a quote is unknown.
		/// </summary>
		public const string UnknownAuthor = "Unknown";

		/// <summary>
		/// Returns a new instance of the <see cref="Quote"/> class representing an unknown quote (can be used e.g. when quote with tag was not found).
		/// </summary>
		public static Quote Unknown => new();

		/// <inheritdoc/>
		[JsonProperty("author", Order = 1, Required = Required.Always)]
		public string Author { get; }

		/// <inheritdoc/>
		[JsonProperty("date", Order = 2)]
		[JsonConverter(typeof(Settings.DateOnlyConverter))]
		public DateTime? Date { get; }

		/// <inheritdoc/>
		[JsonProperty("source", Order = 3, Required = Required.Always)]
		public string Source { get; }

		/// <summary>
		/// Tags associated with the quote.
		/// </summary>
		[JsonProperty("tags", Order = 4, Required = Required.DisallowNull)]
		public string[] Tags { get; }

		/// <inheritdoc/>
		[JsonProperty("quote", Order = 0, Required = Required.Always)]
		public string Value { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Quote"/> class with a <paramref name="quote"/> and an <paramref name="author"/> specified.
		/// </summary>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		public Quote(string quote, string author) : this(quote, author, null, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Quote"/> class with a <paramref name="quote"/>, <paramref name="author"/> and a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <param name="source">Source of the quote, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
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
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		[JsonConstructor]
		public Quote(string quote, string author, string? source, DateTime? date, params string[]? tags)
		{
			if (string.IsNullOrWhiteSpace(quote))
			{
				throw Throw.NullOrEmpty(nameof(quote));
			}

			if (string.IsNullOrWhiteSpace(author))
			{
				throw Throw.NullOrEmpty(nameof(author));
			}

			Value = quote;
			Author = author;
			Source = source ?? string.Empty;
			Date = date;
			Tags = tags ?? Array.Empty<string>();
		}

		private Quote()
		{
			Author = "Unknown";
			Value = string.Empty;
			Source = string.Empty;
			Tags = Array.Empty<string>();
		}

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(Quote? other)
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

		/// <inheritdoc/>
		public override string ToString()
		{
			StringBuilder builder = new();

			builder.Append($"\"{Value}\", {Author}");

			if (Date.HasValue)
			{
				builder.Append($", {Date.Value.ToShortDateString()}");
			}

			return builder.ToString();
		}

		int IQuote.GetId()
		{
			return GetHashCode();
		}
	}
}
