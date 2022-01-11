using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace JollyQuotes
{
	internal static class Internals
	{
		public static bool ApplyQuery(ref string link, string query)
		{
			if(query.Length > 0)
			{
				link += $"?{query}";
				return true;
			}

			return false;
		}

		public static HttpClient CreateDefaultClient()
		{
			HttpClient client = new();
			client.DefaultRequestHeaders.Accept.Add(new("*/*"));
			return client;
		}

		public static HttpResolver CreateResolver(string source, bool includeBaseAddress)
		{
			if (!includeBaseAddress)
			{
				// In this case, 'source' is not used outside of constructor of RandomQuoteGenerator,
				// so it doesn't have to be checked against null before that constructor runs,
				// unlike 'uri' in method below, which is converted to string using ToString().

				return HttpResolver.CreateDefault();
			}

			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			HttpClient client = CreateDefaultClient();
			client.BaseAddress = new Uri(source);

			return new HttpResolver(client);
		}

		public static HttpResolver CreateResolver(Uri uri)
		{
			if (uri is null)
			{
				throw Error.Null(nameof(uri));
			}

			HttpClient client = CreateDefaultClient();
			client.BaseAddress = uri;

			return new HttpResolver(client);
		}

		public static void EnsureParameter(ref bool hasParam, StringBuilder builder)
		{
			if (hasParam)
			{
				builder.Append('&');
			}
			else
			{
				hasParam = true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IResourceResolver GetResolverFromService(IQuoteService service)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			return service.Resolver;
		}

		public static string RetrieveSourceFromClient(HttpClient client)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			string? str = client.BaseAddress?.ToString();

			if (string.IsNullOrWhiteSpace(str))
			{
				throw Error.Arg("Base address of client cannot be null or empty when no source specified", nameof(client));
			}

			return str;
		}

		public static bool TryDispose(object? obj)
		{
			if (obj is IDisposable disposable)
			{
				disposable.Dispose();
				return true;
			}

			return false;
		}
	}
}
