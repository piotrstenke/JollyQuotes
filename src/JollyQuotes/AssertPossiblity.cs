using System;

namespace JollyQuotes
{
	/// <summary>
	/// Generates a random <see cref="bool"/> based on either static assertion or the inherited <see cref="Possibility"/> implementation if that assertion fails.
	/// </summary>
	public class AssertPossiblity : Possibility
	{
		/// <summary>
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </summary>
		public Func<bool> Assertion { get; }

		/// <summary>
		/// A <see cref="bool"/> value that is returned by <see cref="Determine"/> if <see cref="Assertion"/> returns <see langword="true"/>.
		/// </summary>
		/// <remarks>The default value is <see langword="true"/>.</remarks>
		public bool BoolValue { get; set; } = true;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with an underlaying <paramref name="assertion"/> function specified.
		/// </summary>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
		public AssertPossiblity(Func<bool> assertion) : this(new ThreadRandom(), assertion)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with an underlaying <paramref name="assertion"/> function random number generator specified.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>. -or- <paramref name="assertion"/> is <see langword="null"/>.</exception>
		public AssertPossiblity(IRandomNumberGenerator random, Func<bool> assertion) : base(random)
		{
			if (assertion is null)
			{
				throw Error.Null(nameof(assertion));
			}

			Assertion = assertion;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with an underlaying random number generator and <paramref name="upperLimit"/> specified.
		/// <see cref="Possibility.Step"/> is set to half of the <paramref name="upperLimit"/>.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c>.</param>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>. -or- <paramref name="assertion"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> must be greater than <c>0</c>.</exception>
		public AssertPossiblity(IRandomNumberGenerator random, int upperLimit, Func<bool> assertion) : base(random, upperLimit)
		{
			if (assertion is null)
			{
				throw Error.Null(nameof(assertion));
			}

			Assertion = assertion;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with an underlaying random number generator,
		/// <paramref name="step"/> and <paramref name="upperLimit"/> specified.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c> and greater than or equal to <paramref name="step"/>.</param>
		/// <param name="step">If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>. Must be greater than <c>0</c>.</param>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>. -or- <paramref name="assertion"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="step"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than or equal to <paramref name="step"/>.
		/// </exception>
		public AssertPossiblity(IRandomNumberGenerator random, int upperLimit, int step, Func<bool> assertion) : base(random, upperLimit, step)
		{
			if (assertion is null)
			{
				throw Error.Null(nameof(assertion));
			}

			Assertion = assertion;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with an <paramref name="upperLimit"/> specified.
		/// <see cref="Possibility.Step"/> is set to half of the <paramref name="upperLimit"/>.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c>.</param>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> must be greater than <c>0</c>.</exception>
		public AssertPossiblity(int upperLimit, Func<bool> assertion) : this(new ThreadRandom(), upperLimit, assertion)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssertPossiblity"/> class with a <paramref name="step"/> and <paramref name="upperLimit"/> specified.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c> and greater than or equal to <paramref name="step"/>.</param>
		/// <param name="step">If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>. Must be greater than <c>0</c>.</param>
		/// <param name="assertion">
		/// Function that determines whether the static <see cref="BoolValue"/> should be returned by <see cref="Determine"/>.
		/// If <see langword="false"/>, the inherited <see cref="Possibility.Determine"/> implementation is used.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="assertion"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="step"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than or equal to <paramref name="step"/>.
		/// </exception>
		public AssertPossiblity(int upperLimit, int step, Func<bool> assertion) : this(new ThreadRandom(), upperLimit, step, assertion)
		{
		}

		/// <inheritdoc/>
		public override bool Determine()
		{
			if (Assertion())
			{
				return BoolValue;
			}

			return base.Determine();
		}
	}
}
