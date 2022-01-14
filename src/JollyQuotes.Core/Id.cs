using System;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// Represents an id of a quote.
	/// </summary>
	[Serializable]
	[JsonConverter(typeof(Converter))]
	public readonly struct Id : IEquatable<Id>
	{
		/// <summary>
		/// Custom JSON converter for the <see cref="Id"/> struct.
		/// </summary>
		public sealed class Converter : JsonConverter<Id>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Convert"/> class.
			/// </summary>
			public Converter()
			{
			}

			/// <inheritdoc/>
			public override Id ReadJson(JsonReader reader, Type objectType, Id existingValue, bool hasExistingValue, JsonSerializer serializer)
			{
				string value = (string)reader.Value!;

				return new Id(value);
			}

			/// <inheritdoc/>
			public override void WriteJson(JsonWriter writer, Id value, JsonSerializer serializer)
			{
				writer.WriteValue(value.ToString());
			}
		}

		/// <summary>
		/// Actual id value.
		/// </summary>
		public string Value { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Id"/> struct with a specified underlaying <paramref name="value"/>.
		/// </summary>
		/// <param name="value"><see cref="int"/> to use as the value of the id.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be greater than or equal to <c>0</c>.</exception>
		public Id(int value) : this((long)value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id"/> struct with a specified underlaying <paramref name="value"/>.
		/// </summary>
		/// <param name="value"><see cref="uint"/> to use as the value of the id.</param>
		public Id(uint value) : this((ulong)value)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id"/> struct with a specified underlaying <paramref name="value"/>.
		/// </summary>
		/// <param name="value"><see cref="long"/> to use as the value of the id.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be greater than or equal to <c>0</c>.</exception>
		public Id(long value)
		{
			if (value < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
			}

			Value = value.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id"/> struct with a specified underlaying <paramref name="value"/>.
		/// </summary>
		/// <param name="value"><see cref="ulong"/> to use as the value of the id.</param>
		public Id(ulong value)
		{
			Value = value.ToString();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Id"/> struct with a specified udnder
		/// </summary>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or empty.</exception>
		public Id(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			Value = value;
		}

		/// <summary>
		/// Converts an <see cref="uint"/> value into a new instance of the <see cref="Id"/> struct.
		/// </summary>
		/// <param name="value"><see cref="uint"/> value to convert.</param>
		public static implicit operator Id(uint value)
		{
			return new Id(value);
		}

		/// <summary>
		/// Converts an <see cref="ulong"/> value into a new instance of the <see cref="Id"/> struct.
		/// </summary>
		/// <param name="value"><see cref="ulong"/> value to convert.</param>
		public static implicit operator Id(ulong value)
		{
			return new Id(value);
		}

		/// <summary>
		/// Converts an instance of the <see cref="Id"/> struct into a <see cref="string"/>.
		/// </summary>
		/// <param name="id"><see cref="Id"/> to convert.</param>
		public static implicit operator string(Id id)
		{
			return id.Value;
		}

		/// <summary>
		/// Checks whether two instances of the <see cref="Id"/> struct are considered not equal.
		/// </summary>
		/// <param name="left">First value to check.</param>
		/// <param name="right">Second value to check.</param>
		public static bool operator !=(Id left, Id right)
		{
			return !(left.Value == right.Value);
		}

		/// <summary>
		/// Checks whether two instances of the <see cref="Id"/> struct are considered equal.
		/// </summary>
		/// <param name="left">First value to check.</param>
		/// <param name="right">Second value to check.</param>
		public static bool operator ==(Id left, Id right)
		{
			return left.Value == right.Value;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			if (obj is Id id)
			{
				return id == this;
			}

			return false;
		}

		/// <inheritdoc/>
		public bool Equals(Id other)
		{
			return other == this;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			if (Value is null)
			{
				return 0;
			}

			return Value.GetHashCode();
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Value;
		}
	}
}
