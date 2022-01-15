using System;
using System.Net.Http;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Contains links to important <c>quotable</c> resources.
	/// </summary>
	public static class QuotableResources
	{
		/// <summary>
		/// Name of the API.
		/// </summary>
		public const string ApiName = "quotable";

		/// <summary>
		/// Entry point for API calls.
		/// </summary>
		public const string ApiPage = "https://api.quotable.io";

		/// <summary>
		/// Link to page with documentation of the <c>quotable</c> API.
		/// </summary>
		public const string DocsPage = "https://github.com/lukePeavey/quotable/blob/master/README.md";

		/// <summary>
		/// Default number of fuzzy matching expansions.
		/// </summary>
		public const int FuzzyDefaultExpansions = 50;

		/// <summary>
		/// Maximum number of fuzzy matching expansions.
		/// </summary>
		public const int FuzzyMaxExpansions = 150;

		/// <summary>
		/// Link to the GitHub page of the <c>quotable</c> source code.
		/// </summary>
		public const string GitHubPage = "https://github.com/lukePeavey/quotable";

		/// <summary>
		/// Default number of results on a response page.
		/// </summary>
		public const int ResultsPerPageDefault = 20;

		/// <summary>
		/// Maximum number of results on a response page.
		/// </summary>
		public const int ResultsPerPageMax = 150;

		/// <summary>
		/// Minimum number of results on a response page.
		/// </summary>
		public const int ResultsPerPageMin = 1;

		internal const string AUTHOR_ID_OBSOLETE = "Author id was deprecated by the maintainers of quotable.io. ";

		internal const string BASE_ADDRESS = ApiPage;

		internal const string MAX_FUZZY_EXPANSIONS_NAME = nameof(QuotableResources) + "." + nameof(FuzzyMaxExpansions);
		internal const string MAX_RESULTS_PER_PAGE_NAME = nameof(QuotableResources) + "." + nameof(ResultsPerPageMax);

		internal static HttpResolver CreateDefaultResolver()
		{
			HttpClient client = Internals.CreateDefaultClient();
			client.BaseAddress = new Uri(BASE_ADDRESS);
			return new HttpResolver(client);
		}
	}
}
