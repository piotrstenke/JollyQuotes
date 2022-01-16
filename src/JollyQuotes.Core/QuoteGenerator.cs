using System;

namespace JollyQuotes
{
	/// <inheritdoc cref="IQuoteGenerator"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class QuoteGenerator<T> : IQuoteGenerator where T : class, IQuote
	{
		/// <inheritdoc/>
		public abstract string ApiName { get; }

		/// <summary>
		/// Source of the quotes, e.g. a link, file name or raw text.
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteGenerator{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected QuoteGenerator(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			Source = source;
		}

		/// <inheritdoc cref="IQuoteGenerator.GetRandomQuote()"/>
		public abstract T GetRandomQuote();

		/// <inheritdoc cref="IQuoteGenerator.GetRandomQuote(string)"/>
		public abstract T? GetRandomQuote(string tag);

		/// <inheritdoc cref="IQuoteGenerator.GetRandomQuote(string[])"/>
		public abstract T? GetRandomQuote(params string[]? tags);

		IQuote IQuoteGenerator.GetRandomQuote()
		{
			return GetRandomQuote();
		}

		IQuote? IQuoteGenerator.GetRandomQuote(string tag)
		{
			return GetRandomQuote(tag);
		}

		IQuote? IQuoteGenerator.GetRandomQuote(params string[]? tags)
		{
			return GetRandomQuote(tags);
		}
	}
}
