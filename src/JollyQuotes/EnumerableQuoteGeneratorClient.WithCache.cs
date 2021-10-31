using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace JollyQuotes
{
	public abstract partial class EnumerableQuoteGeneratorClient<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IEnumerableQuoteGeneratorClient"/> that provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : QuoteGeneratorClient<T>.WithCache, IEnumerableQuoteGeneratorClient
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified.
			/// </summary>
			/// <param name="source">Base address of the underlaying quote API.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>. -or- <paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
			protected WithCache(string source, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(source, cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a specified <paramref name="uri"/>
			/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="QuoteGeneratorClient{T}.WithCache.BaseClient"/>.
			/// </summary>
			/// <param name="uri">
			/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="QuoteGeneratorClient{T}.WithCache.BaseClient"/>.
			/// </param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>. -or- <paramref name="cache"/> is <see langword="null"/>. -or- <paramref name="possibility"/> is <see langword="null"/>.</exception>
			protected WithCache(Uri uri, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(uri, cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="client"/> specified.
			/// </summary>
			/// <param name="client">Underlaying client that is used to access required resources.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="cache"/> is <see langword="null"/>. -or- <paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty.</exception>
			protected WithCache(HttpClient client, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(client, cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
			/// </summary>
			/// <param name="client">Underlaying client that is used to access required resources.</param>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>. -or- <paramref name="cache"/> is <see langword="null"/>. -or- <paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(HttpClient client, string source, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(client, source, cache, possibility)
			{
			}

			/// <summary>
			/// Returns a collection of all possible quotes.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public virtual IEnumerable<T> GetAllQuotes(QuoteInclude which = QuoteInclude.All)
			{
				return GetAllQuotesAsync(which).ToEnumerable();
			}

			/// <summary>
			/// Returns a collection of all possible quotes associated with the specified <paramref name="tag"/>.
			/// </summary>
			/// <param name="tag">Tag to get all <see cref="IQuote"/>s associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			public IEnumerable<T> GetAllQuotes(string tag, QuoteInclude which = QuoteInclude.All)
			{
				return GetAllQuotesAsync(tag, which).ToEnumerable();
			}

			/// <summary>
			/// Returns a collection of all possible quotes associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to get all <see cref="IQuote"/> associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerable<T> GetAllQuotes(string[]? tags, QuoteInclude which = QuoteInclude.All)
			{
				return GetAllQuotesAsync(tags, which).ToEnumerable();
			}

			/// <summary>
			/// Returns an asynchronous collection of all possible quotes.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public IAsyncEnumerable<T> GetAllQuotesAsync(QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached().ToAsyncEnumerable().Concat(DownloadAllQuotesAsync());

					case QuoteInclude.Download:
						return DownloadAllQuotesAsync();

					case QuoteInclude.Cached:
						return Cache.GetCached().ToAsyncEnumerable();

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Returns an asynchronous collection of all possible quotes associated with the specified <paramref name="tag"/>.
			/// </summary>
			/// <param name="tag">Tag to get all <see cref="IQuote"/>s associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			/// <param name="which">Determines which quotes to include.</param>
			public IAsyncEnumerable<T> GetAllQuotesAsync(string tag, QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached(tag).ToAsyncEnumerable().Concat(DownloadAllQuotesAsync(tag));

					case QuoteInclude.Download:
						return DownloadAllQuotesAsync(tag);

					case QuoteInclude.Cached:
						return Cache.GetCached(tag).ToAsyncEnumerable();

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Returns an asynchronous collection of all possible quotes associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to get all <see cref="IQuote"/> associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public IAsyncEnumerable<T> GetAllQuotesAsync(string[]? tags, QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached(tags).ToAsyncEnumerable().Concat(DownloadAllQuotesAsync(tags));

					case QuoteInclude.Download:
						return DownloadAllQuotesAsync(tags);

					case QuoteInclude.Cached:
						return Cache.GetCached(tags).ToAsyncEnumerable();

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Returns an <see cref="IAsyncEnumerator{T}"/> that asynchronously iterates through all the available <see cref="IQuote"/>s.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			/// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous iteration.</param>
			public IAsyncEnumerator<T> GetAsyncEnumerator(QuoteInclude which = QuoteInclude.All, CancellationToken cancellationToken = default)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return YieldAll();

					case QuoteInclude.Download:
						return DownloadAndAsyncEnumerate(cancellationToken);

					case QuoteInclude.Cached:
						return Cache.ToAsyncEnumerable().GetAsyncEnumerator(cancellationToken);

					default:
						goto case QuoteInclude.All;
				}

				async IAsyncEnumerator<T> YieldAll()
				{
					foreach (T quote in Cache)
					{
						yield return quote;
					}

					await using IAsyncEnumerator<T> enumerator = DownloadAndAsyncEnumerate(cancellationToken);

					while (await enumerator.MoveNextAsync())
					{
						yield return enumerator.Current;
					}
				}
			}

			/// <summary>
			/// Returns an <see cref="IEnumerator{T}"/> that iterates through all the available <see cref="IQuote"/>s.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerator<T> GetEnumerator(QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return YieldAll();

					case QuoteInclude.Download:
						return DownloadAndEnumerate();

					case QuoteInclude.Cached:
						return Cache.GetEnumerator();

					default:
						goto case QuoteInclude.All;
				}

				IEnumerator<T> YieldAll()
				{
					foreach (T quote in Cache)
					{
						yield return quote;
					}

					using IEnumerator<T> enumerator = DownloadAndEnumerate();

					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
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
				await using IAsyncEnumerator<T> enumerator = GetAsyncEnumerator(cancellationToken: cancellationToken);

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

			/// <summary>
			/// Downloads all quotes available from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>
			/// </summary>
			protected virtual IEnumerable<T> DownloadAllQuotes()
			{
				return DownloadAllQuotesAsync().ToEnumerable();
			}

			/// <summary>
			/// Downloads all quotes associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download all quotes associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual IEnumerable<T> DownloadAllQuotes(string tag)
			{
				return DownloadAllQuotesAsync(tag).ToEnumerable();
			}

			/// <summary>
			/// Downloads all quotes associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download all quotes associated with.</param>
			protected virtual IEnumerable<T> DownloadAllQuotes(params string[]? tags)
			{
				return DownloadAllQuotesAsync(tags).ToEnumerable();
			}

			/// <summary>
			/// Asynchronously downloads all quotes available from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>
			/// </summary>
			protected abstract IAsyncEnumerable<T> DownloadAllQuotesAsync();

			/// <summary>
			/// Asynchronously downloads all quotes associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download all quotes associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual IAsyncEnumerable<T> DownloadAllQuotesAsync(string tag)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Internals.NullOrEmpty(nameof(tag));
				}

				return DownloadAllQuotesAsync(new string[] { tag });
			}

			/// <summary>
			/// Asynchronously downloads all quotes associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download all quotes associated with.</param>
			protected abstract IAsyncEnumerable<T> DownloadAllQuotesAsync(params string[]? tags);

			/// <summary>
			/// Downloads all available <see cref="IQuote"/>s and asynchronously enumerates them.
			/// </summary>
			/// <param name="cancellationToken">A <see cref="CancellationToken"/> that may be used to cancel the asynchronous iteration.</param>
			protected virtual IAsyncEnumerator<T> DownloadAndAsyncEnumerate(CancellationToken cancellationToken = default)
			{
				return DownloadAllQuotesAsync().GetAsyncEnumerator(cancellationToken);
			}

			/// <summary>
			/// Downloads all available <see cref="IQuote"/>s and enumerates them.
			/// </summary>
			protected virtual IEnumerator<T> DownloadAndEnumerate()
			{
				return DownloadAllQuotes().GetEnumerator();
			}
		}
	}
}
