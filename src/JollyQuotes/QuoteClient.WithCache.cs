﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes
{
	public abstract partial class QuoteClient<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IQuoteResolver"/> that provides special handling of <see cref="IResourceResolver"/>s of type <see cref="JollyQuotes.HttpResolver"/>
		/// and a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : QuoteResolver<T>.WithCache
		{
			/// <summary>
			/// Returns the <see cref="QuoteResolver{T}.WithCache.Resolver"/> as a <see cref="JollyQuotes.HttpResolver"/>.
			/// </summary>
			public HttpResolver? HttpResolver => Resolver as HttpResolver;

			/// <summary>
			/// Determines whether the <see cref="QuoteResolver{T}.WithCache.Resolver"/> is a <see cref="JollyQuotes.HttpResolver"/>.
			/// </summary>
			[MemberNotNullWhen(true, nameof(HttpResolver), nameof(BaseClient))]
			public bool IsHttpBased => Resolver is HttpResolver;

			/// <summary>
			/// <see cref="HttpClient"/> that is used to resolve the requested resources or <see langword="null"/> if <see cref="IsHttpBased"/> is <see langword="false"/>.
			/// </summary>
			public HttpClient? BaseClient => Resolver is HttpResolver r ? r.BaseClient : null;

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			/// <exception cref="UriFormatException">Invalid format of the <paramref name="source"/>.</exception>
			protected WithCache(
				string source,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(Internals.CreateResolver(source), source, cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a specified <paramref name="uri"/>
			/// that is used as a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
			/// </summary>
			/// <param name="uri">
			/// <see cref="Uri"/> that is used a <see cref="RandomQuoteGenerator{T}.Source"/> of the quote resources
			/// and <see cref="HttpClient.BaseAddress"/> of the underlaying <see cref="BaseClient"/>.
			/// </param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="uri"/> is <see langword="null"/>.</exception>
			protected WithCache(
				Uri uri,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(Internals.CreateResolver(uri), uri.ToString(), cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="client"/> specified.
			/// </summary>
			/// <param name="client">Underlaying client that is used to access required resources.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="client"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
			protected WithCache(
				HttpClient client,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(new HttpResolver(client), Internals.RetrieveSourceFromClient(client), cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="resolver"/> specified.
			/// </summary>
			/// <param name="resolver"><see cref="JollyQuotes.HttpResolver"/> that is used to access the requested resources.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><see cref="HttpClient.BaseAddress"/> of <paramref name="resolver"/> cannot be <see langword="null"/> or empty when no source specified.</exception>
			protected WithCache(
				HttpResolver resolver,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(resolver, Internals.RetrieveSourceFromClient(resolver.BaseClient), cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="resolver"/> and <paramref name="source"/> specified.
			/// </summary>
			/// <param name="resolver"><see cref="JollyQuotes.HttpResolver"/> that is used to access the requested resources.</param>
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
			) : base(resolver, source, cache, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with an underlaying <paramref name="client"/> and <paramref name="source"/> specified.
			/// </summary>
			/// <param name="client">Underlaying client that is used to access required resources.</param>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(
				HttpClient client,
				string source,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			) : base(new HttpResolver(client), source, cache, possibility)
			{
			}
		}
	}
}
