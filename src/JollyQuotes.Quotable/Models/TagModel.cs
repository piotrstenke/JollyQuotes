using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Represents a quote tag.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TagModel
	{
		private readonly string _id;
		private readonly string _name;
		private readonly int _quoteCount;
		private readonly int _v;
		private readonly DateTime _dateModified;

		/// <summary>
		/// Id of the tag.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("_id", Order = 0, Required = Required.Always)]
		public string Id
		{
			get => _id;
			init
			{
				if (string.IsNullOrEmpty(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_id = value;
			}
		}

		/// <summary>
		/// Name of the tag.
		/// </summary>
		/// <exception cref="ArgumentException">Value is <see langword="null"/> or empty.</exception>
		[JsonProperty("name", Order = 1, Required = Required.Always)]
		public string Name
		{
			get => _name;
			init
			{
				if (string.IsNullOrEmpty(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				_name = value;
			}
		}

		/// <summary>
		/// Number of quotes associated with this tag.
		/// </summary>
		[JsonProperty("quoteCount", Order = 2, Required = Required.Always)]
		public int QuoteCount
		{
			get => _quoteCount;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_quoteCount = value;
			}
		}

		/// <summary>
		/// Date the tag was added at.
		/// </summary>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateAdded", Order = 3, Required = Required.Always)]
		public DateTime DateAdded { get; init; }

		/// <summary>
		/// Date of the tag's most recent update.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="DateAdded"/>.</exception>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateModified", Order = 4)]
		public DateTime DateModified
		{
			get => _dateModified;
			init
			{
				if (value < DateAdded)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(DateAdded));
				}

				_dateModified = value;
			}
		}

		// This property is needed to properly deserialize response from quotable API.
		// I have no idea what does it represent.
		[JsonProperty("__v", Order = 5)]
#pragma warning disable IDE0051 // Remove unused private members
		private int V
#pragma warning restore IDE0051 // Remove unused private members
		{
			get => _v;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_v = 0;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagModel"/> class with <paramref name="id"/>, <paramref name="name"/>, <paramref name="quoteCount"/> and date of creation specified.
		/// </summary>
		/// <param name="id">Id of the tag.</param>
		/// <param name="name">Name of the tag.</param>
		/// <param name="quoteCount">Number of quotes associated with this tag.</param>
		/// <param name="dateAdded">Date the tag was added at.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="quoteCount"/> must be greater than or equal to <c>0</c>.</exception>
		public TagModel(
			string id,
			string name,
			int quoteCount,
			DateTime dateAdded
		) : this(id, name, quoteCount, dateAdded, dateAdded)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagModel"/> class with <paramref name="id"/>, <paramref name="name"/>, <paramref name="quoteCount"/> and dates of creation and last update specified.
		/// </summary>
		/// <param name="id">Id of the tag.</param>
		/// <param name="name">Name of the tag.</param>
		/// <param name="quoteCount">Number of quotes associated with this tag.</param>
		/// <param name="dateAdded">Date the tag was added at.</param>
		/// <param name="dateModified">Date of the tag's most recent update.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="id"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="name"/> is <see langword="null"/> or empty.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="quoteCount"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="dateModified"/> must be greater or equal to <paramref name="dateAdded"/>.
		/// </exception>
		public TagModel(
			string id,
			string name,
			int quoteCount,
			DateTime dateAdded,
			DateTime dateModified
		) : this(id, name, quoteCount, dateAdded, dateModified, default)
		{
		}

		[JsonConstructor]
		private TagModel(
			string id,
			string name,
			int quoteCount,
			DateTime dateAdded,
			DateTime dateModified,
			int v
		)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if (quoteCount < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(quoteCount), 0);
			}

			if (v < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(v), 0);
			}

			if (dateModified < dateAdded)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(dateModified), nameof(dateAdded));
			}

			_id = id;
			_name = name;
			_quoteCount = quoteCount;
			_dateModified = dateModified;
			_v = v;
			DateAdded = dateAdded;
		}
	}
}
