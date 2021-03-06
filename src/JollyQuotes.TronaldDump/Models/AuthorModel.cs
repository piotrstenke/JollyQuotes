using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents an author of the current resource.
	/// </summary>
	[JsonObject]
	public sealed record AuthorModel : DatabaseModel
	{
		private readonly SelfLinkModel _links;
		private readonly string _name;
		private readonly string _slug;
		private readonly string _id;

		/// <summary>
		/// Bio of the author.
		/// </summary>
		[JsonProperty("bio", Order = 3)]
		public string? Bio { get; init; }

		/// <summary>
		/// Id of the author.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("author_id", Order = 0, Required = Required.Always)]
		public string Id
		{
			get => _id;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_id = value;
			}
		}

		/// <summary>
		/// Link to the author data.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_links", Order = 4, Required = Required.Always)]
		public SelfLinkModel Links
		{
			get => _links;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_links = value;
			}
		}

		/// <summary>
		/// Name of the author.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("name", Order = 1, Required = Required.Always)]
		public string Name
		{
			get => _name;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_name = value;
			}
		}

		/// <summary>
		/// Slugified version of the <see cref="Name"/>.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("slug", Order = 2, Required = Required.Always)]
		public string Slug
		{
			get => _slug;
			init
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_slug = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorModel"/> class with author's <paramref name="id"/>, <paramref name="name"/>, <paramref name="slug"/>, <paramref name="bio"/>, self <paramref name="links"/> and date of creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the author.</param>
		/// <param name="name">Name of the author.</param>
		/// <param name="slug">Slugified version of the <see cref="Name"/>.</param>
		/// <param name="bio">Bio of the author.</param>
		/// <param name="links">Link to the author data.</param>
		/// <param name="createdAt">Date the data of the author was created at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="slug"/> is <see langword="null"/> or empty.
		/// </exception>
		public AuthorModel(
			string id,
			string name,
			string slug,
			string? bio,
			SelfLinkModel links,
			DateTime createdAt
		) : this(id, name, slug, bio, links, createdAt, createdAt)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthorModel"/> class with author's <paramref name="id"/>, <paramref name="name"/>, <paramref name="slug"/>, <paramref name="bio"/>, self <paramref name="links"/> and date of creation specified.
		/// </summary>
		/// <param name="id">Id of the author.</param>
		/// <param name="name">Name of the author.</param>
		/// <param name="slug">Slugified version of the <see cref="Name"/>.</param>
		/// <param name="bio">Bio of the author.</param>
		/// <param name="links">Link to the author data.</param>
		/// <param name="createdAt">Date the data of the author was created at.</param>
		/// <param name="updatedAt">Date the data of the author was updated at.</param>
		/// <exception cref="ArgumentNullException"><paramref name="links"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="slug"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater than or equal to <paramref name="createdAt"/>.</exception>
		[JsonConstructor]
		public AuthorModel(
			string id,
			string name,
			string slug,
			string? bio,
			SelfLinkModel links,
			DateTime createdAt,
			DateTime updatedAt
		) : base(createdAt, updatedAt)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if (string.IsNullOrWhiteSpace(slug))
			{
				throw Error.NullOrEmpty(nameof(slug));
			}

			if (links is null)
			{
				throw Error.Null(nameof(links));
			}

			_id = id;
			_name = name;
			_slug = slug;
			Bio = bio;
			_links = links;
		}
	}
}
