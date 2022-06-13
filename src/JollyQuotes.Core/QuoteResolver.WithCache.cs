using System;

namespace JollyQuotes
{
	public abstract partial class QuoteResolver<T> where T : class, IQuote
	{
		/// <summary>
		/// <see cref="IQuoteGenerator"/> that generates random quotes using an external API accessed by an <see cref="IResourceResolver"/> and provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : QuoteGenerator<T>.WithCache, IDisposable
		{
			/// <inheritdoc/>
			public IResourceResolver Resolver { get; }

			/// <summary>
			/// Determines whether the current instance has been already disposed.
			/// </summary>
			protected bool Disposed { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
			/// </summary>
			/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested resources.</param>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(
				IResourceResolver resolver,
				string source,
				IQuoteCache<T>? cache = default,
				IPossibility? possibility = default
			) : base(source, cache, possibility)
			{
				if (resolver is null)
				{
					throw Error.Null(nameof(resolver));
				}

				Resolver = resolver;
			}

			/// <summary>
			/// Releases the unmanaged resources used by the <see cref="WithCache"/> and optionally disposes of the managed resources.
			/// </summary>
			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			/// <summary>
			/// Releases the unmanaged resources used by the <see cref="WithCache"/> and optionally disposes of the managed resources.
			/// </summary>
			/// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;<see langword="false"/> to release only unmanaged resources.</param>
			protected virtual void Dispose(bool disposing)
			{
				if (Disposed)
				{
					return;
				}

				if (disposing)
				{
					Internals.TryDispose(Resolver);
				}

				Disposed = true;
			}
		}
	}
}
