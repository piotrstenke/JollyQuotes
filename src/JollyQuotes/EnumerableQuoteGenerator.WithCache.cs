using System;
using System.Collections.Generic;
using System.Linq;

namespace JollyQuotes
{
	public abstract partial class EnumerableQuoteGenerator<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IEnumerableQuoteGenerator"/> that provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : EnumerableQuoteGenerator<T>
		{
			/// <summary>
			/// Contains all the cached <see cref="IQuote"/>s.
			/// </summary>
			public BlockableQuoteCache<T> Cache { get; }

			/// <summary>
			/// Random number generator used to determine whether to pick quotes from the <see cref="Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </summary>
			public IPossibility Possibility { get; }

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source) : base(source)
			{
				Cache = new BlockableQuoteCache<T>(new QuoteCache<T>());
				Possibility = new Possibility();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified and <paramref name="possibility"/>.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, IPossibility possibility) : base(source)
			{
				if (possibility is null)
				{
					throw Internals.Null(nameof(possibility));
				}

				Cache = new BlockableQuoteCache<T>(new QuoteCache<T>());
				Possibility = possibility;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> and an underlaying <paramref name="cache"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, BlockableQuoteCache<T> cache) : base(source)
			{
				if (cache is null)
				{
					throw Internals.Null(nameof(cache));
				}

				Cache = cache;
				Possibility = new Possibility();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/>, underlaying <paramref name="cache"/> and <paramref name="possibility"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>. -or-
			/// <paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, BlockableQuoteCache<T> cache, IPossibility possibility) : base(source)
			{
				if (cache is null)
				{
					throw Internals.Null(nameof(cache));
				}

				if (possibility is null)
				{
					throw Internals.Null(nameof(possibility));
				}

				Cache = cache;
				Possibility = possibility;
			}

			/// <inheritdoc/>
			public sealed override IEnumerable<T> GetAllQuotes()
			{
				return GetAllQuotes(QuoteInclude.All);
			}

			/// <inheritdoc/>
			public sealed override IEnumerable<T> GetAllQuotes(string tag)
			{
				return GetAllQuotes(tag, QuoteInclude.All);
			}

			/// <inheritdoc/>
			public sealed override IEnumerable<T> GetAllQuotes(params string[]? tags)
			{
				return GetAllQuotes(tags, QuoteInclude.All);
			}

			/// <summary>
			/// Returns a collection of all possible quotes.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerable<T> GetAllQuotes(QuoteInclude which)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached().Concat(DownloadAllQuotes());

					case QuoteInclude.Cached:
						return Cache.GetCached();

					case QuoteInclude.Download:
						return DownloadAllQuotes();

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Returns a collection of all possible quotes associated with the specified <paramref name="tag"/>.
			/// </summary>
			/// <param name="tag">Tag to get all <see cref="IQuote"/>s associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			public IEnumerable<T> GetAllQuotes(string tag, QuoteInclude which)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached(tag).Concat(DownloadAllQuotes(tag));

					case QuoteInclude.Cached:
						return Cache.GetCached(tag);

					case QuoteInclude.Download:
						return DownloadAllQuotes(tag);

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Returns a collection of all possible quotes associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to get all <see cref="IQuote"/> associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerable<T> GetAllQuotes(string[]? tags, QuoteInclude which)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return Cache.GetCached(tags).Concat(DownloadAllQuotes(tags));

					case QuoteInclude.Cached:
						return Cache.GetCached(tags);

					case QuoteInclude.Download:
						return DownloadAllQuotes(tags);

					default:
						goto case QuoteInclude.All;
				}
			}

			#region Copied from EnumerableQuoteGenerator<T>.WithCache

			/// <inheritdoc/>
			public sealed override T GetRandomQuote()
			{
				return GetRandomQuote(QuoteInclude.All)!;
			}

			/// <inheritdoc/>
			public sealed override T? GetRandomQuote(string tag)
			{
				return GetRandomQuote(tag, QuoteInclude.All);
			}

			/// <inheritdoc/>
			public sealed override T? GetRandomQuote(params string[]? tags)
			{
				return GetRandomQuote(tags, QuoteInclude.All);
			}

			/// <summary>
			/// Generates a random quote.
			/// </summary>
			/// <param name="which">Determines which quotes to include in the search.</param>
			public T? GetRandomQuote(QuoteInclude which)
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
						return DownloadRandomQuote();

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
			public T? GetRandomQuote(string tag, QuoteInclude which)
			{
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
						return DownloadRandomQuote(tag);

					default:
						goto case QuoteInclude.All;
				}
			}

			/// <summary>
			/// Generates a random quote associated with any of the specified <paramref name="tags"/>.
			/// </summary>
			/// <param name="tags">Tags to generate a quote associated with.</param>
			/// <param name="which">Determines which quotes to include.</param>
			public T? GetRandomQuote(string[]? tags, QuoteInclude which)
			{
				switch (which)
				{
					case QuoteInclude.All:

						if (Cache.IsEmpty || Possibility.Determine() || GetQuoteFromCache() is not T quote)
						{
							goto case QuoteInclude.Download;
						}
						else
						{
							return quote;
						}

					case QuoteInclude.Cached:
						return GetQuoteFromCache();

					case QuoteInclude.Download:
						return DownloadRandomQuote(tags);

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

			/// <summary>
			/// Downloads a quote from the <see cref="RandomQuoteGenerator{T}.Source"/>.
			/// </summary>
			protected abstract T DownloadRandomQuote();

			/// <summary>
			/// Downloads a quote associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download a quote associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual T? DownloadRandomQuote(string tag)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Internals.NullOrEmpty(nameof(tag));
				}

				return DownloadRandomQuote(new string[] { tag });
			}

			/// <summary>
			/// Downloads a random quote associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download a quote associated with.</param>
			protected abstract T? DownloadRandomQuote(params string[]? tags);

			#endregion Copied from EnumerableQuoteGenerator<T>.WithCache

			/// <summary>
			/// Downloads all quotes available from the <see cref="RandomQuoteGenerator{T}.Source"/>
			/// </summary>
			protected abstract IEnumerable<T> DownloadAllQuotes();

			/// <summary>
			/// Downloads all quotes associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.Source"/>.
			/// </summary>
			/// <param name="tag">Tag to download all quotes associated with.</param>
			/// <exception cref="ArgumentException"><paramref name="tag"/> is <see langword="null"/> or empty.</exception>
			protected virtual IEnumerable<T> DownloadAllQuotes(string tag)
			{
				if (string.IsNullOrWhiteSpace(tag))
				{
					throw Internals.NullOrEmpty(nameof(tag));
				}

				return DownloadAllQuotes(new string[] { tag });
			}

			/// <summary>
			/// Downloads all quotes associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download all quotes associated with.</param>
			protected abstract IEnumerable<T> DownloadAllQuotes(params string[]? tags);
		}
	}
}
