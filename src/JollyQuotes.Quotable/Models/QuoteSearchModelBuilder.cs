using System;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Builds a <see cref="QuoteSearchModel"/> step-by-step.
	/// </summary>
	public class QuoteSearchModelBuilder : SearchModelBuilder<QuoteSearchModel>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModelBuilder"/> class.
		/// </summary>
		public QuoteSearchModelBuilder()
		{
		}

		/// <inheritdoc/>
		public override QuoteSearchModelBuilder WithAuthor(string author)
		{
			base.WithAuthor(author);
			return this;
		}

		/// <inheritdoc/>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use WithAuthor(string) instead.")]
		public override QuoteSearchModelBuilder WithAuthorId(string authorId)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			base.WithAuthorId(authorId);
#pragma warning restore CS0618 // Type or member is obsolete

			return this;
		}

		/// <inheritdoc/>
		public override QuoteSearchModelBuilder WithMaxLength(int maxLength)
		{
			base.WithMaxLength(maxLength);
			return this;
		}

		/// <inheritdoc/>
		public override QuoteSearchModelBuilder WithMinLength(int minLength)
		{
			base.WithMinLength(minLength);
			return this;
		}

		/// <inheritdoc/>
		protected override QuoteSearchModel Create()
		{
			return new QuoteSearchModel(MinLength, MaxLength)
			{
#pragma warning disable CS0618 // Type or member is obsolete
				AuthorIds = AuthorIds.ToArray(),
#pragma warning restore CS0618 // Type or member is obsolete
				Authors = Authors.ToArray(),

				// TODO: Add Tags

				//Tags = Tags
			};
		}
	}
}
