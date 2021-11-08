﻿using System;
using System.Linq;
using System.Text;
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
		/// <summary>
		/// Quotes this list contains.
		/// </summary>
		[JsonProperty("quotes", Required = Required.Always)]
		public QuoteModel[] Quotes { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteListModel"/> class with an array of underlaying <paramref name="quotes"/> specified.
		/// </summary>
		/// <param name="quotes">Quotes this list contains.</param>
		/// <exception cref="ArgumentNullException"><paramref name="quotes"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="quotes"/> is empty.</exception>
		[JsonConstructor]
		public QuoteListModel(QuoteModel[] quotes)
		{
			if (quotes is null)
			{
				throw Error.Null(nameof(quotes));
			}

			if (quotes.Length == 0)
			{
				throw Error.Empty(nameof(quotes));
			}

			Quotes = quotes;
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

#pragma warning disable IDE0051 // Remove unused private members
		private bool PrintMembers(StringBuilder builder)
#pragma warning restore IDE0051 // Remove unused private members
		{
			Internals.PrintArray(builder, nameof(Quotes), Quotes);
			return true;
		}
	}
}
