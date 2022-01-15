using System;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of authors and sources of the current resource.
	/// </summary>
	[JsonObject]
	public sealed record AuthorsAndSourcesModel
	{
		private readonly AuthorModel[] _authors;
		private readonly QuoteSourceModel[] _sources;

		/// <summary>
		/// Array of authors of the current resource.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("author", Order = 0, Required = Required.Always)]
		public AuthorModel[] Authors
		{
			get => _authors;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_authors = value;
			}
		}

		/// <summary>
		/// Array of sources of the current resource.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("source", Order = 1, Required = Required.Always)]
		public QuoteSourceModel[] Sources
		{
			get => _sources;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_sources = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorsAndSourcesModel"/> class with arrays of target <paramref name="authors"/> and <paramref name="sources"/> specified.
		/// </summary>
		/// <param name="authors">Array of authors of the current resource.</param>
		/// <param name="sources">Array of sources of the current resource.</param>
		/// <exception cref="ArgumentNullException"><paramref name="authors"/> is <see langword="null"/>. -or- <paramref name="sources"/> is <see langword="null"/>.</exception>
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

			_authors = authors;
			_sources = sources;
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

			_authors = new AuthorModel[] { author };
			_sources = new QuoteSourceModel[] { source };
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
				other._authors.Length == Authors.Length &&
				other._sources.Length == Sources.Length &&
				other._authors.SequenceEqual(Authors) &&
				other._sources.SequenceEqual(Sources);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.AddSequence(_authors);
			hash.AddSequence(_sources);

			return hash.ToHashCode();
		}
	}
}
