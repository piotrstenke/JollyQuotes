using System;
using System.Collections.Generic;
using JollyQuotes.TronaldDump.Models;

namespace JollyQuotes.TronaldDump
{
	/// <inheritdoc cref="ITronaldDumpModelConverter"/>
	public class TronaldDumpModelConverter : ITronaldDumpModelConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TronaldDumpModelConverter"/> class.
		/// </summary>
		public TronaldDumpModelConverter()
		{
		}

		/// <inheritdoc/>
		public TronaldDumpQuote ConvertQuoteModel(QuoteModel model)
		{
			if (model is null)
			{
				throw Error.Null(nameof(model));
			}

			return ConvertQuoteModelInternal(model);
		}

		/// <inheritdoc/>
		public int CountPages(ISearchResultModel model)
		{
			if (model is null)
			{
				throw Error.Null(nameof(model));
			}

			return (model.Total / TronaldDumpResources.MaxItemsPerPage) + 1;
		}

		/// <inheritdoc/>
		public IEnumerable<TronaldDumpQuote> EnumerateQuotes(QuoteListModel model)
		{
			if (model is null)
			{
				throw Error.Null(nameof(model));
			}

			if (model.Quotes.Length == 0)
			{
				return Array.Empty<TronaldDumpQuote>();
			}

			return Yield();

			IEnumerable<TronaldDumpQuote> Yield()
			{
				foreach (QuoteModel quote in model.Quotes)
				{
					yield return ConvertQuoteModelInternal(quote);
				}
			}
		}

		private static TronaldDumpQuote ConvertQuoteModelInternal(QuoteModel model)
		{
			return new TronaldDumpQuote(
				model.Id,
				model.Value,
				model.Embedded.Sources[0].Url,
				model.Tags,
				model.AppearedAt,
				model.CreatedAt,
				model.UpdatedAt
			);
		}
	}
}
