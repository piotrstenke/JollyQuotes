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
		Zero,

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
		One,

		/// <summary>
		/// At most two characters can be different.
		/// </summary>
		Two,

		/// <summary>
		/// Max threshold, which is <see cref="Two"/>.
		/// </summary>
		Max = Two
	}
}
