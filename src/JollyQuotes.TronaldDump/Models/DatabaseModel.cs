using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Base class for all <c>Tronald Dump</c> database objects.
	/// </summary>
	public abstract record DatabaseModel
	{
		private readonly DateTime _updatedAt;

		/// <summary>
		/// Date the object was added at.
		/// </summary>
		[JsonProperty("created_at", Order = 100, Required = Required.Always)]
		public DateTime CreatedAt { get; init; }

		/// <summary>
		/// Date of the object's most recent update.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <see cref="CreatedAt"/>.</exception>
		[JsonProperty("updated_at", Order = 101)]
		public DateTime UpdatedAt
		{
			get => _updatedAt;
			init
			{
				if (value < CreatedAt)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(CreatedAt));
				}

				_updatedAt = value;
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
		/// <param name="createdAt">Date the object was added at.</param>
		protected DatabaseModel(DateTime createdAt)
		{
			CreatedAt = createdAt;
			_updatedAt = createdAt;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DatabaseModel"/> class with a dates of creation and last update specified.
		/// </summary>
		/// <param name="createdAt">Date the object was added at.</param>
		/// <param name="updatedAt">Date of the object's most recent update.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="updatedAt"/> must be greater or equal to <paramref name="createdAt"/>.</exception>
		protected DatabaseModel(DateTime createdAt, DateTime updatedAt)
		{
			if (updatedAt < createdAt)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(updatedAt), nameof(createdAt));
			}

			_updatedAt = updatedAt;
			CreatedAt = createdAt;
		}
	}
}
