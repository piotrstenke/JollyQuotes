using System;

namespace JollyQuotes
{
	/// <inheritdoc cref="IQuoteResolver"/>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class QuoteResolver<T> : RandomQuoteGenerator<T>, IQuoteResolver where T : IQuote
	{
		/// <inheritdoc/>
		public IResourceResolver Resolver { get; }

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
				throw Internals.Null(nameof(resolver));
			}

			Resolver = resolver;
		}
	}
}
