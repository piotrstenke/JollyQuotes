using System;
using System.Collections.Generic;

namespace JollyQuotes.TronaldDump
{
	public partial class TronaldDumpQuoteGenerator
	{
		private sealed class InternalGenerator : EnumerableQuoteResolver<TronaldDumpQuote>.WithCache
		{
			private const string ERROR_ENUMERABLE_NOT_SUPPORTED = "Tronald Dump does not support quote enumeration";

			private readonly TronaldDumpQuoteGenerator _generator;

			/// <inheritdoc cref="TronaldDumpQuoteGenerator.ModelConverter"/>
			public ITronaldDumpModelConverter ModelConverter => _generator.ModelConverter;

			/// <inheritdoc cref="TronaldDumpQuoteGenerator.Random"/>
			public Random Random => _generator.Random;

			/// <inheritdoc cref="TronaldDumpQuoteGenerator.Service"/>
			public ITronaldDumpService Service => _generator.Service;

			/// <summary>
			/// Initializes a new instance of the <see cref="InternalGenerator"/> class with a <paramref name="baseGenerator"/> specified.
			/// </summary>
			/// <param name="baseGenerator"><see cref="TronaldDumpQuoteGenerator"/> this generator was instantiated by.</param>
			public InternalGenerator(TronaldDumpQuoteGenerator baseGenerator)
				: base(baseGenerator.Resolver, baseGenerator.Source, baseGenerator.Cache, baseGenerator.Possibility)
			{
				_generator = baseGenerator;
			}

			/// <inheritdoc/>
			[Obsolete(ERROR_ENUMERABLE_NOT_SUPPORTED)]
			protected override IEnumerable<TronaldDumpQuote> DownloadAllQuotes()
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc/>
			protected override IEnumerable<TronaldDumpQuote> DownloadAllQuotes(string tag)
			{
				return _generator.DownloadAllQuotes(tag);
			}

			/// <inheritdoc/>
			protected override IEnumerable<TronaldDumpQuote> DownloadAllQuotes(params string[]? tags)
			{
				return _generator.DownloadAllQuotes(tags);
			}

			/// <inheritdoc/>
			[Obsolete(ERROR_ENUMERABLE_NOT_SUPPORTED)]
			protected override IEnumerator<TronaldDumpQuote> DownloadAndEnumerate()
			{
				throw new NotImplementedException();
			}

			/// <inheritdoc/>
			protected override TronaldDumpQuote DownloadRandomQuote()
			{
				return _generator.DownloadRandomQuote();
			}

			/// <inheritdoc/>
			protected override TronaldDumpQuote? DownloadRandomQuote(params string[]? tags)
			{
				return _generator.DownloadRandomQuote(tags);
			}

			/// <inheritdoc/>
			protected override TronaldDumpQuote? DownloadRandomQuote(string tag)
			{
				return _generator.DownloadRandomQuote(tag);
			}
		}
	}
}
