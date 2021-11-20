﻿using System;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// A <see cref="Quote"/> with a manually-specified id.
	/// </summary>
	[Serializable]
	[JsonObject]
	public record QuoteWithId : Quote
	{
		/// <summary>
		/// Id of the quote.
		/// </summary>
		[JsonProperty("id", Order = -1, Required = Required.Always)]
		public int Id { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteWithId"/> class.
		/// </summary>
		protected QuoteWithId()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteWithId"/> class with a <paramref name="quote"/> and an <paramref name="author"/> specified.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/> must be greater than or equal to <c>0</c>.</exception>
		public QuoteWithId(int id, string quote, string author) : this(id, quote, author, null, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteWithId"/> class with a <paramref name="quote"/>, <paramref name="author"/> and a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <param name="source">Source of the quote, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/> must be greater than or equal to <c>0</c>.</exception>
		public QuoteWithId(int id, string quote, string author, string? source) : this(id, quote, author, source, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteWithId"/> class with a <paramref name="quote"/>, <paramref name="author"/>, <paramref name="source"/>, <paramref name="date"/> and related <paramref name="tags"/> specified.
		/// </summary>
		/// <param name="id">Id of the quote.</param>
		/// <param name="quote">Actual quote.</param>
		/// <param name="author">Author of the quote.</param>
		/// <param name="source">Source of the quote, e.g. a link, file name or raw text.</param>
		/// <param name="date">Date at which the quote was said/written.</param>
		/// <param name="tags">Tags associated with the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> or <paramref name="author"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/> must be greater than or equal to <c>0</c>.</exception>
		[JsonConstructor]
		public QuoteWithId(
			int id,
			string quote,
			string author,
			string? source,
			DateTime? date,
			params string[]? tags
		) : base(quote, author, source, date, tags)
		{
			if (id < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(id), 0);
			}

			Id = id;
		}

		/// <inheritdoc/>
		protected override int GetId()
		{
			return Id;
		}
	}
}
