using System;

namespace JollyQuotes
{
	/// <summary>
	/// Contains name of an option and its possibility.
	/// </summary>
	public readonly struct NamedOption : IEquatable<NamedOption>, IComparable<NamedOption>
	{
		/// <summary>
		/// Name of the option.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Possibility of this option occurring.
		/// </summary>
		public int Possibility { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="NamedOption"/> struct with <paramref name="name"/> and <paramref name="possibility"/> specified.
		/// </summary>
		/// <param name="name">Name of the option.</param>
		/// <param name="possibility">Possibility of this option occurring.</param>
		/// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>
		public NamedOption(string name, int possibility)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			Name = name;
			Possibility = possibility;
		}

		/// <summary>
		/// Deconstructs the struct into its underlaying values.
		/// </summary>
		public void Deconstruct(out string name, out int possibility)
		{
			name = Name;
			possibility = Possibility;
		}

		/// <inheritdoc/>
		public override bool Equals(object? obj)
		{
			if (obj is NamedOption other)
			{
				return other == this;
			}

			return false;
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{Name}, {Possibility}";
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return HashCode.Combine(Name, Possibility);
		}

		/// <inheritdoc/>
		public bool Equals(NamedOption other)
		{
			return other == this;
		}

		/// <inheritdoc/>
		public int CompareTo(NamedOption other)
		{
			return other.Possibility.CompareTo(Possibility);
		}

		/// <summary>
		/// Checks if two instances of <see cref="NamedOption"/> are considered equal.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator ==(in NamedOption left, in NamedOption right)
		{
			return left.Name == right.Name && left.Possibility == right.Possibility;
		}

		/// <summary>
		/// Checks if two instances of <see cref="NamedOption"/> are considered not equal.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator !=(in NamedOption left, in NamedOption right)
		{
			return !(left == right);
		}

		/// <summary>
		/// Checks if one instance of <see cref="NamedOption"/> is considered being greater than the second.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator >(in NamedOption left, in NamedOption right)
		{
			return left.Possibility > right.Possibility;
		}

		/// <summary>
		/// Checks if instance of <see cref="NamedOption"/> is considered being greater than an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/>.</param>
		/// <param name="right"><see cref="int"/> to compare to.</param>
		public static bool operator >(in NamedOption left, int right)
		{
			return left.Possibility > right;
		}

		/// <summary>
		/// Checks if an <see cref="int"/> value is considered being greater than an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left"><see cref="int"/> to compare to.</param>
		/// <param name="right">Instance of <see cref="NamedOption"/>.</param>
		public static bool operator >(int left, in NamedOption right)
		{
			return left > right.Possibility;
		}

		/// <summary>
		/// Checks if one instance of <see cref="NamedOption"/> is considered being less than the second.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator <(in NamedOption left, in NamedOption right)
		{
			return left.Possibility < right.Possibility;
		}

		/// <summary>
		/// Checks if instance of <see cref="NamedOption"/> is considered being less than an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/>.</param>
		/// <param name="right"><see cref="int"/> to compare to.</param>
		public static bool operator <(in NamedOption left, int right)
		{
			return left.Possibility < right;
		}

		/// <summary>
		/// Checks if an <see cref="int"/> value is considered being less than an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left"><see cref="int"/> to compare to.</param>
		/// <param name="right">Instance of <see cref="NamedOption"/>.</param>
		public static bool operator <(int left, in NamedOption right)
		{
			return left < right.Possibility;
		}

		/// <summary>
		/// Checks if one instance of <see cref="NamedOption"/> is considered being greater than or equal the second.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator >=(in NamedOption left, in NamedOption right)
		{
			return left.Possibility >= right.Possibility;
		}

		/// <summary>
		/// Checks if instance of <see cref="NamedOption"/> is considered being greater than or equal to an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/>.</param>
		/// <param name="right"><see cref="int"/> to compare to.</param>
		public static bool operator >=(in NamedOption left, int right)
		{
			return left.Possibility >= right;
		}

		/// <summary>
		/// Checks if an <see cref="int"/> value is considered being greater than or equal to an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left"><see cref="int"/> to compare to.</param>
		/// <param name="right">Instance of <see cref="NamedOption"/>.</param>
		public static bool operator >=(int left, in NamedOption right)
		{
			return left >= right.Possibility;
		}

		/// <summary>
		/// Checks if one instance of <see cref="NamedOption"/> is considered being less than or equal to the second.
		/// </summary>
		/// <param name="left">First instance of <see cref="NamedOption"/>.</param>
		/// <param name="right">Second instance of <see cref="NamedOption"/>.</param>
		public static bool operator <=(in NamedOption left, in NamedOption right)
		{
			return left.Possibility <= right.Possibility;
		}

		/// <summary>
		/// Checks if instance of <see cref="NamedOption"/> is considered being less than or equal to an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/>.</param>
		/// <param name="right"><see cref="int"/> to compare to.</param>
		public static bool operator <=(in NamedOption left, int right)
		{
			return left.Possibility <= right;
		}

		/// <summary>
		/// Checks if an <see cref="int"/> value is considered being less than or equal to an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left"><see cref="int"/> to compare to.</param>
		/// <param name="right">Instance of <see cref="NamedOption"/>.</param>
		public static bool operator <=(int left, in NamedOption right)
		{
			return left <= right.Possibility;
		}

		/// <summary>
		/// Adds an <see cref="int"/> value to an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to add the <see cref="int"/> value to.</param>
		/// <param name="right"><see cref="int"/> value to add.</param>
		public static NamedOption operator +(in NamedOption left, int right)
		{
			return new NamedOption(left.Name, left.Possibility + right);
		}

		/// <summary>
		/// Subtracts an <see cref="int"/> value from an instance of <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to subtract the <see cref="int"/> value from.</param>
		/// <param name="right"><see cref="int"/> value to subtract.</param>
		public static NamedOption operator -(in NamedOption left, int right)
		{
			return new NamedOption(left.Name, left.Possibility - right);
		}

		/// <summary>
		/// Multiplies an instance of <see cref="NamedOption"/> by an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to multiply.</param>
		/// <param name="right"><see cref="int"/> value to multiply by.</param>
		public static NamedOption operator *(in NamedOption left, int right)
		{
			return new NamedOption(left.Name, left.Possibility * right);
		}

		/// <summary>
		/// Divides an instance of <see cref="NamedOption"/> by an <see cref="int"/> value.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to divide.</param>
		/// <param name="right"><see cref="int"/> value to divide by.</param>
		public static NamedOption operator /(in NamedOption left, int right)
		{
			return new NamedOption(left.Name, left.Possibility / right);
		}

		/// <summary>
		/// Increments the possibility of an <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to increment the possibility of.</param>
		public static NamedOption operator ++(in NamedOption left)
		{
			return new NamedOption(left.Name, left.Possibility + 1);
		}

		/// <summary>
		/// Decrements the possibility of an <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="left">Instance of <see cref="NamedOption"/> to decrement the possibility of.</param>
		public static NamedOption operator --(in NamedOption left)
		{
			return new NamedOption(left.Name, left.Possibility - 1);
		}

	}
}
