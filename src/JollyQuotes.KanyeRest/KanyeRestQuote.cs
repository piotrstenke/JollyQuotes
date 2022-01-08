using System;
using Newtonsoft.Json;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// Represents a quote that is returned by the <c>kanye.rest</c> API.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record KanyeRestQuote : IQuote
	{
		private const string _author = "Kanye West";
		private const string _source = KanyeRestResources.ApiPage;

		private readonly Id _id;

		/// <summary>
		/// Text of the quote.
		/// </summary>
		[JsonProperty("quote", Required = Required.Always)]
		public string Quote
		{
			get => _id.Value;
			init => _id = new(value);
		}

		Id IQuote.Id => _id;
		string IQuote.Author => _author;
		DateTime? IQuote.Date => default;
		string IQuote.Source => _source;
		string IQuote.Value => Quote;
		string[] IQuote.Tags => Array.Empty<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuote"/> class with a <paramref name="quote"/> specified.
		/// </summary>
		/// <param name="quote">Text of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> cannot be <see langword="null"/> or empty.</exception>
		public KanyeRestQuote(string quote)
		{
			if (string.IsNullOrWhiteSpace(quote))
			{
				throw Error.NullOrEmpty(nameof(quote));
			}

			Quote = quote;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Quote;
		}
	}
}
