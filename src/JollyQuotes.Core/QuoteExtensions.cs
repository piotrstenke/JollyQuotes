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
		public static Quote ToGeneric(this IQuote quote)
		{
			return new Quote(
				quote.Value,
				quote.Author,
				quote.Source,
				quote.Date,
				quote.Tags
			);
		}

		/// <summary>
		/// Converts the specified <paramref name="quote"/> to a new instance of the <see cref="QuoteWithId"/> class with the given <paramref name="id"/> as parameter.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to convert.</param>
		/// <param name="id">Id to assign to the <paramref name="quote"/>.</param>
		public static QuoteWithId WithId(this IQuote quote, Id id)
		{
			if (quote is QuoteWithId q && q.Id == id)
			{
				return q;
			}

			return new QuoteWithId(
				id,
				quote.Value,
				quote.Author,
				quote.Source,
				quote.Date,
				quote.Tags
			);
		}
	}
}
