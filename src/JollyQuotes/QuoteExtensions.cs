using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// Contains various utility extension methods widely used through the <c>JollyQuotes</c> libraries.
	/// </summary>
	public static class QuoteExtensions
	{
		/// <summary>
		/// Downloads json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="client">Client that is used to retrieve the json data.</param>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="InvalidOperationException">Object could not be deserialized from <paramref name="source"/>.</exception>
		public static async Task<T> DownloadAndDeserialize<T>(this HttpClient client, string source)
		{
			HttpResponseMessage response = await client.GetAsync(source);
			string json = await response.Content.ReadAsStringAsync();
			T? t = JsonConvert.DeserializeObject<T>(json, Settings.JsonSettings);

			if (t is null)
			{
				throw new InvalidOperationException($"Object could not be deserialized from source '{source}'");
			}

			return t;
		}

		/// <summary>
		/// Converts the specified <paramref name="quote"/> to a new instance of the <see cref="Quote"/> class.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to convert.</param>
		public static Quote ToGenericQuote(this IQuote quote)
		{
			return new Quote(quote.Value, quote.Author, quote.Source, quote.Date);
		}
	}
}
