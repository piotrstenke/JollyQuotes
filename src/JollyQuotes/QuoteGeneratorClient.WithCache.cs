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

			/// <inheritdoc/>
			public HttpClient BaseClient { get; }

			/// <summary>
			/// Determines whether to dispose the <see cref="BaseClient"/> when <see cref="Dispose()"/> is called.
			/// </summary>
			/// <remarks>The default value is <see langword="true"/>.</remarks>
			public bool DisposeClient { get; set; } = true;

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
			/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
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
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
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
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
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
			/// Releases the resources held by the client.
			/// </summary>
			/// <param name="disposing">Determines whether this method was called directly (<see langword="true"/>) or from destructor (<see langword="false"/>).</param>
			protected virtual void Dispose(bool disposing)
			{
				if (!_disposed)
				{
					if (disposing && DisposeClient)
					{
						BaseClient.Dispose();
					}

					_disposed = true;
				}
			}
		}
	}
}
