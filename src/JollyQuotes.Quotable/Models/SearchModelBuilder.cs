using System;
using System.Collections.Generic;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Builds a search query model step-by-step.
	/// </summary>
	/// <typeparam name="T">Type of search query model.</typeparam>
	public abstract class SearchModelBuilder<T> where T : class
	{
		/// <summary>
		/// List of author ids to add to the query.
		/// </summary>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use Authors instead.")]
		protected List<string> AuthorIds { get; }

		/// <summary>
		/// List of authors to add to the query.
		/// </summary>
		protected List<string> Authors { get; }

		/// <summary>
		/// Max length of the object being searched for.
		/// </summary>
		protected int MaxLength { get; private set; }

		/// <summary>
		/// Min length of the object being searched for.
		/// </summary>
		protected int MinLength { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchModelBuilder{T}"/> class.
		/// </summary>
		protected SearchModelBuilder()
		{
			MaxLength = -1;
			MinLength = -1;
			Authors = new();
#pragma warning disable CS0618 // Type or member is obsolete
			AuthorIds = new();
#pragma warning restore CS0618 // Type or member is obsolete
		}

		/// <summary>
		/// Actually creates the object being built.
		/// </summary>
		public T Build()
		{
			Validate();
			T t = Create();
			Reset();

			return t;
		}

		/// <summary>
		/// Resets the builder to its default state.
		/// </summary>
		public virtual void Reset()
		{
			MaxLength = -1;
			MinLength = -1;
			Authors.Clear();
#pragma warning disable CS0618 // Type or member is obsolete
			AuthorIds.Clear();
#pragma warning restore CS0618 // Type or member is obsolete
		}

		/// <summary>
		/// Adds the specified <paramref name="author"/> to the search query. Both full name and slug can be used.
		/// </summary>
		/// <remarks>This method can be called multiple times to include multiple authors in the search query.</remarks>
		/// <param name="author">Full name or slug of author to add to the search query.</param>
		/// <exception cref="ArgumentException"><paramref name="author"/> is <see langword="null"/> or empty.</exception>
		public virtual SearchModelBuilder<T> WithAuthor(string author)
		{
			if (string.IsNullOrWhiteSpace(author))
			{
				throw Error.NullOrEmpty(nameof(author));
			}

			if (!Authors.Contains(author))
			{
				Authors.Add(author);
			}

			return this;
		}

		/// <summary>
		/// Adds the specified <paramref name="authorId"/> to the search query.
		/// </summary>
		/// <remarks>This method can be called multiple times to include multiple author ids in the search query.</remarks>
		/// <param name="authorId">Id of author to add to the search query.</param>
		/// <returns></returns>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use WithAuthor(string) instead.")]
		public virtual SearchModelBuilder<T> WithAuthorId(string authorId)
		{
			if (string.IsNullOrWhiteSpace(authorId))
			{
				throw Error.NullOrEmpty(nameof(authorId));
			}

			if (!AuthorIds.Contains(authorId))
			{
				Authors.Add(authorId);
			}

			return this;
		}

		/// <summary>
		/// Adds the specified <paramref name="maxLength"/> of object to the search query.
		/// </summary>
		/// <param name="maxLength">Max length of the object being searched for.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="maxLength"/> must be greater than or equal to <c>0</c>.</exception>
		public virtual SearchModelBuilder<T> WithMaxLength(int maxLength)
		{
			if (maxLength < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(maxLength), 0);
			}

			MaxLength = maxLength;

			return this;
		}

		/// <summary>
		/// Adds the specified <paramref name="minLength"/> of object to the search query.
		/// </summary>
		/// <param name="minLength">Min length of the object being searched for.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="minLength"/> must be greater than or equal to <c>0</c>.</exception>
		public virtual SearchModelBuilder<T> WithMinLength(int minLength)
		{
			if (minLength < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(minLength), 0);
			}

			MinLength = minLength;

			return this;
		}

		/// <summary>
		/// Creates the object being built without validation.
		/// </summary>
		protected abstract T Create();

		/// <summary>
		/// Ensures that the collected values are valid.
		/// </summary>
		/// <exception cref="InvalidOperationException">MaxLength must be greater than or equal to MinLength.</exception>
		protected virtual void Validate()
		{
			if (MaxLength > -1 && MaxLength < MinLength)
			{
				throw Error.InvOp($"{nameof(MaxLength)} must be greater than or equal to {nameof(MinLength)}");
			}
		}
	}
}
