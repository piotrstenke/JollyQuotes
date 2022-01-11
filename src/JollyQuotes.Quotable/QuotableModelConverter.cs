using System.Text;
using JollyQuotes.Quotable.Models;

using static JollyQuotes.Internals;

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
			if(model is null)
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
		public string GetSearchQuery(QuoteSearchModel searchModel)
		{
			if(searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			StringBuilder builder = new();
			bool hasParam = false;

			if(searchModel.MinLength > 0)
			{
				hasParam = true;
				builder.Append("minLength=");
				builder.Append(searchModel.MinLength);
			}

			if(searchModel.MaxLength.HasValue)
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("maxLength=");
				builder.Append(searchModel.MaxLength);
			}

			if(searchModel.Tags is not null)
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("tags=");
				WriteTagExpression(searchModel.Tags, builder);
			}

			if(searchModel.HasAuthor)
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("author=");
				builder.Append(string.Join(SearchOperator.Or.ToChar(), searchModel.Authors));
			}

#pragma warning disable CS0618 // Type or member is obsolete
			if (searchModel.HasAuthorId)
			{
				EnsureParameter(ref hasParam, builder);
				builder.Append("authorId=");
				builder.Append(string.Join(SearchOperator.Or.ToChar(), searchModel.AuthorIds));
			}
#pragma warning restore CS0618 // Type or member is obsolete

			return builder.ToString();
		}

		/// <inheritdoc/>
		public string GetTagExpression(TagExpression expression)
		{
			if(expression is null)
			{
				throw Error.Null(nameof(expression));
			}

			StringBuilder builder = new();
			WriteTagExpression(expression, builder);

			return builder.ToString();
		}

		private static void WriteTagExpression(TagExpression expression, StringBuilder builder)
		{
			if(expression.IsEndNode)
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
