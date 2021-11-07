using System;

namespace JollyQuotes
{
	public abstract partial class RandomQuoteGenerator<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IRandomQuoteGenerator"/> that provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public abstract class WithCache : IRandomQuoteGenerator
		{
			/// <summary>
			/// Container of all the cached <see cref="IQuote"/>s.
			/// </summary>
			public BlockableQuoteCache<T> Cache { get; }

			/// <summary>
			/// Random number generator used to determine whether to pick quotes from the <see cref="Cache"/>
			/// or <see cref="Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </summary>
			public IPossibility Possibility { get; }

			/// <summary>
			/// Source of the quotes, e.g. a link, file name or raw text.
			/// </summary>
			public string Source { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="Cache"/>
			/// or <see cref="Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(
				string source,
				IQuoteCache<T>? cache = null,
				IPossibility? possibility = null
			)
			{
				if (string.IsNullOrWhiteSpace(source))
				{
					throw Throw.NullOrEmpty(nameof(source));
				}

				Source = source;
				Possibility = possibility ?? new Possibility();

				if (cache is null)
				{
					Cache = new BlockableQuoteCache<T>(new QuoteCache<T>());
				}
				else if (cache is BlockableQuoteCache<T> b)
				{
					Cache = b;
				}
				else
				{
					Cache = new BlockableQuoteCache<T>(cache);
				}
			}

			/// <summary>
			/// Generates a random quote.
			/// </summary>
			/// <param name="which">Determines which quotes to include in the search.</param>
			public T GetRandomQuote(QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:

						if (Cache.IsEmpty || Possibility.Determine())
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							goto case QuoteInclude.Cached;
						}

					case QuoteInclude.Cached:
						return Cache.GetRandomQuote();

					case QuoteInclude.Download:
						T quote = DownloadRandomQuote();
						CacheQuote(quote);
						return quote;

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Generates a random quote associated with the specified tag.
			/// </summary>
			/// <param name="tag">Tag to generate a quote associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			public T? GetRandomQuote(string tag, QuoteInclude which = QuoteInclude.All)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Throw.NullOrEmpty(nameof(tag));
				}

				switch (which)
				{
					case QuoteInclude.All:
					{
						if (Cache.IsEmpty || Possibility.Determine() || !Cache.TryGetRandomQuote(tag, out T? quote))
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							return quote;
						}
					}

					case QuoteInclude.Cached:
					{
						if (Cache.TryGetRandomQuote(tag, out T? quote))
						{
							return quote;
						}

						return default;
					}

					case QuoteInclude.Download:
					{
						T? quote = DownloadRandomQuote(tag);

						if (quote is not null)
						{
							CacheQuote(quote);
						}

						return quote;
					}

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Generates a random quote associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to generate a quote associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public T? GetRandomQuote(string[]? tags, QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
					{
						if (Cache.IsEmpty || Possibility.Determine() || GetQuoteFromCache() is not T quote)
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							return quote;
						}
					}

					case QuoteInclude.Cached:
						return GetQuoteFromCache();

					case QuoteInclude.Download:
					{
						T? quote = DownloadRandomQuote(tags);

						if (quote is not null)
						{
							CacheQuote(quote);
						}

						return quote;
					}

					default:
						goto case QuoteInclude.All;
				}

				T? GetQuoteFromCache()
				{
					if (tags is null)
					{
						return default;
					}

					foreach (string tag in tags)
					{
						if (Cache.TryGetRandomQuote(tag, out T? quote))
						{
							return quote;
						}
					}

					return default;
				}
			}

			IQuote IRandomQuoteGenerator.GetRandomQuote()
			{
				return GetRandomQuote()!;
			}

			IQuote? IRandomQuoteGenerator.GetRandomQuote(string tag)
			{
				return GetRandomQuote(tag);
			}

			IQuote? IRandomQuoteGenerator.GetRandomQuote(params string[]? tags)
			{
				return GetRandomQuote(tags);
			}

			/// <summary>
			/// Adds the specified <paramref name="quote"/> to the <see cref="Cache"/> if <see cref="BlockableQuoteCache{T}.IsBlocked"/> is <see langword="false"/>.
			/// </summary>
			/// <param name="quote"><see cref="IQuote"/> to cache.</param>
			protected void CacheQuote(T quote)
			{
				if (!Cache.IsBlocked)
				{
					Cache.CacheQuote(quote);
				}
			}

			/// <summary>
			/// Downloads a quote from the <see cref="Source"/>.
			/// </summary>
			protected abstract T DownloadRandomQuote();

			/// <summary>
			/// Downloads a quote associated with the specified <paramref name="tag"/> from the <see cref="Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download a quote associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual T? DownloadRandomQuote(string tag)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Throw.NullOrEmpty(nameof(tag));
				}

				return DownloadRandomQuote(new string[] { tag });
			}

			/// <summary>
			/// Downloads a random quote associated with any of the specified <paramref name="tags"/> from the <see cref="Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download a quote associated with.</param>
			protected abstract T? DownloadRandomQuote(params string[]? tags);
		}
	}
}
