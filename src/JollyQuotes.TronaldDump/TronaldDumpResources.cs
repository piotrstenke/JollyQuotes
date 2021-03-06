using System;
using System.Net.Http;

namespace JollyQuotes.TronaldDump
{
	/// <summary>
	/// Contains links to important <c>Tronald Dump</c> resources.
	/// </summary>
	public static class TronaldDumpResources
	{
		/// <summary>
		/// Name of the API.
		/// </summary>
		public const string ApiName = "Tronald Dump";

		/// <summary>
		/// Entry point for API calls.
		/// </summary>
		public const string ApiPage = "https://www.tronalddump.io";

		/// <summary>
		/// Link to page with documentation of the <c>Tronald Dump</c> API.
		/// </summary>
		public const string DocsPage = "https://docs.tronalddump.io";

		/// <summary>
		/// Link to the GitHub page of the <c>Tronald Dump</c> source code.
		/// </summary>
		public const string GitHubPage = "https://github.com/tronalddump-io/tronald-app";

		/// <summary>
		/// Defines the maximum number of items that can be present on a single page of a search result.
		/// </summary>
		public const int MaxItemsPerPage = 10;

		/// <summary>
		/// Format of a meme image returned by <see cref="ITronaldDumpService.GetRandomMeme"/>.
		/// </summary>
		public const string MemeFormat = "JPEG";

		/// <summary>
		/// Height in pixels of a meme image returned by <see cref="ITronaldDumpService.GetRandomMeme"/>.
		/// </summary>
		public const int MemeHeight = 768;

		/// <summary>
		/// Width in pixels of a meme image returned by <see cref="ITronaldDumpService.GetRandomMeme"/>.
		/// </summary>
		public const int MemeWidth = 1024;

		internal const string BASE_ADDRESS = ApiPage;

		internal static HttpResolver CreateDefaultResolver()
		{
			HttpClient client = Internals.CreateDefaultClient();
			client.BaseAddress = new Uri(BASE_ADDRESS);
			return new HttpResolver(client);
		}
	}
}
