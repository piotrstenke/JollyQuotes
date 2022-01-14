using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Contains data needed to perform a quote search.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record QuoteSearchModel
	{
		private readonly int _page;

		/// <summary>
		/// Phrases to search by.
		/// </summary>
		[JsonProperty("phrases", Order = 0)]
		public string[]? Phrases { get; init; }

		/// <summary>
		/// Tag to search by.
		/// </summary>
		[JsonProperty("tag", Order = 1)]
		public string? Tag { get; init; }

		/// <summary>
		/// The current page of the search result.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>.</exception>
		[JsonProperty("page", Order = 2)]
		public int Page
		{
			get => _page;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_page = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class.
		/// </summary>
		public QuoteSearchModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with target <paramref name="phrases"/>
		/// </summary>
		/// <param name="phrases">Phrases to search by.</param>
		/// <param name="tag">Tag to search by.</param>
		/// <param name="page">The current page of the search result.</param>
		[JsonConstructor]
		public QuoteSearchModel(string[]? phrases = default, string? tag = default, int page = default)
		{
			if (page < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(page), 0);
			}

			Phrases = phrases;
			Tag = tag;
			_page = page;
		}
	}
}
