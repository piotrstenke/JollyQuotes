using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JollyQuotes
{
	/// <inheritdoc cref="IEnumerableQuoteGenerator"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteGenerator<T> : QuoteGenerator<T>, IEnumerableQuoteGenerator where T : class, IQuote
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
		public abstract IEnumerable<T> GetAllQuotes(string tag);

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes(string[])"/>
		public virtual IEnumerable<T> GetAllQuotes(params string[]? tags)
		{
			if (tags is null || tags.Length == 0)
			{
				return Array.Empty<T>();
			}

			return Yield();

			IEnumerable<T> Yield()
			{
				foreach (string tag in tags)
				{
					if (string.IsNullOrWhiteSpace(tag))
					{
						continue;
					}

					foreach (T quote in GetAllQuotes(tag))
					{
						yield return quote;
					}
				}
			}
		}

		/// <summary>
		/// Returns an <see cref="IEnumerator{T}"/> that iterates through all the available <see cref="IQuote"/>s.
		/// </summary>
		public virtual IEnumerator<T> GetEnumerator()
		{
			return GetAllQuotes().GetEnumerator();
		}

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
			return GetEnumerator();
		}
	}
}
