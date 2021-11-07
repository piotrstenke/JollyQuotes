using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that combines functionality of the <see cref="IEnumerableQuoteGenerator"/> and <see cref="IQuoteGeneratorClient"/> interfaces.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteGeneratorClient<T> : QuoteGeneratorClient<T>, IEnumerableQuoteGenerator where T : IQuote
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteGeneratorClient{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
		protected EnumerableQuoteGeneratorClient(string source) : base(source)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteGeneratorClient{T}"/> class with a specified <paramref name="uri"/>
		/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="QuoteGeneratorClient{T}.BaseClient"/>.
		/// </summary>
		/// <param name="uri">
		/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="QuoteGeneratorClient{T}.BaseClient"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
		protected EnumerableQuoteGeneratorClient(Uri uri) : base(uri)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteGeneratorClient{T}"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteGeneratorClient(HttpClient client) : base(client)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteGeneratorClient{T}"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="source"></param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteGeneratorClient(HttpClient client, string source) : base(client, source)
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
