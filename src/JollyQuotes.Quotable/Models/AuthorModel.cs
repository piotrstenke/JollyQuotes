using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Represents an author of a quote.
	/// </summary>
	[JsonObject]
	public sealed record AuthorModel : DatabaseModel
	{
		private readonly string _id;
		private readonly string _name;
		private readonly string _slug;
		private readonly int _quoteCount;

		/// <summary>
		/// Id of the author.
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
		/// Full name of the author.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("name", Order = 1, Required = Required.Always)]
		public string Name
		{
			get => _name;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_name = value;
			}
		}

		/// <summary>
		/// Slug version of the author's <see cref="Name"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("slug", Order = 2, Required = Required.Always)]
		public string Slug
		{
			get => _slug;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_slug = value;
			}
		}

		/// <summary>
		/// Link to the authors wikipedia page or official website.
		/// </summary>
		[JsonProperty("link", Order = 3)]
		public string? Link { get; init; }

		/// <summary>
		/// Brief, one paragraph bio of the author.
		/// </summary>
		[JsonProperty("bio", Order = 4)]
		public string? Bio { get; init; }

		/// <summary>
		/// A one-line description of the author.Typically it is the person's primary occupation or what they are know for.
		/// </summary>
		[JsonProperty("description", Order = 5)]
		public string? Description { get; init; }

		/// <summary>
		/// Number of quotes by this author.
		/// </summary>
		[JsonProperty("quoteCount", Order = 6)]
		public int QuoteCount
		{
			get => _quoteCount;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_quoteCount = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorModel"/> class with author's <paramref name="id"/>, <paramref name="name"/>, <paramref name="slug"/>, <paramref name="quoteCount"/> and date of creation specified.
		/// </summary>
		/// <param name="id">Id of the author.</param>
		/// <param name="name">Full name of the author.</param>
		/// <param name="slug">Slug version of the author's <paramref name="name"/>.</param>
		/// <param name="quoteCount">Number of quotes by this author.</param>
		/// <param name="dateAdded">Date the author was added at.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="slug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="quoteCount"/> must be greater than or equal to <c>0</c>.</exception>
		public AuthorModel(
			string id,
			string name,
			string slug,
			int quoteCount,
			DateTime dateAdded
		) : this(id, name, slug, quoteCount, dateAdded, dateAdded)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorModel"/> class with author's <paramref name="id"/>, <paramref name="name"/>, <paramref name="slug"/>, <paramref name="quoteCount"/> and dates of creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the author.</param>
		/// <param name="name">Full name of the author.</param>
		/// <param name="slug">Slug version of the author's <paramref name="name"/>.</param>
		/// <param name="quoteCount">Number of quotes by this author.</param>
		/// <param name="dateAdded">Date the author was added at.</param>
		/// <param name="dateModified">Date of the author's most recent update.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="slug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="quoteCount"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="dateModified"/> must be greater or equal to <paramref name="dateAdded"/>.
		/// </exception>
		public AuthorModel(
			string id,
			string name,
			string slug,
			int quoteCount,
			DateTime dateAdded,
			DateTime dateModified
		) : this(id, name, slug, quoteCount, dateAdded, dateModified, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorModel"/> class with author's <paramref name="id"/>, <paramref name="name"/>, <paramref name="slug"/>, <paramref name="quoteCount"/>, <paramref name="bio"/>,
		/// <paramref name="description"/>, <paramref name="link"/> and dates of creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the author.</param>
		/// <param name="name">Full name of the author.</param>
		/// <param name="slug">Slug version of the author's <paramref name="name"/>.</param>
		/// <param name="quoteCount">Number of quotes by this author.</param>
		/// <param name="dateAdded">Date the author was added at.</param>
		/// <param name="dateModified">Date of the author's most recent update.</param>
		/// <param name="link">Link to the authors wikipedia page or official website.</param>
		/// <param name="bio">Brief, one paragraph bio of the author.</param>
		/// <param name="description">A one-line description of the author.Typically it is the person's primary occupation or what they are know for.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="slug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="quoteCount"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="dateModified"/> must be greater or equal to <paramref name="dateAdded"/>.</exception>
		[JsonConstructor]
		public AuthorModel(
			string id,
			string name,
			string slug,
			int quoteCount,
			DateTime dateAdded,
			DateTime dateModified,
			string? link = default,
			string? bio = default,
			string? description = default
		) : base(dateAdded, dateModified)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if (string.IsNullOrWhiteSpace(slug))
			{
				throw Error.NullOrEmpty(nameof(slug));
			}

			if (quoteCount < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(quoteCount), 0);
			}

			_id = id;
			_name = name;
			_slug = slug;
			_quoteCount = quoteCount;
			Link = link;
			Bio = bio;
			Description = description;
		}
	}
}
