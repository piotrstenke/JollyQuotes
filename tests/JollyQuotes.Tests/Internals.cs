using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;

namespace JollyQuotes.Tests
{
	internal static class Internals
	{
		private static readonly ConcurrentDictionary<string, HttpClient> _perAddressClients = new();

		public static HttpClient GlobalClient
		{
			get
			{
				HttpClient client = new();
				client.DefaultRequestHeaders.Accept.Add(new("*/*"));

				return client;
			}
		}

		public static HttpResolver GlobalResolver { get; } = new(GlobalClient);

		public static Random RandomNumber { get; } = new();

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
