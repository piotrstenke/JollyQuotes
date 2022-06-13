namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Specifies number of terms an object must match in order to be included in the search result.
	/// </summary>
	public enum MatchThreshold
	{
		/// <summary>
		/// No threshold specified.
		/// </summary>
		None = 0,

		/// <summary>
		/// At least one matched term is required.
		/// </summary>
		One = 1,

		/// <summary>
		/// Minimum threshold, which is <see cref="One"/>.
		/// </summary>
		Min = One,

		/// <summary>
		/// At least two matched terms are required.
		/// </summary>
		Two = 2,

		/// <summary>
		/// Default value, which is <see cref="Two"/>.
		/// </summary>
		Default = Two,

		/// <summary>
		/// At least three matched terms are required.
		/// </summary>
		Three = 3,

		/// <summary>
		/// Maximum threshold, which is <see cref="Three"/>.
		/// </summary>
		Max = Three,
	}
}
