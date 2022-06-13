using System;
using System.Net.Http;

namespace JollyQuotes
{
	/// <inheritdoc cref="IQuoteService"/>
	public abstract class QuoteService : IQuoteService, IDisposable
	{
		/// <summary>
		/// <see cref="IResourceResolver"/> that is used to access requested resources.
		/// </summary>
		public IResourceResolver Resolver { get; }

		/// <summary>
		/// Determines whether the current instance has been already disposed.
		/// </summary>
		protected bool Disposed { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		protected QuoteService(HttpClient client)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			Resolver = new HttpResolver(client);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested <c>kanye.rest</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		protected QuoteService(IResourceResolver resolver)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			Resolver = resolver;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="QuoteService"/> and optionally disposes of the managed resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="QuoteService"/> and optionally disposes of the managed resources.
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
