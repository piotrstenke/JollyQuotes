using System;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace JollyQuotes
{
	internal static class Internals
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HttpResolver CreateResolver(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Throw.NullOrEmpty(nameof(source));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = new Uri(source) });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HttpResolver CreateResolver(Uri uri)
		{
			if (uri is null)
			{
				throw Throw.Null(nameof(uri));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = uri });
		}

		public static string RetrieveSourceFromClient(HttpClient client)
		{
			if (client is null)
			{
				throw Throw.Null(nameof(client));
			}

			string? str = client.BaseAddress?.ToString();

			if (string.IsNullOrWhiteSpace(str))
			{
				throw new ArgumentException("Base address of client cannot be null or empty when no source specified", nameof(client));
			}

			return str;
		}
	}
}
