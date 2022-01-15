using System;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Base class for all <c>quotable</c> database objects.
	/// </summary>
	public abstract record DatabaseModel
	{
		private readonly DateTime _dateModified;

		/// <summary>
		/// Date the object was added at.
		/// </summary>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateAdded", Order = 100, Required = Required.Always)]
		public DateTime DateAdded { get; init; }

		/// <summary>
		/// Date of the object's most recent update.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="DateAdded"/>.</exception>
		[JsonConverter(typeof(Quote.DateOnlyConverter))]
		[JsonProperty("dateModified", Order = 101)]
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

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseModel"/> class.
		/// </summary>
		protected DatabaseModel()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseModel"/> class with a date of creation specified.
		/// </summary>
		/// <param name="dateAdded">Date the object was added at.</param>
		protected DatabaseModel(DateTime dateAdded)
		{
			DateAdded = dateAdded;
			_dateModified = dateAdded;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseModel"/> class with a dates of creation and last update specified.
		/// </summary>
		/// <param name="dateAdded">Date the object was added at.</param>
		/// <param name="dateModified">Date of the object's most recent update.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="dateModified"/> must be greater or equal to <paramref name="dateAdded"/>.</exception>
		protected DatabaseModel(DateTime dateAdded, DateTime dateModified)
		{
			if (dateModified < dateAdded)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(dateModified), nameof(dateAdded));
			}

			_dateModified = dateModified;
			DateAdded = dateAdded;
		}
	}
}
