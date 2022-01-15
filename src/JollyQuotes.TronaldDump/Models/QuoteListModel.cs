using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of quotes.
	/// </summary>
	[JsonObject]
	public sealed record QuoteListModel : IEnumerable<QuoteModel>
	{
		private readonly QuoteModel[] _quotes;

		/// <summary>
		/// Quotes this list contains.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("quotes", Order = 0, Required = Required.Always)]
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
				other._quotes.Length == _quotes.Length &&
				other._quotes.SequenceEqual(_quotes);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();
			hash.AddSequence(_quotes);
			return hash.ToHashCode();
		}

		/// <inheritdoc/>
		public IEnumerator<QuoteModel> GetEnumerator()
		{
			return ((IEnumerable<QuoteModel>)_quotes).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _quotes.GetEnumerator();
		}
	}
}
