using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;

namespace JollyQuotes
{
	internal static class Internals
	{
		public static void AddSequence<T>(this ref HashCode hash, IEnumerable<T>? collection)
		{
			if(collection is not null)
			{
				foreach (T item in collection)
				{
					hash.Add(item);
				}
			}
		}

		public static void AddSequence(this ref HashCode hash, IEnumerable? collection)
		{
			if (collection is not null)
			{
				foreach (object item in collection)
				{
					hash.Add(item);
				}
			}
		}

		public static void ApplyParameter(this StringBuilder builder)
		{
			builder.Append('&');
		}

		public static bool ApplyQuery(ref string link, string query)
		{
			if (query.Length > 0)
			{
				link += $"?{query}";
				return true;
			}

			return false;
		}

		public static bool SequenceEqual(string[]? left, string[]? right)
		{
			if (left is null)
			{
				return right is null;
			}

			if (right is null)
			{
				return false;
			}

			return left.Length == right.Length && left.SequenceEqual(right);
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

		public static void EnsureParameter(this StringBuilder builder, ref bool hasParam)
		{
			if (hasParam)
			{
				ApplyParameter(builder);
			}
			else
			{
				hasParam = true;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IResourceResolver GetResolverFromService(this IQuoteService service)
		{
			if (service is null)
			{
				throw Error.Null(nameof(service));
			}

			return service.Resolver;
		}

		public static string RetrieveSourceFromClient(this HttpClient client)
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

		public static void SetFirstElement(string? value, ref string[]? array)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				array = null;
				return;
			}

			if (array is null || array.Length == 0)
			{
				array = new string[] { value };
				return;
			}

			string[] newArray = new string[array.Length];
			array.CopyTo(newArray, 0);
			newArray[0] = value;
			array = newArray;
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
