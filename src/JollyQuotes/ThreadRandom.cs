using System;

namespace JollyQuotes
{
	/// <summary>
	/// A thread-static, thread-safe random number generator.
	/// </summary>
	public sealed class ThreadRandom : IRandomNumberGenerator
	{
		private static readonly Random _seedGenerator = new();

		[ThreadStatic]
		private static Random? _random;

		/// <summary>
		/// Provides access to a random number generator assigned to the current thread.
		/// </summary>
		public static Random Random
		{
			get
			{
				if (_random is null)
				{
					int seed;

					lock (_seedGenerator)
					{
						seed = _seedGenerator.Next();
					}

					_random = new Random(seed);
				}

				return _random;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ThreadRandom"/> class.
		/// </summary>
		public ThreadRandom()
		{
		}

		/// <summary>
		/// Applies the specified <paramref name="seed"/> to random number generator assigned to the current thread.
		/// </summary>
		/// <param name="seed">
		/// A number used to calculate a starting value for the pseudo-random number sequence.
		/// If a negative number is specified, the absolute value of the number is used.
		/// </param>
		public static void Initialize(int seed)
		{
			_random = new(seed);
		}

#pragma warning disable CA1822 // Mark members as static

		/// <inheritdoc cref="Random.Next()"/>
		public int Next()
		{
			return Random.Next();
		}

		/// <inheritdoc cref="Random.Next(int)"/>
		public int Next(int maxValue)
		{
			return Random.Next(maxValue);
		}

		/// <inheritdoc cref="Random.Next(int, int)"/>
		public int Next(int minValue, int maxValue)
		{
			return Random.Next(minValue, maxValue);
		}

		/// <inheritdoc cref="Random.NextBytes(byte[])"/>
		public void NextBytes(byte[] buffer)
		{
			Random.NextBytes(buffer);
		}

		/// <inheritdoc cref="Random.NextBytes(Span{byte})"/>
		public void NextBytes(Span<byte> buffer)
		{
			Random.NextBytes(buffer);
		}

		/// <inheritdoc cref="Random.NextDouble"/>
		public double NextDouble()
		{
			return Random.NextDouble();
		}

#pragma warning restore CA1822 // Mark members as static

		int IRandomNumberGenerator.RandomNumber(int min, int max)
		{
			return Next(min, max);
		}
	}
}
