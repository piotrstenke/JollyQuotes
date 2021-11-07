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
		/// Source of the quote, e.g. a link, file name or raw text.
		/// </summary>
		string Source { get; }

		/// <summary>
		/// Actual quote.
		/// </summary>
		string Value { get; }

		/// <summary>
		/// Returns a unique id of the quote. Best used by calling <see cref="object.GetHashCode()"/>.
		/// </summary>
		int GetId();

		/// <summary>
		/// Determines whether the quote can be accessed using the specified <paramref name="tag"/>.
		/// </summary>
		/// <param name="tag">Tag to check whether the quote can be accessed by.</param>
		/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
		bool HasTag(string tag);
	}
}
