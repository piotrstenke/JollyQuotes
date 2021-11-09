using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace JollyQuotes
{
	internal static class Internals
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HttpResolver CreateResolver(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = new Uri(source) });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HttpResolver CreateResolver(Uri uri)
		{
			if (uri is null)
			{
				throw Error.Null(nameof(uri));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = uri });
		}

		public static HttpClient EnsureNullAddress(HttpClient client)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			if (client.BaseAddress is not null)
			{
				throw new ArgumentException("Base address of a HtppClient must be null", nameof(client));
			}

			return client;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IResourceResolver GetResolverFromClient(HttpClient client, out IStreamResolver resolver)
		{
			resolver = new HttpResolver(client);
			return resolver;
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
				throw new ArgumentException("Base address of client cannot be null or empty when no source specified", nameof(client));
			}

			return str;
		}

		public static HttpClient SetAddress(HttpClient client, string address)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			if (client.BaseAddress is not null && client.BaseAddress.OriginalString != address)
			{
				throw new ArgumentException($"BaseAddress of a HttpClient must be null or equal to '{address}'");
			}

			client.BaseAddress = new Uri(address);

			return client;
		}

		public static HttpClient SetAddress(HttpClient client, string address, string addressSource)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			if (client.BaseAddress is not null && client.BaseAddress.OriginalString != address)
			{
				throw new ArgumentException($"BaseAddress of a HttpClient must be null or equal to the value of {addressSource}");
			}

			client.BaseAddress = new Uri(address);

			return client;
		}
	}
}
