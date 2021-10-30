using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JollyQuotes
{
	/// <inheritdoc cref="IEnumerableQuoteGenerator"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteGenerator<T> : RandomQuoteGenerator<T>, IEnumerableQuoteGenerator where T : IQuote
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteGenerator{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteGenerator(string source) : base(source)
		{
		}

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes()"/>
		public abstract IEnumerable<T> GetAllQuotes();

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes(string)"/>
		public virtual IEnumerable<T> GetAllQuotes(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			return GetAllQuotes(new string[] { tag });
		}

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes(string[])"/>
		public abstract IEnumerable<T> GetAllQuotes(params string[]? tags);

		IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes()
		{
			return GetAllQuotes().Cast<IQuote>();
		}

		IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes(string tag)
		{
			return GetAllQuotes(tag).Cast<IQuote>();
		}

		IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes(params string[]? tags)
		{
			return GetAllQuotes(tags).Cast<IQuote>();
		}

		IEnumerator<IQuote> IEnumerable<IQuote>.GetEnumerator()
		{
			foreach (T quote in GetAllQuotes())
			{
				yield return quote;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<IQuote>).GetEnumerator();
		}
	}
}
