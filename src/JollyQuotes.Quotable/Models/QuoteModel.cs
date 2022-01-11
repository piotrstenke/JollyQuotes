using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Represents a quote that was created from raw data returned by the <c>quotable</c> API.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteModel
	{
		private readonly string[] _tags;
		private readonly string _id;
		private readonly string _content;
		private readonly string _author;
		private readonly string _authorSlug;
		private readonly int _length;

		/// <summary>
		/// Id of the quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("_id", Order = 0, Required = Required.Always)]
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
		/// Actual text of the quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("content", Order = 1, Required = Required.Always)]
		public string Content
		{
			get => _content;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_content = value;
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
		/// Length of the quote's text in characters.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>.</exception>
		[JsonProperty("length", Order = 5, Required = Required.Always)]
		public int Length
		{
			get => _length;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_length = value;
			}
		}

		/// <summary>
		/// Date the quote was added at.
		/// </summary>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateAdded", Order = 6, Required = Required.Always)]
		public DateTime DateAdded { get; init; }

		/// <summary>
		/// Date of the quote's most recent update.
		/// </summary>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateModified", Order = 7)]
		public DateTime DateModified { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteModel"/> class with an <paramref name="id"/>, actual <paramref name="content"/>, <paramref name="author"/>, <paramref name="authorSlug"/>, associated <paramref name="tags"/> and date of the quote's creation.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="content">Text of the quote.</param>
		/// <param name="author">Full name of the quote's author.</param>
		/// <param name="authorSlug">Slug version of the <paramref name="author"/>.</param>
		/// <param name="tags">Tags associated with this quote.</param>
		/// <param name="dateAdded">Date the quote was added at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="content"/> is <see langword="null"/> or empty.
		/// <paramref name="author"/> is <see langword="null"/> or empty.
		/// <paramref name="authorSlug"/> is <see langword="null"/> or empty.
		/// </exception>
		public QuoteModel(
			string id,
			string content,
			string author,
			string authorSlug,
			string[] tags,
			DateTime dateAdded
		) : this(id, content, author, authorSlug, tags, dateAdded, dateAdded)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteModel"/> class with an <paramref name="id"/>, actual <paramref name="content"/>, <paramref name="author"/>, <paramref name="authorSlug"/>, associated <paramref name="tags"/> and dates of creation and last update.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="content">Text of the quote.</param>
		/// <param name="author">Full name of the quote's author.</param>
		/// <param name="authorSlug">Slug version of the <paramref name="author"/>.</param>
		/// <param name="tags">Tags associated with this quote.</param>
		/// <param name="dateAdded">Date the quote was added at.</param>
		/// <param name="dateModified">Date of the quote's most recent update.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="content"/> is <see langword="null"/> or empty.
		/// <paramref name="author"/> is <see langword="null"/> or empty.
		/// <paramref name="authorSlug"/> is <see langword="null"/> or empty.
		/// </exception>
		public QuoteModel(
			string id,
			string content,
			string author,
			string authorSlug,
			string[] tags,
			DateTime dateAdded,
			DateTime dateModified
		) : this(id, content, author, authorSlug, tags, dateAdded, dateModified, GetQuoteLength(content))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteModel"/> class with an <paramref name="id"/>, actual <paramref name="content"/>, <paramref name="author"/>, <paramref name="authorSlug"/>, associated <paramref name="tags"/>, <paramref name="length"/> of the quote and dates of creation and last update.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="content">Text of the quote.</param>
		/// <param name="author">Full name of the quote's author.</param>
		/// <param name="authorSlug">Slug version of the <paramref name="author"/>.</param>
		/// <param name="tags">Tags associated with this quote.</param>
		/// <param name="dateAdded">Date the quote was added at.</param>
		/// <param name="dateModified">Date of the quote's most recent update.</param>
		/// <param name="length">Length of the quote's text in characters.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="content"/> is <see langword="null"/> or empty.
		/// <paramref name="author"/> is <see langword="null"/> or empty.
		/// <paramref name="authorSlug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="length"/> must be greater than or equal to <c>0</c>.</exception>
		[JsonConstructor]
		public QuoteModel(
			string id,
			string content,
			string author,
			string authorSlug,
			string[] tags,
			DateTime dateAdded,
			DateTime dateModified,
			int length
		)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(content))
			{
				throw Error.NullOrEmpty(nameof(content));
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

			if (length < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(length), 0);
			}

			_id = id;
			_content = content;
			_author = author;
			_authorSlug = authorSlug;
			_length = length;
			_tags = tags;
			DateAdded = dateAdded;
			DateModified = dateModified;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		private static int GetQuoteLength(string content)
		{
			if (content is null)
			{
				throw Error.Null(nameof(content));
			}

			return content.Length;
		}
	}
}
