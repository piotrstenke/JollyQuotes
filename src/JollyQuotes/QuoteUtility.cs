using System;
using System.Collections.Generic;
using System.Reflection;

namespace JollyQuotes
{
	/// <summary>
	/// Contains various utility methods helpful when dealing with quotes.
	/// </summary>
	public static class QuoteUtility
	{
		private const int NUM_QUOTE_APIS = 3;

		/// <summary>
		/// Returns a value of <see cref="JollyQuotesAPI"/> representing all <c>JollyQuotes</c> APIs supported by the specified <paramref name="assembly"/>.
		/// </summary>
		/// <param name="assembly"><see cref="Assembly"/> to get all <c>JollyQuotes</c> APIs supported by.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		public static JollyQuotesAPI GetSupportedAPIs(this Assembly assembly)
		{
			if (assembly is null)
			{
				throw Error.Null(nameof(assembly));
			}

			return GetSupportedAPIs(assembly.GetReferencedAssemblies());
		}

		/// <summary>
		/// Returns a value of <see cref="JollyQuotesAPI"/> representing all <c>JollyQuotes</c> APIs supported by the specified collection of assembly <paramref name="references"/>.
		/// </summary>
		/// <param name="references">A collection of <see cref="AssemblyName"/>s representing target assembly references.</param>
		/// <exception cref="ArgumentNullException"><paramref name="references"/> is <see langword="null"/>.</exception>
		public static JollyQuotesAPI GetSupportedAPIs(IEnumerable<AssemblyName> references)
		{
			if (references is null)
			{
				throw Error.Null(nameof(references));
			}

			JollyQuotesAPI all = JollyQuotesAPI.None;

			int count = 0;

			foreach (AssemblyName assembly in references)
			{
				if (TryParse(assembly.Name, out JollyQuotesAPI api) && !all.HasFlag(api))
				{
					all |= api;
					count++;

					if (count == NUM_QUOTE_APIS)
					{
						break;
					}
				}
			}

			return all;
		}

		/// <summary>
		/// Converts a specified <see cref="string"/> into a value of <see cref="JollyQuotesAPI"/>.
		/// </summary>
		/// <param name="source"><see cref="string"/> to convert.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="source"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> could not be converted to a value of type <see cref="JollyQuotesAPI"/>.
		/// </exception>
		public static JollyQuotesAPI Parse(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			if (!TryParse_Internal(source, out JollyQuotesAPI api))
			{
				throw Error.Arg($"Value '{source}' could not be converted to a value of type {nameof(JollyQuotesAPI)}", nameof(source));
			}

			return api;
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> <paramref name="apis"/> are supported by the specified <paramref name="assembly"/>.
		/// </summary>
		/// <param name="assembly"><see cref="Assembly"/> to check if supports the specified <paramref name="apis"/>.</param>
		/// <param name="apis"><see cref="JollyQuotesAPI"/>s to check if are supported by the provided <paramref name="assembly"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		public static bool SupportsAPIs(this Assembly assembly, JollyQuotesAPI apis)
		{
			if (assembly is null)
			{
				throw Error.Null(nameof(assembly));
			}

			return SupportsAPIs(assembly.GetReferencedAssemblies(), apis);
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> <paramref name="apis"/> are supported by the provided collection of <paramref name="references"/>.
		/// </summary>
		/// <param name="references">A collection of <see cref="AssemblyName"/>s representing target assembly references.</param>
		/// <param name="apis"><see cref="JollyQuotesAPI"/>s to check if are supported by the provided <paramref name="references"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="references"/> is <see langword="null"/>.</exception>
		public static bool SupportsAPIs(IEnumerable<AssemblyName> references, JollyQuotesAPI apis)
		{
			if (references is null)
			{
				throw Error.Null(nameof(references));
			}

			JollyQuotesAPI current = apis;

			foreach (AssemblyName assembly in references)
			{
				if (assembly is null)
				{
					continue;
				}

				if (TryParse(assembly.Name, out JollyQuotesAPI result) && current.HasFlag(result))
				{
					current &= ~result;

					if (current == default)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Attempts to convert a <see cref="string"/> into a value of <see cref="JollyQuotesAPI"/>.
		/// </summary>
		/// <param name="source"><see cref="string"/> to convert.</param>
		/// <param name="api">Result of the conversion.</param>
		/// <returns><see langword="true"/> if the conversion was a success, <see langword="false"/> otherwise.</returns>
		public static bool TryParse(string? source, out JollyQuotesAPI api)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				api = default;
				return false;
			}

			return TryParse_Internal(source, out api);
		}

		private static bool TryParse_Internal(string source, out JollyQuotesAPI api)
		{
			const StringComparison com = StringComparison.OrdinalIgnoreCase;

			const string kanye = "kanye";
			const string tronald = "tronald";
			const string quotable = "quotable";

			ReadOnlySpan<char> span = source.StartsWith("JollyQuotes")
				? source[11..]
				: source;

			ReadOnlySpan<char> trimmed = span.Trim();

			if (trimmed.Length == 0)
			{
				api = default;
				return false;
			}

			if (trimmed.StartsWith(kanye, com))
			{
				if (IsApi(trimmed, kanye.Length, '.', "rest", true))
				{
					api = JollyQuotesAPI.KanyeRest;
					return true;
				}
			}
			else if (trimmed.StartsWith(tronald))
			{
				if (IsApi(trimmed, tronald.Length, ' ', "dump", true))
				{
					api = JollyQuotesAPI.TronaldDump;
					return true;
				}
			}
			else if (trimmed.StartsWith(quotable))
			{
				if (IsApi(trimmed, quotable.Length, '.', "io", false))
				{
					api = JollyQuotesAPI.Quotable;
					return true;
				}
			}

			api = JollyQuotesAPI.None;
			return false;

			static bool IsApi(ReadOnlySpan<char> original, int length, char splitChar, string postfix, bool requirePostfix)
			{
				ReadOnlySpan<char> span = original[length..].TrimStart();

				if (span.Length == 0)
				{
					return !requirePostfix;
				}

				if (span[0] == splitChar)
				{
					span = span[1..].TrimStart();
				}

				return span.Equals(postfix, com);
			}
		}
	}
}
