using System;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of authors and sources of the current resource.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record AuthorsAndSourcesModel
	{
		/// <summary>
		/// Array of authors of the current resource.
		/// </summary>
		[JsonProperty("author", Required = Required.Always)]
		public AuthorModel[] Authors { get; init; }

		/// <summary>
		/// Array of sources of the current resource.
		/// </summary>
		[JsonProperty("source", Required = Required.Always)]
		public QuoteSourceModel[] Sources { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorsAndSourcesModel"/> class with arrays of target <paramref name="authors"/> and <paramref name="sources"/> specified.
		/// </summary>
		/// <param name="authors">Array of authors of the current resource.</param>
		/// <param name="sources">Array of sources of the current resource.</param>
		/// <exception cref="ArgumentNullException"><paramref name="authors"/> is <see langword="null"/>. -or- <paramref name="sources"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="authors"/> is empty. -or- <paramref name="sources"/> is empty.</exception>
		[JsonConstructor]
		public AuthorsAndSourcesModel(AuthorModel[] authors, QuoteSourceModel[] sources)
		{
			if (authors is null)
			{
				throw Error.Null(nameof(authors));
			}

			if (sources is null)
			{
				throw Error.Null(nameof(sources));
			}

			if (authors.Length == 0)
			{
				throw Error.Empty(nameof(authors));
			}

			if (sources.Length == 0)
			{
				throw Error.Empty(nameof(sources));
			}

			Authors = authors;
			Sources = sources;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorsAndSourcesModel"/> class with target <paramref name="author"/> and <paramref name="source"/> specified.
		/// </summary>
		/// <param name="author">Author of the current resource.</param>
		/// <param name="source">Source of the current resource.</param>
		/// <exception cref="ArgumentNullException"><paramref name="author"/> is <see langword="null"/>. -or- <paramref name="source"/> is <see langword="null"/>.</exception>
		public AuthorsAndSourcesModel(AuthorModel author, QuoteSourceModel source)
		{
			if (author is null)
			{
				throw Error.Null(nameof(author));
			}

			if (source is null)
			{
				throw Error.Null(nameof(source));
			}

			Authors = new AuthorModel[] { author };
			Sources = new QuoteSourceModel[] { source };
		}

		/// <inheritdoc/>
		public bool Equals(AuthorsAndSourcesModel? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return
				other.Authors.Length == Authors.Length &&
				other.Sources.Length == Sources.Length &&
				other.Authors.SequenceEqual(Authors) &&
				other.Sources.SequenceEqual(Sources);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			foreach (AuthorModel author in Authors)
			{
				hash.Add(author);
			}

			foreach (QuoteSourceModel source in Sources)
			{
				hash.Add(source);
			}

			return hash.ToHashCode();
		}
	}
}
