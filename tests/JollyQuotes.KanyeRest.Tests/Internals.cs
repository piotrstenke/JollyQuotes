using System.Net.Http;


namespace JollyQuotes.KanyeRest.Tests
{
	internal static class Internals
	{
		public static HttpClient HttpClient { get; } = new();
	}
}
