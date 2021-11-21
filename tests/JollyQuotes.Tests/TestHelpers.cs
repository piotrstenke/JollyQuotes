using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace JollyQuotes.Tests
{
	internal static class TestHelpers
	{
		private static readonly ConcurrentDictionary<string, HttpClient> _perAddressClients = new();
		private static HttpResolver? _globalResolver;

		public static HttpResolver GlobalResolver => _globalResolver ??= HttpResolver.CreateDefault();

		public static HttpClient GetClient(string address)
		{
			if (!_perAddressClients.TryGetValue(address, out HttpClient? client))
			{
				client = new HttpClient()
				{
					BaseAddress = new Uri(address),
				};

				client.DefaultRequestHeaders.Accept.Add(new("*/*"));

				_perAddressClients[address] = client;
			}

			return client;
		}

		public static HttpResolver GetResolver(string address)
		{
			HttpClient client = GetClient(address);

			return new HttpResolver(client);
		}
	}
}
