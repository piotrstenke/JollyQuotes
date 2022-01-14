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
		private readonly string _id;
		private readonly string _url;
		private readonly SelfLinkModel _links;
		private readonly DateTime _updatedAt;

		/// <summary>
		/// Date the quote source was added to the database at.
		/// </summary>
		[JsonProperty("created_at", Order = 2, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Name of file associated with the quote.
		/// </summary>
		[JsonProperty("filename", Order = 4)]
		public string? FileName { get; init; }

		/// <summary>
		/// Id of the quote source.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("quote_source_id", Order = 0, Required = Required.Always)]
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
		/// Link to the quote source data.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_links", Order = 6, Required = Required.Always)]
		public SelfLinkModel Links
		{
			get => _links;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_links = value;
			}
		}

		/// <summary>
		/// Remarks about the quote source.
		/// </summary>
		[JsonProperty("remarks", Order = 5)]
		public string? Remarks { get; init; }

		/// <summary>
		/// Date the quote source was last updated at.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="CreatedAt"/>.</exception>
		[JsonProperty("updated_at", Order = 3, Required = Required.DisallowNull)]
		public DateTime UpdatedAt
		{
			get => _updatedAt;
			init
			{
				if (value < CreatedAt)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(CreatedAt));
				}

				_updatedAt = value;
			}
		}

		/// <summary>
		/// URL of the actual quote source.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("url", Order = 1, Required = Required.Always)]
		public string Url
		{
			get => _url;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_url = value;
			}
		}

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
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater than or equal to <paramref name="createdAt"/>.</exception>
		[JsonConstructor]
		public QuoteSourceModel(
			string id,
			string url,
			string? filename,
			string? remarks,
			SelfLinkModel links,
			DateTime createdAt,
			DateTime updatedAt
		)
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

			if (updatedAt < createdAt)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(updatedAt), nameof(createdAt));
			}

			_id = id;
			_url = url;
			CreatedAt = createdAt;
			_updatedAt = updatedAt;
			FileName = filename;
			Remarks = remarks;
			_links = links;
		}
	}
}
