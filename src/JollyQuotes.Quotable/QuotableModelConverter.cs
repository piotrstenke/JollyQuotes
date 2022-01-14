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
		public string GetName(SortOrder value)
		{
			return QuotableResources.GetName(value);
		}

		/// <inheritdoc/>
		public string GetName(SortBy value)
		{
			return QuotableResources.GetName(value);
		}

		/// <inheritdoc/>
		public string GetName(QuoteSortBy value)
		{
			return QuotableResources.GetName(value);
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

			WriteSearchQuery(searchModel, ref hasParam, builder);

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

			WriteSearchQuery(searchModel, ref hasParam, builder);

			if (searchModel.Limit != QuotableResources.DefaultResultsPerPage)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("limit=");
				builder.Append(searchModel.Limit);
			}

			if (searchModel.Page > 1)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("page=");
				builder.Append(searchModel.Page);
			}

			builder.EnsureParameter(ref hasParam);
			builder.Append("sortBy=");
			builder.Append(GetName(searchModel.SortBy));

			builder.EnsureParameter(ref hasParam);
			builder.Append("order=");
			builder.Append(GetName(searchModel.Order));

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

			builder.Append("sortBy=");
			builder.Append(GetName(searchModel.SortBy));
			builder.ApplyParameter();
			builder.Append("order=");
			builder.Append(GetName(searchModel.Order));

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
			WriteTagExpression(expression, builder);

			return builder.ToString();
		}

		private static void WriteSearchQuery(QuoteSearchModel searchModel, ref bool hasParam, StringBuilder builder)
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
				WriteTagExpression(searchModel.Tags, builder);
			}

			if (searchModel.HasAuthor)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("author=");
				builder.Append(string.Join(SearchOperator.Or.ToChar(), searchModel.Authors));
			}

#pragma warning disable CS0618 // Type or member is obsolete
			if (searchModel.HasAuthorId)
			{
				builder.EnsureParameter(ref hasParam);
				builder.Append("authorId=");
				builder.Append(string.Join(SearchOperator.Or.ToChar(), searchModel.AuthorIds));
			}
#pragma warning restore CS0618 // Type or member is obsolete
		}

		private static void WriteTagExpression(TagExpression expression, StringBuilder builder)
		{
			if (expression.IsEndNode)
			{
				builder.Append(expression.Value);
			}
			else
			{
				WriteTagExpression(expression.Left, builder);
				builder.Append(expression.Operator.ToChar());
				WriteTagExpression(expression.Left, builder);
			}
		}
	}
}
