using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a source of a quote (e.g. a twit, article or interview).
	/// </summary>
	[JsonObject]
	public sealed record QuoteSourceModel
	{
		/// <summary>
		/// Date the quote source was added to the database at.
		/// </summary>
		[JsonProperty("created_at", Order = 2, Required = Required.Always)]
		public DateTime CreatedAt { get; }

		/// <summary>
		/// Name of file associated with the quote.
		/// </summary>
		[JsonProperty("filename", Order = 4)]
		public string? FileName { get; }

		/// <summary>
		/// Id of the quote source.
		/// </summary>
		[JsonProperty("quote_source_id", Order = 0, Required = Required.Always)]
		public string Id { get; }

		/// <summary>
		/// Link to the quote source data.
		/// </summary>
		[JsonProperty("_links", Order = 6, Required = Required.Always)]
		public SelfLinkModel Links { get; }

		/// <summary>
		/// Remarks about the quote source.
		/// </summary>
		[JsonProperty("remarks", Order = 5)]
		public string? Remarks { get; }

		/// <summary>
		/// Date the quote source was last updated at.
		/// </summary>
		[JsonProperty("updated_at", Order = 3, Required = Required.DisallowNull)]
		public DateTime UpdatedAt { get; }

		/// <summary>
		/// URL of the actual quote source.
		/// </summary>
		[JsonProperty("url", Order = 1, Required = Required.Always)]
		public string Url { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSourceModel"/> class with <paramref name="id"/>, target <paramref name="url"/>, <paramref name="filename"/>, <paramref name="remarks"/> and date of creation specified.
		/// </summary>
		/// <param name="id">Id of the quote source.</param>
		/// <param name="url">URL of the actual quote source.</param>
		/// <param name="filename">Name of file associated with the quote.</param>
		/// <param name="remarks">Remarks about the quote source.</param>
		/// <param name="links">Link to the quote source data.</param>
		/// <param name="createdAt">Date the quote source was added to the database at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty. -or- <paramref name="url"/> is <see langword="null"/> or empty.</exception>
		public QuoteSourceModel(
			string id,
			string url,
			string? filename,
			string? remarks,
			SelfLinkModel links,
			DateTime createdAt
		) : this(id, url, filename, remarks, links, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSourceModel"/> class with <paramref name="id"/>, target <paramref name="url"/>, <paramref name="filename"/>, <paramref name="remarks"/> and dates of creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the quote source.</param>
		/// <param name="url">URL of the actual quote source.</param>
		/// <param name="filename">Name of file associated with the quote.</param>
		/// <param name="remarks">Remarks about the quote source.</param>
		/// <param name="links">Link to the quote source data.</param>
		/// <param name="createdAt">Date the quote source was added to the database at.</param>
		/// <param name="updatedAt">Date the quote source was last updated at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="id"/> is <see langword="null"/> or empty. -or- <paramref name="url"/> is <see langword="null"/> or empty.</exception>
		[JsonConstructor]
		public QuoteSourceModel(
			string id,
			string url,
			string? filename,
			string? remarks,
			SelfLinkModel links,
			DateTime createdAt,
			DateTime updatedAt)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(url))
			{
				throw Error.NullOrEmpty(nameof(url));
			}

			if (links is null)
			{
				throw Error.Null(nameof(links));
			}

			Id = id;
			Url = url;
			CreatedAt = createdAt;
			UpdatedAt = updatedAt;
			FileName = filename;
			Remarks = remarks;
			Links = links;
		}
	}
}
