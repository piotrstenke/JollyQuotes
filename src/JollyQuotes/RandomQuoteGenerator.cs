using System;

namespace JollyQuotes
{
	/// <inheritdoc cref="IRandomQuoteGenerator"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class RandomQuoteGenerator<T> : IRandomQuoteGenerator where T : IQuote
	{
		/// <summary>
		/// Source of the quotes, e.g. a link, file name or raw text.
		/// </summary>
		public string Source { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomQuoteGenerator{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected RandomQuoteGenerator(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Throw.NullOrEmpty(nameof(source));
			}

			Source = source;
		}

		/// <inheritdoc cref="IRandomQuoteGenerator.GetRandomQuote()"/>
		public abstract T GetRandomQuote();

		/// <inheritdoc cref="IRandomQuoteGenerator.GetRandomQuote(string)"/>
		public abstract T? GetRandomQuote(string tag);

		/// <inheritdoc cref="IRandomQuoteGenerator.GetRandomQuote(string[])"/>
		public virtual T? GetRandomQuote(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return default;
			}

			foreach (string tag in tags)
			{
				if (GetRandomQuote(tag) is T t)
				{
					return t;
				}
			}

			return default;
		}

		IQuote IRandomQuoteGenerator.GetRandomQuote()
		{
			return GetRandomQuote();
		}

		IQuote? IRandomQuoteGenerator.GetRandomQuote(string tag)
		{
			return GetRandomQuote(tag);
		}

		IQuote? IRandomQuoteGenerator.GetRandomQuote(params string[]? tags)
		{
			return GetRandomQuote(tags);
		}
	}
}
