using System;

namespace JollyQuotes
{
	public abstract partial class EnumerableQuoteResolver<T> where T : class, IQuote
	{
		/// <summary>
		/// <see cref="IQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s using a <see cref="IResourceResolver"/>
		/// and a for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : EnumerableQuoteGenerator<T>.WithCache, IDisposable
		{
			/// <inheritdoc/>
			public IResourceResolver Resolver { get; }

			/// <summary>
			/// Determines whether the current instance has been already disposed.
			/// </summary>
			protected bool Disposed { get; private set; }

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
				if (!Disposed)
				{
					if (disposing && Resolver is IDisposable d)
					{
						d.Dispose();
					}

					Disposed = true;
				}
			}
		}
	}
}
