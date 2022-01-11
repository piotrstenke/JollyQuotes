using System;
using System.Collections.Generic;
using System.Text;
using JollyQuotes.TronaldDump.Models;

using static JollyQuotes.Internals;

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

			int pages = model.Total / TronaldDumpResources.MaxItemsPerPage;

			if (model.Total % TronaldDumpResources.MaxItemsPerPage > 0)
			{
				pages++;
			}

			return pages;
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

		/// <inheritdoc/>
		public string GetSearchQuery(QuoteSearchModel searchModel)
		{
			if(searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			if (searchModel.Phrases is not null && searchModel.Phrases.Length > 0)
			{
				hasParam = true;
				builder.Append("query=");
				builder.Append(string.Join('+', searchModel.Phrases));
			}

			if (!string.IsNullOrWhiteSpace(searchModel.Tag))
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("tag=");
				builder.Append(searchModel.Tag);
			}

			if (searchModel.Page > 0)
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("page=");
				builder.Append(searchModel.Page);
			}

			return builder.ToString();
		}

		private static TronaldDumpQuote ConvertQuoteModelInternal(QuoteModel model)
		{
			return new TronaldDumpQuote(
				new Id(model.Id),
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
