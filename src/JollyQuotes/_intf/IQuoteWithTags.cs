using System;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IQuote"/> with an array of applicable tags specified.
	/// </summary>
	public interface IQuoteWithTags : IQuote
	{
		/// <summary>
		/// Tags that are applicable to this quote.
		/// </summary>
		string[] Tags { get; }

		/// <inheritdoc/>
		bool IQuote.HasTag(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Throw.NullOrEmpty(nameof(tag));
			}

			return Array.IndexOf(Tags, tag) != -1;
		}
	}
}
