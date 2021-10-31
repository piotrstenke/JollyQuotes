using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JollyQuotes
{
	public abstract partial class EnumerableQuoteGenerator<T> where T : IQuote
	{
		/// <summary>
		/// <see cref="IEnumerableQuoteGenerator"/> that provides a mechanism for caching <see cref="IQuote"/>s.
		/// </summary>
		public new abstract class WithCache : RandomQuoteGenerator<T>.WithCache, IEnumerableQuoteGenerator
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source) : base(source)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> specified and <paramref name="possibility"/>.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, IPossibility possibility) : base(source, possibility)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/> and an underlaying <paramref name="cache"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, BlockableQuoteCache<T> cache) : base(source, cache)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="WithCache"/> class with a <paramref name="source"/>, underlaying <paramref name="cache"/> and <paramref name="possibility"/> specified.
			/// </summary>
			/// <param name="source">Source of the quotes, e.g. a link, file name or raw text.</param>
			/// <param name="cache">Container of all the cached <see cref="IQuote"/>s.</param>
			/// <param name="possibility">
			/// Random number generator used to determine whether to pick quotes from the <see cref="RandomQuoteGenerator{T}.WithCache.Cache"/>
			/// or <see cref="RandomQuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
			/// </param>
			/// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>. -or-
			/// <paramref name="possibility"/> is <see langword="null"/>.</exception>
			/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
			protected WithCache(string source, BlockableQuoteCache<T> cache, IPossibility possibility) : base(source, cache, possibility)
			{
			}

			/// <summary>
			/// Returns a collection of all possible quotes.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerable<T> GetAllQuotes(QuoteInclude which = QuoteInclude.All)
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
			public IEnumerable<T> GetAllQuotes(string tag, QuoteInclude which = QuoteInclude.All)
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
			public IEnumerable<T> GetAllQuotes(string[]? tags, QuoteInclude which = QuoteInclude.All)
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

			/// <summary>
			/// Returns an <see cref="IEnumerator{T}"/> that iterates through all the available <see cref="IQuote"/>s.
			/// </summary>
			/// <param name="which">Determines which quotes to include.</param>
			public IEnumerator<T> GetEnumerator(QuoteInclude which = QuoteInclude.All)
			{
				switch (which)
				{
					case QuoteInclude.All:
						return YieldAll();

					case QuoteInclude.Download:
						return DownloadAndEnumerate();

					case QuoteInclude.Cached:
						return Cache.GetEnumerator();

					default:
						goto case QuoteInclude.All;
				}

				IEnumerator<T> YieldAll()
				{
					foreach (T quote in Cache)
					{
						yield return quote;
					}

					using IEnumerator<T> enumerator = DownloadAndEnumerate();

					while (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}
				}
			}

			IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes()
			{
				return GetAllQuotes().Cast<IQuote>();
			}

			IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes(string tag)
			{
				return GetAllQuotes(tag).Cast<IQuote>();
			}

			IEnumerable<IQuote> IEnumerableQuoteGenerator.GetAllQuotes(params string[]? tags)
			{
				return GetAllQuotes(tags).Cast<IQuote>();
			}

			IEnumerator<IQuote> IEnumerable<IQuote>.GetEnumerator()
			{
				foreach (T quote in GetAllQuotes())
				{
					yield return quote;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			/// <summary>
			/// Downloads all quotes available from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>
			/// </summary>
			protected abstract IEnumerable<T> DownloadAllQuotes();

			/// <summary>
			/// Downloads all quotes associated with the specified <paramref name="tag"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
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
			/// Downloads all quotes associated with any of the specified <paramref name="tags"/> from the <see cref="RandomQuoteGenerator{T}.WithCache.Source"/>.
			/// </summary>
			/// <param name="tags">Tags to download all quotes associated with.</param>
			protected abstract IEnumerable<T> DownloadAllQuotes(params string[]? tags);

			/// <summary>
			/// Downloads all available <see cref="IQuote"/>s and enumerates them.
			/// </summary>
			protected virtual IEnumerator<T> DownloadAndEnumerate()
			{
				return DownloadAllQuotes().GetEnumerator();
			}
		}
	}
}
