using System;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s using a <see cref="IResourceResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteResolver<T> : EnumerableQuoteGenerator<T>, IQuoteResolver where T : IQuote
	{
		/// <inheritdoc/>
		public IResourceResolver Resolver { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteResolver{T}"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteResolver(IResourceResolver resolver, string source) : base(source)
		{
			if (resolver is null)
			{
				throw Throw.Null(nameof(resolver));
			}

			Resolver = resolver;
		}
	}
}
