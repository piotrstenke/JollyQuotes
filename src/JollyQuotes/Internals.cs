﻿using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace JollyQuotes
{
	internal static class Internals
	{
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

		public static void EnsureNullAddress(HttpClient client)
		{
			if (client.BaseAddress is not null)
			{
				throw new ArgumentException("Base address of a HtppClient must be null", nameof(client));
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
