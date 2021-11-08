using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="QuoteResolver{T}"/> that provides special handling of <see cref="IResourceResolver"/>s of type <see cref="JollyQuotes.HttpResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of <see cref="IQuote"/> this class can generate.</typeparam>
	public abstract partial class QuoteClient<T> : QuoteResolver<T> where T : IQuote
	{
		/// <summary>
		/// Returns the <see cref="QuoteResolver{T}.Resolver"/> as a <see cref="JollyQuotes.HttpResolver"/>.
		/// </summary>
		public HttpResolver? HttpResolver => Resolver as HttpResolver;

		/// <summary>
		/// Determines whether the <see cref="QuoteResolver{T}.Resolver"/> is a <see cref="JollyQuotes.HttpResolver"/>.
		/// </summary>
		[MemberNotNullWhen(true, nameof(HttpResolver), nameof(BaseClient))]
		public bool IsHttpBased => Resolver is HttpResolver;

		/// <summary>
		/// <see cref="HttpClient"/> that is used to resolve the requested resources or <see langword="null"/> if <see cref="IsHttpBased"/> is <see langword="false"/>.
		/// </summary>
		public HttpClient? BaseClient => Resolver is HttpResolver r ? r.BaseClient : null;

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with a <paramref name="source"/> specified.
		/// </summary>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
		protected QuoteClient(string source) : base(Internals.CreateResolver(source), source)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with a specified <paramref name="uri"/>
		/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </summary>
		/// <param name="uri">
		/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
		/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
		protected QuoteClient(Uri uri) : base(Internals.CreateResolver(uri), uri.ToString())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
		protected QuoteClient(HttpClient client) : base(new HttpResolver(client), Internals.RetrieveSourceFromClient(client))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="JollyQuotes.HttpResolver"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="resolver"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
		protected QuoteClient(HttpResolver resolver) : base(resolver, Internals.RetrieveSourceFromClient(resolver.BaseClient))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected QuoteClient(IResourceResolver resolver, string source) : base(resolver, source)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteClient{T}"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="client">Underlaying client that is used to access required resources.</param>
		/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		protected QuoteClient(HttpClient client, string source) : base(new HttpResolver(client), source)
		{
		}
	}
}
