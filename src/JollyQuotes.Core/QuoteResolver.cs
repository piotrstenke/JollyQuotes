using System;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IQuoteGenerator"/> that generates random quotes using an external API accessed by an <see cref="IResourceResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class QuoteResolver<T> : QuoteGenerator<T>, IQuoteService, IDisposable where T : class, IQuote
	{
		/// <inheritdoc/>
		public IResourceResolver Resolver { get; }

		/// <summary>
		/// Determines whether the current instance has been already disposed.
		/// </summary>
		protected bool Disposed { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteResolver{T}"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected QuoteResolver(IResourceResolver resolver, string source) : base(source)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			Resolver = resolver;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="QuoteResolver{T}"/> and optionally disposes of the managed resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="QuoteResolver{T}"/> and optionally disposes of the managed resources.
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
