using System;
using Newtonsoft.Json;

namespace JollyQuotes.KanyeRest
{
	/// <summary>
	/// Represents a quote that is returned by the <c>kanye.rest</c> API.
	/// </summary>
	[JsonObject]
	public sealed record KanyeRestQuote : IQuote
	{
		private const string AUTHOR = "Kanye West";
		private const string SOURCE = KanyeRestResources.ApiPage;

		private readonly Id _id;

		/// <summary>
		/// Text of the quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("quote", Required = Required.Always)]
		public string Value
		{
			get => _id.Value;
			init => _id = new(value);
		}

		Id IQuote.Id => _id;
		string IQuote.Author => AUTHOR;
		DateTime? IQuote.Date => default;
		string IQuote.Source => SOURCE;
		string[] IQuote.Tags => Array.Empty<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestQuote"/> class with a <paramref name="quote"/> specified.
		/// </summary>
		/// <param name="quote">Text of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="quote"/> is <see langword="null"/> or empty.</exception>
		[JsonConstructor]
		public KanyeRestQuote(string quote)
		{
			_id = new(quote);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Value;
		}
	}
}
