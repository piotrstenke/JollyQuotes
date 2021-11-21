namespace JollyQuotes
{
	/// <summary>
	/// Service for generating a random number from a specified range.
	/// </summary>
	public interface IRandomNumberGenerator
	{
		/// <summary>
		/// Generates a random number in range from inclusive <paramref name="min"/> to exclusive <paramref name="max"/>.
		/// </summary>
		/// <param name="min">Inclusive lower bound of value that can be generated.</param>
		/// <param name="max">Exclusive upper bound of value that can be generated.</param>
		int RandomNumber(int min, int max);
	}
}
