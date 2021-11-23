using System;

namespace JollyQuotes
{
	/// <summary>
	/// Generates a random <see cref="bool"/> value.
	/// </summary>
	public class Possibility : IPossibility
	{
		/// <summary>
		/// A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.
		/// </summary>
		public IRandomNumberGenerator Random { get; }

		/// <summary>
		/// If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>.
		/// </summary>
		/// <remarks>
		/// If this value is equal to <c>1</c>, <see langword="true"/> is always returned.
		/// Accordingly, if it is equal to <see cref="UpperLimit"/>, <see langword="false"/> is always returned.
		/// <para>The default value is <c>50</c>.</para>
		/// </remarks>
		public int Step { get; private set; }

		/// <summary>
		/// Greatest number that can be generated. Must be greater than or equal to <see cref="Step"/>.
		/// </summary>
		/// <remarks>The default value is <c>100</c>.</remarks>
		public int UpperLimit { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class.
		/// </summary>
		public Possibility()
		{
			Random = new ThreadRandom();
			Reset();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class with an underlaying random number generator specified.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
		public Possibility(IRandomNumberGenerator random)
		{
			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			Random = random;
			Reset();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class with an underlaying random number generator and <paramref name="upperLimit"/> specified.
		/// <see cref="Step"/> is set to half of the <paramref name="upperLimit"/>.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> must be greater than <c>0</c>.</exception>
		public Possibility(IRandomNumberGenerator random, int upperLimit)
		{
			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			Random = random;
			Bound(upperLimit);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class with an underlaying random number generator,
		/// <paramref name="step"/> and <paramref name="upperLimit"/> specified.
		/// </summary>
		/// <param name="random">A random number generator that is used to determine a <see cref="bool"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c> and greater than or equal to <paramref name="step"/>.</param>
		/// <param name="step">If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>. Must be greater than <c>0</c>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="step"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than or equal to <paramref name="step"/>.
		/// </exception>
		public Possibility(IRandomNumberGenerator random, int upperLimit, int step)
		{
			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			Bound(upperLimit, step);
			Random = random;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class with an <paramref name="upperLimit"/> specified.
		/// <see cref="Step"/> is set to half of the <paramref name="upperLimit"/>.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c>.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> must be greater than <c>0</c>.</exception>
		public Possibility(int upperLimit)
		{
			Bound(upperLimit);
			Random = new ThreadRandom();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Possibility"/> class with a <paramref name="step"/> and <paramref name="upperLimit"/> specified.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c> and greater than or equal to <paramref name="step"/>.</param>
		/// <param name="step">If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>. Must be greater than <c>0</c>.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="step"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than or equal to <paramref name="step"/>.
		/// </exception>
		public Possibility(int upperLimit, int step)
		{
			Bound(upperLimit, step);
			Random = new ThreadRandom();
		}

		/// <summary>
		/// Sets the <see cref="UpperLimit"/> to the specified value. <see cref="Step"/> is set to half of that value.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c>.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="upperLimit"/> must be greater than <c>0</c>.</exception>
		public void Bound(int upperLimit)
		{
			if (upperLimit < 1)
			{
				throw Error.MustBeGreaterThan(nameof(upperLimit), 0);
			}

			UpperLimit = upperLimit;
			Step = upperLimit == 1 ? 1 : upperLimit / 2;
		}

		/// <summary>
		/// Sets the <see cref="Step"/> and <see cref="UpperLimit"/> to the specified values.
		/// </summary>
		/// <param name="upperLimit">Greatest number that can be generated. Must greater than <c>0</c> and greater than or equal to <paramref name="step"/>.</param>
		/// <param name="step">If a generated number is greater than or equal to this value, <see langword="true"/> is returned, otherwise <see langword="false"/>. Must be greater than <c>0</c>.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="step"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than <c>0</c>. -or-
		/// <paramref name="upperLimit"/> must be greater than or equal to <paramref name="step"/>.</exception>
		public void Bound(int upperLimit, int step)
		{
			if (step < 1)
			{
				throw Error.MustBeGreaterThan(nameof(step), 0);
			}

			if (upperLimit < 1)
			{
				throw Error.MustBeGreaterThan(nameof(upperLimit), 0);
			}

			if (upperLimit < step)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(upperLimit), nameof(step));
			}

			Step = step;
			UpperLimit = upperLimit;
		}

		/// <inheritdoc/>
		public virtual bool Determine()
		{
			return Random.RandomNumber(1, UpperLimit + 1) > Step;
		}

		/// <summary>
		/// Resets the <see cref="Step"/> and <see cref="UpperLimit"/> to their default values.
		/// </summary>
		public virtual void Reset()
		{
			UpperLimit = 100;
			Step = 50;
		}
	}
}
