namespace JollyQuotes
{
	/// <summary>
	/// Provides a method that returns a random <see cref="bool"/> value.
	/// </summary>
	public interface IPossibility
	{
		/// <summary>
		/// Returns a random <see cref="bool"/> value.
		/// </summary>
		bool Determine();
	}
}
