namespace JollyQuotes
{
	/// <summary>
	/// Specifies which quotes to include in the search.
	/// </summary>
	public enum QuoteInclude
	{
		/// <summary>
		/// Returns available quotes, either from the generator's source or cache.
		/// </summary>
		All,

		/// <summary>
		/// Returns quotes that are cached.
		/// </summary>
		Cached,

		/// <summary>
		/// Returns quotes directly from the generator's source.
		/// </summary>
		Download
	}
}
