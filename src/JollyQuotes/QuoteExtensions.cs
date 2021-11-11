namespace JollyQuotes
{
	/// <summary>
	/// Contains various utility extension methods widely used through the <c>JollyQuotes</c> libraries.
	/// </summary>
	public static class QuoteExtensions
	{
		/// <summary>
		/// Converts the specified <paramref name="quote"/> to a new instance of the <see cref="Quote"/> class.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to convert.</param>
		public static Quote ToGenericQuote(this IQuote quote)
		{
			return new Quote(
				quote.Value,
				quote.Author,
				quote.Source,
				quote.Date,
				quote.Tags
			);
		}
	}
}
