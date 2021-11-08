namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Contains links to important <c>Tronald Dump</c> resources.
	/// </summary>
	public static class TronaldDumpResources
	{
		/// <summary>
		/// Entry point for API calls.
		/// </summary>
		public const string APIPage = "https://www.tronalddump.io/";

		/// <summary>
		/// Link to page with documentation of the <c>Tronald Dump</c> API.
		/// </summary>
		public const string DocsPage = "https://docs.tronalddump.io/";

		/// <summary>
		/// Link to the GitHub page of the <c>Tronald Dump</c> app.
		/// </summary>
		public const string GitHubPage = "https://github.com/tronalddump-io/tronald-app";

		/// <summary>
		/// Defines the maximum number of items that can be present on a single page of a search result.
		/// </summary>
		public const int MaxItemsPerPage = 10;

		internal const string BaseAddress = APIPage;

		internal const string BaseAddressSource =
			nameof(JollyQuotes) + "." +
			nameof(TronaldDump) + "." +
			nameof(TronaldDumpResources) + "." +
			nameof(TronaldDumpResources.APIPage);
	}
}
