using System;

namespace JollyQuotes
{
	public abstract partial class QuoteResolver<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IRandomQuoteGenerator"/> that generates random quotes using an external API accessed by an <see cref="IResourceResolver"/> and provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : RandomQuoteGenerator<T>.WithCache
		{
			/// <inheritdoc/>
			public IResourceResolver Resolver { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
			/// </summary>
			/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested resources.</param>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(
				IResourceResolver resolver,
				string source,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(source, cache, possibility)
			{
				if (resolver is null)
				{
					throw Error.Null(nameof(resolver));
				}

				Resolver = resolver;
			}
		}
	}
}
