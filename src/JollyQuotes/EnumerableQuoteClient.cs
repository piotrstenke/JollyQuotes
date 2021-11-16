using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that provides mechanism for enumerating through a set of available <see cref="IQuote"/>s using a <see cref="IResourceResolver"/> with an additional handling of <see cref="JollyQuotes.HttpResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class EnumerableQuoteClient<T> : EnumerableQuoteResolver<T> where T : class, IQuote
	{
		/// <summary>
		/// <see cref="HttpClient"/> that is used to resolve the requested resources or <see langword="null"/> if <see cref="IsHttpBased"/> is <see langword="false"/>.
		/// </summary>
		public HttpClient? BaseClient => Resolver is HttpResolver r ? r.BaseClient : null;

		/// <summary>
		/// Returns the <see cref="EnumerableQuoteResolver{T}.Resolver"/> as a <see cref="JollyQuotes.HttpResolver"/>.
		/// </summary>
		public HttpResolver? HttpResolver => Resolver as HttpResolver;

		/// <summary>
		/// Determines whether the <see cref="EnumerableQuoteResolver{T}.Resolver"/> is a <see cref="JollyQuotes.HttpResolver"/>.
		/// </summary>
		[MemberNotNullWhen(true, nameof(HttpResolver), nameof(BaseClient))]
		public bool IsHttpBased => Resolver is HttpResolver;

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
		protected EnumerableQuoteClient(string source) : base(Internals.CreateResolver(source), source)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with a specified <paramref name="uri"/>
		/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </summary>
		/// <param name="uri">
		/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
		protected EnumerableQuoteClient(Uri uri) : base(Internals.CreateResolver(uri), uri.ToString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
		protected EnumerableQuoteClient(HttpClient client) : base(new HttpResolver(client), Internals.RetrieveSourceFromClient(client))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="JollyQuotes.HttpResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="resolver"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
		protected EnumerableQuoteClient(HttpResolver resolver) : base(resolver, Internals.RetrieveSourceFromClient(resolver.BaseClient))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteClient(IResourceResolver resolver, string source) : base(resolver, source)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EnumerableQuoteClient{T}"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected EnumerableQuoteClient(HttpClient client, string source) : base(new HttpResolver(client), source)
		{
		}
	}
}
