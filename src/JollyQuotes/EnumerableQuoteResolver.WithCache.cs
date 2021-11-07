using System;

namespace JollyQuotes
{
	public abstract partial class EnumerableQuoteResolver<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IRandomQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s using a <see cref="IResourceResolver"/>
		/// and a for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : EnumerableQuoteGenerator<T>.WithCache
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
					throw Internals.Null(nameof(resolver));
				}

				Resolver = resolver;
			}
		}
	}
}
