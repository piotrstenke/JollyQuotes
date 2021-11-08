using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a link.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record LinkModel
	{
		/// <summary>
		/// Actual link.
		/// </summary>
		[JsonProperty("href", Required = Required.Always)]
		public string Href { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LinkModel"/> class with a <paramref name="href"/> specified.
		/// </summary>
		/// <param name="href">Actual link.</param>
		/// <exception cref="ArgumentException"><paramref name="href"/> is null or empty.</exception>
		[JsonConstructor]
		public LinkModel(string href)
		{
			if (string.IsNullOrWhiteSpace(href))
			{
				throw Error.NullOrEmpty(nameof(href));
			}

			Href = href;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Href;
		}
	}
}
