using System;

namespace JollyQuotes
{
	/// <summary>
	/// Defines an actual quote along with additional data, such as the quote's author or its source.
	/// </summary>
	public interface IQuote
	{
		/// <summary>
		/// Author of the quote.
		/// </summary>
		string Author { get; }

		/// <summary>
		/// Date at which the quote was said/written.
		/// </summary>
		DateTime? Date { get; }

		/// <summary>
		/// Unique id of the quote.
		/// </summary>
		Id Id { get; }

		/// <summary>
		/// Source of the quote, e.g. a link, file name or raw text.
		/// </summary>
		string Source { get; }

		/// <summary>
		/// Tags that are applicable to this quote.
		/// </summary>
		string[] Tags { get; }

		/// <summary>
		/// Actual text of the quote.
		/// </summary>
		string Value { get; }
	}
}
