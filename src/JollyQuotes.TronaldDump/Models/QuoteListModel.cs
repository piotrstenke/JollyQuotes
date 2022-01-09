﻿using System;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of quotes.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteListModel
	{
		private readonly QuoteModel[] _quotes;

		/// <summary>
		/// Quotes this list contains.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("quotes", Required = Required.Always)]
		public QuoteModel[] Quotes
		{
			get => _quotes;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_quotes = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteListModel"/> class with an array of underlaying <paramref name="quotes"/> specified.
		/// </summary>
		/// <param name="quotes">Quotes this list contains.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quotes"/> is <see langword="null"/>.</exception>
		[JsonConstructor]
		public QuoteListModel(QuoteModel[] quotes)
		{
			if (quotes is null)
			{
				throw Error.Null(nameof(quotes));
			}

			_quotes = quotes;
		}

		/// <inheritdoc/>
		public bool Equals(QuoteListModel? other)
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
				other.Quotes.Length == Quotes.Length &&
				other.Quotes.SequenceEqual(Quotes);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			foreach (QuoteModel quote in Quotes)
			{
				hash.Add(quote);
			}

			return hash.ToHashCode();
		}
	}
}
