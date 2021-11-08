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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IResourceResolver GetResolverFromClient(HttpClient client, out IResourceResolver resolver)
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

		public static void PrintArray(StringBuilder builder, string propertyName, object[] array)
		{
			builder.Append(propertyName);
			builder.Append(" = { ");

			if (array.Length > 0)
			{
				builder.Append(" 0 = ");
				builder.Append(array[0].ToString());

				for (int i = 1; i < array.Length; i++)
				{
					builder.Append(", ");
					builder.Append(i);
					builder.Append(" = ");
					builder.Append(array[i].ToString());
				}

				builder.Append(' ');
			}

			builder.Append('}');
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
	}
}
