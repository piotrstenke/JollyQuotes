using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains data required to perform a quote search.
	/// </summary>
	[JsonObject]
	public record QuoteSearchModel
	{
		private readonly string[]? _authors;
		private readonly string[]? _authorIds;
		private readonly int? _maxLength;
		private readonly int _minLength;

		/// <summary>
		/// Maximum length in characters of the quote.
		/// </summary>
		/// <remarks>Setting this to <see langword="null"/> will specify that <see cref="MaxLength"/> should not be included in the query.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be greater than or equal to <see cref="MinLength"/>.</exception>
		[JsonProperty("maxLength", Order = 0)]
		public int? MaxLength
		{
			get => _maxLength;
			init
			{
				if (value.HasValue && value < _minLength)
				{
					if (value < 0)
					{
						throw Error.MustBeGreaterThanOrEqualTo(nameof(value), -1);
					}

					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(MinLength));
				}

				_maxLength = value;
			}
		}

		/// <summary>
		/// Minimum length in characters of the quote.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="MaxLength"/>.</exception>
		[JsonProperty("minLength", Order = 1)]
		public int MinLength
		{
			get => _minLength;
			init
			{
				if (value > _maxLength)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), nameof(MinLength));
				}

				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_minLength = value;
			}
		}

		/// <summary>
		/// Determines whether an <see cref="Author"/> is included in the search query.
		/// </summary>
		[MemberNotNullWhen(true, nameof(Author), nameof(Authors))]
		[JsonIgnore]
		public bool HasAuthor => _authors is not null && _authors.Length > 0;

		/// <summary>
		/// Determines whether an <see cref="AuthorId"/> is included in the search query.
		/// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
		[MemberNotNullWhen(true, nameof(AuthorId), nameof(AuthorIds))]
#pragma warning restore CS0618 // Type or member is obsolete
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use HasAuthor instead.")]
		[JsonIgnore]
		public bool HasAuthorId => _authorIds is not null && _authorIds.Length > 0;

		/// <summary>
		/// Author of the target quote.
		/// </summary>
		/// <remarks>This property returns the first element of the <see cref="Authors"/> array.
		/// <para>If value of this property is set to <see langword="null"/> or an empty <see cref="string"/>, <see cref="Authors"/> is automatically set to <see langword="null"/> as well.</para></remarks>
		[JsonIgnore]
		public string? Author
		{
			get
			{
				if (_authors is null || _authors.Length == 0)
				{
					return null;
				}

				return _authors[0];
			}
			init => Internals.SetFirstElement(value, ref _authors);
		}

		/// <summary>
		/// Collection of all possible authors of the returned quote.
		/// </summary>
		[JsonProperty("authors", Order = 3)]
		public string[]? Authors
		{
			get => _authors;
			init => _authors = value;
		}

		/// <summary>
		/// Id of author of the target quote.
		/// </summary>
		/// <remarks>This property returns the first element of the <see cref="AuthorIds"/> array.
		/// <para>If value of this property is set to <see langword="null"/> or an empty <see cref="string"/>, <see cref="AuthorIds"/> is automatically set to <see langword="null"/> as well.</para></remarks>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use Author instead.")]
		[JsonIgnore]
		public string? AuthorId
		{
			get
			{
				if (_authorIds is null || _authorIds.Length == 0)
				{
					return null;
				}

				return _authorIds[0];
			}
			init => Internals.SetFirstElement(value, ref _authorIds);
		}

		/// <summary>
		/// Collection of ids of all possible authors of the returned quote.
		/// </summary>
		[Obsolete(QuotableResources.AUTHOR_ID_OBSOLETE + "Use Authors instead.")]
		[JsonProperty("authorIds", Order = 4)]
		public string[]? AuthorIds
		{
			get => _authorIds;
			init => _authorIds = value;
		}

		/// <summary>
		/// Tags associated with the quote searched for.
		/// </summary>
		[JsonProperty("tags", Order = 5)]
		public string? Tags { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class.
		/// </summary>
		public QuoteSearchModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with a <paramref name="minLength"/>, <paramref name="maxLength"/>, <paramref name="tags"/> and <paramref name="authors"/> specified.
		/// </summary>
		/// <param name="minLength">Minimum length in characters of the quote.</param>
		/// <param name="maxLength">Maximum length in characters of the quote.</param>
		/// <param name="tags">Tags associated with the quote searched for.</param>
		/// <param name="authors">Collection of all possible authors of the returned quote.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be less than or equal to <paramref name="maxLength"/>.
		/// </exception>
		public QuoteSearchModel(
			int minLength = default,
			int? maxLength = default,
			string? tags = default,
			params string[]? authors
#pragma warning disable CS0618 // Type or member is obsolete
		) : this(minLength, maxLength, tags, authors, default)
#pragma warning restore CS0618 // Type or member is obsolete
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteSearchModel"/> class with a <paramref name="minLength"/>, <paramref name="maxLength"/>,
		/// <paramref name="tags"/>, <paramref name="authors"/> and <paramref name="authorIds"/> specified.
		/// </summary>
		/// <param name="minLength">Minimum length in characters of the quote.</param>
		/// <param name="maxLength">Maximum length in characters of the quote.</param>
		/// <param name="tags">Tags associated with the quote searched for.</param>
		/// <param name="authors">Collection of all possible authors of the returned quote.</param>
		/// <param name="authorIds">Collection of ids of all possible authors of the returned quote.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="maxLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="minLength"/> must be less than or equal to <paramref name="maxLength"/>.
		/// </exception>
		[JsonConstructor]
		public QuoteSearchModel(
			int minLength = default,
			int? maxLength = default,
			string? tags = default,
			string[]? authors = default,
			string[]? authorIds = default
		)
		{
			if (minLength < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(minLength), 0);
			}

			if (maxLength.HasValue)
			{
				if (maxLength < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(maxLength), 0);
				}

				if (minLength > maxLength)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(minLength), nameof(maxLength));
				}
			}

			_minLength = minLength;
			_maxLength = maxLength;
			_authors = authors;
			_authorIds = authorIds;
			Tags = tags;
		}

		/// <inheritdoc/>
		public virtual bool Equals(QuoteSearchModel? other)
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
				other._maxLength == _maxLength &&
				other._minLength == _minLength &&
				other.Tags == Tags &&
				Internals.SequenceEqual(other._authors, _authors) &&
				Internals.SequenceEqual(other._authorIds, other._authorIds);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(_maxLength);
			hash.Add(_minLength);
			hash.Add(Tags);
			hash.AddSequence(_authors);
			hash.AddSequence(_authorIds);

			return hash.ToHashCode();
		}
	}
}
