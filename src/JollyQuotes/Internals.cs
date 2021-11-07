using System;
using System.Diagnostics;
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
				throw NullOrEmpty(nameof(source));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = new Uri(source) });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static HttpResolver CreateResolver(Uri uri)
		{
			if (uri is null)
			{
				throw Null(nameof(uri));
			}

			return new HttpResolver(new HttpClient() { BaseAddress = uri });
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException MustBeGreaterThan(string paramName, object value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than '{value}'");
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException MustBeGreaterThanOrEqualTo(string paramName, object value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than or equal to '{value}'");
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentNullException Null(string paramName)
		{
			return new ArgumentNullException(paramName);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException NullOrEmpty(string paramName)
		{
			return new ArgumentException($"{paramName} cannot be null or empty", paramName);
		}

		public static string RetrieveSourceFromClient(HttpClient client)
		{
			if (client is null)
			{
				throw Null(nameof(client));
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
