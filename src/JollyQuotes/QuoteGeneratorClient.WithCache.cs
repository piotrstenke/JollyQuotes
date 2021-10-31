using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes
{
	public abstract partial class QuoteGeneratorClient<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IQuoteGeneratorClient"/> that provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : RandomQuoteGenerator<T>.WithCache, IQuoteGeneratorClient, IDisposable
		{
			private bool _disposed;
			private Task<T?>? _emptyTask;

			/// <inheritdoc/>
			public HttpClient BaseClient { get; }

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
			protected WithCache(string source, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(
				source,
				cache ?? GetDefaultQuoteCache(),
				possibility ?? GetDefaultPossibility()
			)
			{
				BaseClient = new()
				{
					BaseAddress = new Uri(source)
				};
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a specified <paramref name="uri"/>
			/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
			/// </summary>
			/// <param name="uri">
			/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
			/// </param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>. -or- <paramref name="cache"/> is <see langword="null"/>. -or- <paramref name="possibility"/> is <see langword="null"/>.</exception>
			protected WithCache(Uri uri, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(
				RetrieveSourceFromUri(uri),
				cache ?? GetDefaultQuoteCache(),
				possibility ?? GetDefaultPossibility()
			)
			{
				BaseClient = new()
				{
					BaseAddress = uri
				};
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
			protected WithCache(HttpClient client, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(
				RetrieveSourceFromClient(client),
				cache ?? GetDefaultQuoteCache(),
				possibility ?? GetDefaultPossibility()
			)
			{
				BaseClient = client;
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
			protected WithCache(HttpClient client, string source, BlockableQuoteCache<T>? cache = null, IPossibility? possibility = null) : base(
				source,
				cache ?? GetDefaultQuoteCache(),
				possibility ?? GetDefaultPossibility()
			)
			{
				if (client is null)
				{
					throw Internals.Null(nameof(client));
				}

				BaseClient = client;
			}

			/// <summary>
			/// Destructs the <see cref="WithCache"/> class.
			/// </summary>
			~WithCache()
			{
				Dispose(false);
			}

			/// <summary>
			/// Releases the resources held by the current instance.
			/// </summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Asynchronously generates a random quote.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public Task<T> GetRandomQuoteAsync(QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:

						if (Cache.IsEmpty || Possibility.Determine())
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							goto case QuoteInclude.Cached;
						}

					case QuoteInclude.Download:
						return DownloadRandomQuoteAsync();

					case QuoteInclude.Cached:
						return Task.FromResult(Cache.GetRandomQuote());

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Asynchronously generates a random quote associated with the specified tag.
			/// </summary>
			/// <param name="tag">Tag to generate a quote associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			public Task<T?> GetRandomQuoteAsync(string tag, QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
					{
						if (Cache.IsEmpty || Possibility.Determine() || !Cache.TryGetRandomQuote(tag, out T? quote))
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							return Task.FromResult(quote)!;
						}
					}

					case QuoteInclude.Cached:
					{
						if (Cache.TryGetRandomQuote(tag, out T? quote))
						{
							return Task.FromResult(quote)!;
						}

						return GetEmptyTask();
					}

					case QuoteInclude.Download:
						return DownloadRandomQuoteAsync(tag)!;

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Asynchronously generates a random quote associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to generate a quote associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public Task<T?> GetRandomQuoteAsync(string[]? tags, QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:

						if (Cache.IsEmpty || Possibility.Determine() || GetQuoteFromCache() is not T quote)
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							return Task.FromResult(quote)!;
						}

					case QuoteInclude.Cached:
						return Task.FromResult(GetQuoteFromCache());

					case QuoteInclude.Download:
						return DownloadRandomQuoteAsync(tags)!;

					default:
						goto case QuoteInclude.All;
				}

				T? GetQuoteFromCache()
				{
					if (tags is null)
					{
						return default;
					}

					foreach (string tag in tags)
					{
						if (Cache.TryGetRandomQuote(tag, out T? quote))
						{
							return quote;
						}
					}

					return default;
				}
			}

			Task<IQuote> IQuoteGeneratorClient.GetRandomQuoteAsync()
			{
				return GetRandomQuoteAsync().ContinueWith(t => t.Result as IQuote);
			}

			Task<IQuote?> IQuoteGeneratorClient.GetRandomQuoteAsync(params string[]? tags)
			{
				return GetRandomQuoteAsync(tags).ContinueWith(t => t.Result as IQuote);
			}

			Task<IQuote?> IQuoteGeneratorClient.GetRandomQuoteAsync(string tag)
			{
				return GetRandomQuoteAsync(tag).ContinueWith(t => t.Result as IQuote);
			}

			/// <summary>
			/// Releases the resources held by the client.
			/// </summary>
			/// <param name="disposing">Determines whether this method was called directly (<see langword="true"/>) or from destructor (<see langword="false"/>).</param>
			protected virtual void Dispose(bool disposing)
			{
				if (!_disposed)
				{
					if (disposing)
					{
						BaseClient.Dispose();
					}

					_disposed = true;
				}
			}

			/// <inheritdoc/>
			protected override T DownloadRandomQuote()
			{
				return DownloadRandomQuoteAsync().Result;
			}

			/// <inheritdoc/>
			protected override T? DownloadRandomQuote(string tag)
			{
				return DownloadRandomQuoteAsync(tag).Result;
			}

			/// <inheritdoc/>
			protected override T? DownloadRandomQuote(params string[]? tags)
			{
				return DownloadRandomQuoteAsync(tags).Result;
			}

			/// <summary>
			/// Asynchronously downloads a quote from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			protected abstract Task<T> DownloadRandomQuoteAsync();

			/// <summary>
			/// Asynchronously downloads a quote associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download a quote associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual Task<T?> DownloadRandomQuoteAsync(string tag)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Internals.NullOrEmpty(nameof(tag));
				}

				return DownloadRandomQuoteAsync(new string[] { tag });
			}

			/// <summary>
			/// Asynchronously downloads a random quote associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download a quote associated with.</param>
			protected abstract Task<T?> DownloadRandomQuoteAsync(string[]? tags);

			/// <summary>
			/// Returns a <see cref="Task{TResult}"/> with the <see cref="Task{TResult}.Result"/> set to <see langword="default"/>(<typeparamref name="T"/>).
			/// </summary>
			protected Task<T?> GetEmptyTask()
			{
				if (_emptyTask is null)
				{
					_emptyTask = Task.FromResult(default(T));
				}

				return _emptyTask;
			}
		}
	}
}
