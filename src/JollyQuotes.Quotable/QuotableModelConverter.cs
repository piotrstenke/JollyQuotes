using System.Text;
using JollyQuotes.Quotable.Models;

namespace JollyQuotes.Quotable
{
	/// <inheritdoc cref="IQuotableModelConverter"/>
	public class QuotableModelConverter : IQuotableModelConverter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableModelConverter"/> class.
		/// </summary>
		public QuotableModelConverter()
		{
		}

		/// <inheritdoc/>
		public QuotableQuote ConvertQuoteModel(QuoteModel model)
		{
			if (model is null)
			{
				throw Error.Null(nameof(model));
			}

			return new QuotableQuote(
				new Id(model.Id),
				model.Content,
				model.Author,
				model.AuthorSlug,
				model.Tags,
				model.DateAdded,
				model.DateModified
			);
		}

		/// <inheritdoc/>
		public string GetFieldQuery(QuoteSearchFields fields)
		{
			StringBuilder builder = new();
			WriteFieldQuery(builder, fields);
			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(QuoteSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			WriteSearchQuery(builder, searchModel, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(QuoteListSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			WriteSearchQuery(builder, searchModel, ref hasParam);
			WriteLimitPage(builder, searchModel.Limit, searchModel.Page, ref hasParam);
			WriteSortQuery(builder, searchModel.SortBy, searchModel.Order, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(TagSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			WriteSortQuery(builder, searchModel.SortBy, searchModel.Order, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(AuthorSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			if (searchModel.HasSlug)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("slug=");
				WriteOrQuery(builder, searchModel.Slugs);
			}

			WriteSortQuery(builder, searchModel.SortBy, searchModel.Order, ref hasParam);
			WriteLimitPage(builder, searchModel.Limit, searchModel.Page, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(AuthorNameSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();

			builder.Append("query=");
			builder.Append(searchModel.Query);

			if (!searchModel.AutoComplete)
			{
				builder.ApplyParameter();
				builder.Append("autocomplete=false");
			}

			if (searchModel.MatchTreshold != MatchThreshold.Default)
			{
				builder.ApplyParameter();
				builder.Append("matchThreshold=");
				builder.Append((int)searchModel.MatchTreshold);
			}

			bool hasParam = false;
			WriteLimitPage(builder, searchModel.Limit, searchModel.Page, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetSearchQuery(QuoteContentSearchModel searchModel)
		{
			if (searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();

			builder.Append("query=");
			builder.Append(searchModel.Query);

			if (searchModel.Fields != QuoteSearchFields.All)
			{
				string fields = GetFieldQuery(searchModel.Fields);

				builder.ApplyParameter();
				builder.Append("fields=");
				builder.Append(fields);
			}

			if (searchModel.FuzzyMaxEdits != FuzzyMatchingTreshold.Default)
			{
				builder.ApplyParameter();
				builder.Append("fuzzyMaxEdits=");
				builder.Append(searchModel.FuzzyMaxExpansions);
			}

			if (searchModel.FuzzyMaxExpansions != QuotableResources.FuzzyDefaultExpansions)
			{
				builder.ApplyParameter();
				builder.Append("fuzzyMaxExpansions=");
				builder.Append(searchModel.FuzzyMaxExpansions);
			}

			bool hasParam = false;
			WriteLimitPage(builder, searchModel.Limit, searchModel.Page, ref hasParam);

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetTagQuery(TagExpression expression)
		{
			if (expression is null)
			{
				throw Error.Null(nameof(expression));
			}

			StringBuilder builder = new();
			WriteTagExpression(builder, expression);

			return builder.ToString();
		}

		private static void WriteFieldQuery(StringBuilder builder, QuoteSearchFields fields)
		{
			bool applyComma = false;

			if (fields.HasFlag(QuoteSearchFields.Content))
			{
				applyComma = true;
				builder.Append("content");
			}

			if (fields.HasFlag(QuoteSearchFields.Author))
			{
				EnsureComma();
				builder.Append("author");
			}

			if (fields.HasFlag(QuoteSearchFields.Tags))
			{
				EnsureComma();
				builder.Append("tags");
			}

			void EnsureComma()
			{
				if (applyComma)
				{
					builder.Append(',');
				}
				else
				{
					applyComma = true;
				}
			}
		}

		private static bool WriteLimitPage(StringBuilder builder, int limit, int page, ref bool hasParam)
		{
			if (limit != QuotableResources.ResultsPerPageDefault)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("limit=");
				builder.Append(limit);
			}

			if (page > 1)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("page=");
				builder.Append(page);
			}

			return hasParam;
		}

		private static void WriteOrQuery(StringBuilder builder, string[] values)
		{
			string text = string.Join(QuotableHelpers.CharOr, values);
			builder.Append(text);
		}

		private static void WriteSearchQuery(StringBuilder builder, QuoteSearchModel searchModel, ref bool hasParam)
		{
			if (searchModel.MinLength > 0)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("minLength=");
				builder.Append(searchModel.MinLength);
			}

			if (searchModel.MaxLength.HasValue)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("maxLength=");
				builder.Append(searchModel.MaxLength);
			}

			if (searchModel.Tags is not null)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("tags=");
				WriteTagExpression(builder, searchModel.Tags);
			}

			if (searchModel.HasAuthor)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("author=");
				WriteOrQuery(builder, searchModel.Authors);
			}

#pragma warning disable CS0618 // Type or member is obsolete
			if (searchModel.HasAuthorId)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("authorId=");
				WriteOrQuery(builder, searchModel.AuthorIds);
			}
#pragma warning restore CS0618 // Type or member is obsolete
		}

		private static void WriteSortOrder(StringBuilder builder, SortOrder order, SortOrder defaultOrder, ref bool hasParam)
		{
			if (order != defaultOrder)
			{
				string name = order.GetName();

				builder.EnsureParameter(ref hasParam);
				builder.Append("order=");
				builder.Append(name);
			}
		}

		private static void WriteSortQuery(StringBuilder builder, SortBy sortBy, SortOrder order, ref bool hasParam)
		{
			SortOrder defaultOrder = sortBy.GetDefaultSortOrder();

			if (sortBy != default)
			{
				string name = sortBy.GetName();

				builder.EnsureParameter(ref hasParam);
				builder.Append("sortBy=");
				builder.Append(name);
			}

			WriteSortOrder(builder, order, defaultOrder, ref hasParam);
		}

		private static void WriteSortQuery(StringBuilder builder, QuoteSortBy sortBy, SortOrder order, ref bool hasParam)
		{
			SortOrder defaultOrder = sortBy.GetDefaultSortOrder();

			if (sortBy != default)
			{
				string name = sortBy.GetName();

				builder.EnsureParameter(ref hasParam);
				builder.Append("sortBy=");
				builder.Append(name);
			}

			WriteSortOrder(builder, order, defaultOrder, ref hasParam);
		}

		private static void WriteTagExpression(StringBuilder builder, TagExpression expression)
		{
			if (expression.IsEndNode)
			{
				builder.Append(expression.Value);
			}
			else
			{
				WriteTagExpression(builder, expression.Left);
				builder.Append(expression.Operator.ToChar());
				WriteTagExpression(builder, expression.Left);
			}
		}
	}
}
