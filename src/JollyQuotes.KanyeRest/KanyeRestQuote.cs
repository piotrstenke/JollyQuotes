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
		private const string AUTHOR = "Kanye West";
		private const string SOURCE = KanyeRestResources.ApiPage;

		private readonly Id _id;

		/// <summary>
		/// Text of the quote.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("value", Required = Required.Always)]
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
		/// Initializes a new instance of the <see cref="KanyeRestQuote"/> class with a <paramref name="value"/> specified.
		/// </summary>
		/// <param name="value">Text of the quote.</param>
		/// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or empty.</exception>
		public KanyeRestQuote(string value)
		{
			_id = new(value);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Value;
		}
	}
}
