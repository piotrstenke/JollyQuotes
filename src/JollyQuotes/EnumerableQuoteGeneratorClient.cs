using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace JollyQuotes
{
	/// <inheritdoc cref="IEnumerableQuoteGeneratorClient"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteGeneratorClient<T> : QuoteGeneratorClient<T>, IEnumerableQuoteGeneratorClient where T : IQuote
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
		public virtual IEnumerable<T> GetAllQuotes()
		{
			return GetAllQuotesAsync().ToEnumerable();
		}

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes(string)"/>
		public virtual IEnumerable<T> GetAllQuotes(string tag)
		{
			return GetAllQuotesAsync(tag).ToEnumerable();
		}

		/// <inheritdoc cref="IEnumerableQuoteGenerator.GetAllQuotes(string[])"/>
		public virtual IEnumerable<T> GetAllQuotes(params string[]? tags)
		{
			return GetAllQuotesAsync(tags).ToEnumerable();
		}

		/// <inheritdoc cref="IEnumerableQuoteGeneratorClient.GetAllQuotesAsync()"/>
		public abstract IAsyncEnumerable<T> GetAllQuotesAsync();

		/// <inheritdoc cref="IEnumerableQuoteGeneratorClient.GetAllQuotesAsync(string)"/>
		public virtual IAsyncEnumerable<T> GetAllQuotesAsync(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			return GetAllQuotesAsync(new string[] { tag });
		}

		/// <inheritdoc cref="IEnumerableQuoteGeneratorClient.GetAllQuotesAsync(string[])"/>
		public abstract IAsyncEnumerable<T> GetAllQuotesAsync(params string[]? tags);

		/// <summary>
		/// Returns an <see cref="IAsyncEnumerator{T}"/> that asynchronously iterates through all the available <see cref="IQuote"/>s.
		/// </summary>
		/// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous iteration.</param>
		public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		{
			return GetAllQuotesAsync().GetAsyncEnumerator(cancellationToken);
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

		async IAsyncEnumerable<IQuote> IEnumerableQuoteGeneratorClient.GetAllQuotesAsync()
		{
			await foreach (T quote in GetAllQuotesAsync())
			{
				yield return quote;
			}
		}

		IAsyncEnumerable<IQuote> IEnumerableQuoteGeneratorClient.GetAllQuotesAsync(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			return Yield();

			async IAsyncEnumerable<IQuote> Yield()
			{
				await foreach (T quote in GetAllQuotesAsync(tag))
				{
					yield return quote;
				}
			}
		}

		async IAsyncEnumerable<IQuote> IEnumerableQuoteGeneratorClient.GetAllQuotesAsync(params string[]? tags)
		{
			await foreach (T quote in GetAllQuotesAsync(tags))
			{
				yield return quote;
			}
		}

		async IAsyncEnumerator<IQuote> IAsyncEnumerable<IQuote>.GetAsyncEnumerator(CancellationToken cancellationToken)
		{
			await using IAsyncEnumerator<T> enumerator = GetAsyncEnumerator(cancellationToken);

			while (await enumerator.MoveNextAsync())
			{
				yield return enumerator.Current;
			}
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
