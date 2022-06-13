namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Specifies maximum number of single-character edits required to match a given search term.
	/// </summary>
	public enum FuzzyMatchingTreshold
	{
		/// <summary>
		/// Fuzzy matching is disabled.
		/// </summary>
		Zero = 0,

		/// <summary>
		/// Minimum threshold, which is <see cref="Zero"/>.
		/// </summary>
		Min = Zero,

		/// <summary>
		/// Default threshold, which is <see cref="Zero"/>.
		/// </summary>
		Default = Zero,

		/// <summary>
		/// At most one character can be different.
		/// </summary>
		One = 1,

		/// <summary>
		/// At most two characters can be different.
		/// </summary>
		Two = 2,

		/// <summary>
		/// Max threshold, which is <see cref="Two"/>.
		/// </summary>
		Max = Two
	}
}
