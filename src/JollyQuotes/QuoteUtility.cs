using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using JollyQuotes.KanyeRest;
using JollyQuotes.TronaldDump;
using JollyQuotes.Quotable;

namespace JollyQuotes
{
	/// <summary>
	/// Contains various utility methods helpful when dealing with quotes.
	/// </summary>
	public static class QuoteUtility
	{
		internal const int NUM_QUOTE_APIS = 3;

		/// <inheritdoc cref="IBuiltInQuoteApiHandler.CreateDescription(JollyQuotesApi)"/>
		public static QuoteApiDescription CreateDescription(JollyQuotesApi api)
		{
			if (!TryCreateDescription(api, out QuoteApiDescription? description))
			{
				throw Exc_InvalidEnum(nameof(api));
			}

			return description;
		}

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API. -or-
		/// The <c>Tronald Dump</c> API requires the <paramref name="resolver"/> to implement the <see cref="IStreamResolver"/> interface.
		/// </exception>
		public static IQuoteGenerator CreateGenerator(JollyQuotesApi api, IResourceResolver resolver, IPossibility? possibility = default)
		{
			if (!TryCreateGenerator(api, resolver, out IQuoteGenerator? generator, possibility))
			{
				throw Exc_InvalidEnum(nameof(api));
			}

			return generator;
		}

		/// <summary>
		/// Returns full name of web API behind the specified <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api"><c>JollyQuotes</c> API to get the full name of.</param>
		public static string GetActualApiName(JollyQuotesApi api)
		{
			return api switch
			{
				JollyQuotesApi.KanyeRest => ApiNames.KanyeRest,
				JollyQuotesApi.TronaldDump => ApiNames.TronaldDump,
				JollyQuotesApi.Quotable => ApiNames.Quotable,
				_ => throw Exc_InvalidEnum(nameof(api))
			};
		}

		/// <summary>
		/// Creates a new <see cref="IOptionalPossibility"/> with <see cref="NamedOption"/>s created from the result of calling <see cref="IQuoteApiHandler.GetApis()"/>.
		/// </summary>
		/// <param name="apiHandler"><see cref="IQuoteApiHandler"/> to create the <see cref="IOptionalPossibility"/> for.</param>
		/// <exception cref="ArgumentNullException"><paramref name="apiHandler"/> is <see langword="null"/>.</exception>
		public static IOptionalPossibility GetDefaultPossibility(this IQuoteApiHandler apiHandler)
		{
			if (apiHandler is null)
			{
				throw Error.Null(nameof(apiHandler));
			}

			IEnumerable<string> apis = apiHandler.GetApis();

			return GetDefaultPossibility(apis.ToArray());
		}

		/// <summary>
		/// Creates a new <see cref="IOptionalPossibility"/> for the specified <paramref name="apis"/> with default possibility for each.
		/// </summary>
		/// <param name="apis">Names of APIs that serve as <see cref="NamedOption"/>s in the returned <see cref="IOptionalPossibility"/>.</param>
		public static IOptionalPossibility GetDefaultPossibility(string[]? apis)
		{
			return OptionalPossibility.EqualOptions(apis);
		}

		/// <summary>
		/// Returns a value of <see cref="JollyQuotesApi"/> representing all <c>JollyQuotes</c> APIs supported by the specified <paramref name="assembly"/>.
		/// </summary>
		/// <param name="assembly"><see cref="Assembly"/> to get all <c>JollyQuotes</c> APIs supported by.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		public static JollyQuotesApi GetSupportedApis(Assembly assembly)
		{
			if (assembly is null)
			{
				throw Error.Null(nameof(assembly));
			}

			return GetSupportedApis(assembly.GetReferencedAssemblies());
		}

		/// <summary>
		/// Returns a value of <see cref="JollyQuotesApi"/> representing all <c>JollyQuotes</c> APIs supported by the specified collection of assembly <paramref name="references"/>.
		/// </summary>
		/// <param name="references">A collection of <see cref="AssemblyName"/>s representing target assembly references.</param>
		/// <exception cref="ArgumentNullException"><paramref name="references"/> is <see langword="null"/>.</exception>
		public static JollyQuotesApi GetSupportedApis(IEnumerable<AssemblyName> references)
		{
			if (references is null)
			{
				throw Error.Null(nameof(references));
			}

			JollyQuotesApi all = JollyQuotesApi.None;

			int count = 0;

			foreach (AssemblyName assembly in references)
			{
				if (TryParseApi(assembly.Name, out JollyQuotesApi api) && !all.HasFlag(api))
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
		/// Determines whether the specified value of <see cref="JollyQuotesApi"/> represents a valid single <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Value of <see cref="JollyQuotesApi"/> to check.</param>
		public static bool IsSingleApi(this JollyQuotesApi api)
		{
			return api.EnumToIndex() > -1;
		}

		/// <summary>
		/// Converts a specified <see cref="string"/> into a value of <see cref="JollyQuotesApi"/>.
		/// </summary>
		/// <param name="source"><see cref="string"/> to convert.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="source"/> is <see langword="null"/> or empty. -or-
		/// <paramref name="source"/> could not be converted to a value of type <see cref="JollyQuotesApi"/>.
		/// </exception>
		public static JollyQuotesApi ParseApi(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			if (!TryParseApi_Internal(source, out JollyQuotesApi api))
			{
				throw Error.Arg($"Value '{source}' could not be converted to a value of type {nameof(JollyQuotesApi)}", nameof(source));
			}

			return api;
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> <paramref name="apis"/> are supported by the specified <paramref name="assembly"/>.
		/// </summary>
		/// <param name="assembly"><see cref="Assembly"/> to check if supports the specified <paramref name="apis"/>.</param>
		/// <param name="apis"><see cref="JollyQuotesApi"/>s to check if are supported by the provided <paramref name="assembly"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		public static bool SupportsApis(Assembly assembly, JollyQuotesApi apis)
		{
			if (assembly is null)
			{
				throw Error.Null(nameof(assembly));
			}

			return SupportsApis(assembly.GetReferencedAssemblies(), apis);
		}

		/// <summary>
		/// Determines whether the specified <c>JollyQuotes</c> <paramref name="apis"/> are supported by the provided collection of <paramref name="references"/>.
		/// </summary>
		/// <param name="references">A collection of <see cref="AssemblyName"/>s representing target assembly references.</param>
		/// <param name="apis"><see cref="JollyQuotesApi"/>s to check if are supported by the provided <paramref name="references"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="references"/> is <see langword="null"/>.</exception>
		public static bool SupportsApis(IEnumerable<AssemblyName> references, JollyQuotesApi apis)
		{
			if (references is null)
			{
				throw Error.Null(nameof(references));
			}

			JollyQuotesApi current = apis;

			foreach (AssemblyName assembly in references)
			{
				if (assembly is null)
				{
					continue;
				}

				if (TryParseApi(assembly.Name, out JollyQuotesApi result) && current.HasFlag(result))
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
		/// Attempts to create a new <see cref="QuoteApiDescription"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="QuoteApiDescription"/> for.</param>
		/// <param name="description">Returned <see cref="QuoteApiDescription"/>.</param>
		public static bool TryCreateDescription(JollyQuotesApi api, [NotNullWhen(true)] out QuoteApiDescription? description)
		{
			string apiName = GetActualApiName(api);

			description = api switch
			{
				JollyQuotesApi.KanyeRest => new QuoteApiDescription(
					apiName,
					typeof(KanyeRestQuoteGenerator),
					typeof(KanyeRestService),
					typeof(KanyeRestQuote),
					typeof(KanyeRestResources),
					api
				),

				JollyQuotesApi.TronaldDump => new QuoteApiDescription(
					apiName,
					typeof(TronaldDumpQuoteGenerator),
					typeof(TronaldDumpService),
					typeof(TronaldDumpQuote),
					typeof(TronaldDumpResources),
					api
				),

				JollyQuotesApi.Quotable => new QuoteApiDescription(
					apiName,
					typeof(QuotableQuoteGenerator),
					typeof(QuotableService),
					typeof(QuotableQuote),
					typeof(QuotableResources),
					api
				),

				_ => null
			};

			return description is not null;
		}

		/// <summary>
		/// Creates a new <see cref="IQuoteGenerator"/> for a specified built-in <c>JollyQuotes</c> API.
		/// </summary>
		/// <param name="api">Represents the <c>JollyQuotes</c> API to create the <see cref="IQuoteGenerator"/> for.</param>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access the requested resources.</param>
		/// <param name="generator">Returned <see cref="IQuoteGenerator"/>.</param>
		/// <param name="possibility">
		/// Random number generator used to determine whether to pick quotes from the <see cref="QuoteGenerator{T}.WithCache.Cache"/>
		/// or <see cref="QuoteGenerator{T}.WithCache.Source"/> when <see cref="QuoteInclude.All"/> is passed as argument.
		/// </param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="api"/> must represent a single valid <c>JollyQuotes</c> API.</exception>
		public static bool TryCreateGenerator(
			JollyQuotesApi api,
			IResourceResolver resolver,
			[NotNullWhen(true)] out IQuoteGenerator? generator,
			IPossibility? possibility = default
		)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			if (api == JollyQuotesApi.KanyeRest)
			{
				generator = new KanyeRestQuoteGenerator(resolver, possibility: possibility);
			}
			else if (api == JollyQuotesApi.TronaldDump)
			{
				if (resolver is not IStreamResolver s)
				{
					throw Error.Arg($"The Tronald Dump API requires the '{nameof(resolver)}' to implement the {nameof(IStreamResolver)} interface");
				}

				generator = new TronaldDumpQuoteGenerator(s, possibility: possibility);
			}

			// TODO: Add support for quotable.io IRandomQuoteGenerator
			//else if(api == JollyQuotesApi.Quotable)
			//{
			//}
			else
			{
				generator = default;
				return false;
			}

			return true;
		}

		/// <summary>
		/// Attempts to convert a <see cref="string"/> into a value of <see cref="JollyQuotesApi"/>.
		/// </summary>
		/// <param name="source"><see cref="string"/> to convert.</param>
		/// <param name="api">Result of the conversion.</param>
		/// <returns><see langword="true"/> if the conversion was a success, <see langword="false"/> otherwise.</returns>
		public static bool TryParseApi(string? source, out JollyQuotesApi api)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				api = default;
				return false;
			}

			return TryParseApi_Internal(source, out api);
		}

		internal static int EnumToIndex(this JollyQuotesApi api)
		{
			return api switch
			{
				JollyQuotesApi.KanyeRest => 0,
				JollyQuotesApi.TronaldDump => 1,
				JollyQuotesApi.Quotable => 2,
				_ => -1
			};
		}

		[DebuggerStepThrough]
		internal static ArgumentException Exc_InvalidEnum(string paramName)
		{
			return Error.Arg($"'{paramName}' must represent a single valid JollyQuotes API", paramName);
		}

		[DebuggerStepThrough]
		internal static InvalidOperationException Exc_PossibilityReturnedUnknownApi(string resultName)
		{
			return new InvalidOperationException($"ApiPossibility.Determine() returned an unknown API with name '{resultName}'");
		}

		[DebuggerStepThrough]
		internal static ArgumentException Exc_UnknownApiName(string apiName)
		{
			return new ArgumentException($"Api with the specified name '{apiName}' not found", nameof(apiName));
		}

		[DebuggerStepThrough]
		internal static ArgumentException Exc_UnknownApiNameOrNull(string apiName)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				return Error.NullOrEmpty(nameof(apiName));
			}

			return Exc_UnknownApiName(apiName);
		}

		internal static JollyQuotesApi[] GetFlags(this JollyQuotesApi api)
		{
			List<JollyQuotesApi> list = new(3);

			TryAdd(JollyQuotesApi.KanyeRest);
			TryAdd(JollyQuotesApi.TronaldDump);
			TryAdd(JollyQuotesApi.Quotable);

			return list.ToArray();

			void TryAdd(JollyQuotesApi value)
			{
				if (api.HasFlag(value))
				{
					list.Add(value);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DebuggerStepThrough]
		internal static JollyQuotesApi IndexToEnum(int index)
		{
			return (JollyQuotesApi)(index ^ 2);
		}

		private static bool TryParseApi_Internal(string source, out JollyQuotesApi api)
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
					api = JollyQuotesApi.KanyeRest;
					return true;
				}
			}
			else if (trimmed.StartsWith(tronald))
			{
				if (IsApi(trimmed, tronald.Length, ' ', "dump", true))
				{
					api = JollyQuotesApi.TronaldDump;
					return true;
				}
			}
			else if (trimmed.StartsWith(quotable))
			{
				if (IsApi(trimmed, quotable.Length, '.', "io", false))
				{
					api = JollyQuotesApi.Quotable;
					return true;
				}
			}

			api = JollyQuotesApi.None;
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
