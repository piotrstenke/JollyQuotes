using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <inheritdoc cref="IQuoteGeneratorClient"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class QuoteGeneratorClient<T> : EnumerableQuoteGenerator<T>, IQuoteGeneratorClient, IDisposable where T : IQuote
	{
		private bool _disposed;

		/// <inheritdoc/>
		public HttpClient BaseClient { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteGeneratorClient{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
		protected QuoteGeneratorClient(string source) : base(source)
		{
			BaseClient = new()
			{
				BaseAddress = new Uri(source)
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteGeneratorClient{T}"/> class with a specified <paramref name="uri"/>
		/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </summary>
		/// <param name="uri">
		/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
		protected QuoteGeneratorClient(Uri uri) : base(RetrieveSourceFromUri(uri))
		{
			BaseClient = new()
			{
				BaseAddress = uri
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteGeneratorClient{T}"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty.</exception>
		protected QuoteGeneratorClient(HttpClient client) : base(RetrieveSourceFromClient(client))
		{
			BaseClient = client;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteGeneratorClient{T}"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="source"></param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected QuoteGeneratorClient(HttpClient client, string source) : base(source)
		{
			if (client is null)
			{
				throw Internals.Null(nameof(client));
			}

			BaseClient = client;
		}

		/// <summary>
		/// Destructs the <see cref="QuoteGeneratorClient{T}"/> class.
		/// </summary>
		~QuoteGeneratorClient()
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

		/// <inheritdoc/>
		public override T GetRandomQuote()
		{
			return GetRandomQuoteAsync().Result;
		}

		/// <inheritdoc/>
		public override T? GetRandomQuote(string tag)
		{
			return GetRandomQuoteAsync(tag).Result;
		}

		/// <inheritdoc/>
		public override T? GetRandomQuote(params string[]? tags)
		{
			return GetRandomQuoteAsync(tags).Result;
		}

		/// <inheritdoc cref="IQuoteGeneratorClient.GetRandomQuoteAsync()"/>
		public abstract Task<T> GetRandomQuoteAsync();

		/// <inheritdoc cref="IQuoteGeneratorClient.GetRandomQuoteAsync(string)"/>
		public Task<T?> GetRandomQuoteAsync(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
			{
				throw Internals.NullOrEmpty(nameof(tag));
			}

			return GetRandomQuoteAsync(new string[] { tag });
		}

		/// <inheritdoc cref="IQuoteGeneratorClient.GetRandomQuoteAsync(string[])"/>
		public abstract Task<T?> GetRandomQuoteAsync(params string[]? tags);

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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		private static string RetrieveSourceFromClient(HttpClient client)
		{
			if (client is null)
			{
				throw Internals.Null(nameof(client));
			}

			string? str = client.BaseAddress?.ToString();

			if (string.IsNullOrWhiteSpace(str))
			{
				throw new ArgumentException("Base address of client cannot be null or empty", nameof(client));
			}

			return str;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		private static string RetrieveSourceFromUri(Uri uri)
		{
			if (uri is null)
			{
				throw Internals.Null(nameof(uri));
			}

			return uri.ToString();
		}
	}
}
