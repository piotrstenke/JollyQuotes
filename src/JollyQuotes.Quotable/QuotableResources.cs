using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using JollyQuotes.Quotable.Models;

namespace JollyQuotes.Quotable
{
	/// <summary>
	/// Contains links to important <c>quotable</c> resources.
	/// </summary>
	public static class QuotableResources
	{
		/// <summary>
		/// Name of the API.
		/// </summary>
		public const string ApiName = "quotable";

		/// <summary>
		/// Entry point for API calls.
		/// </summary>
		public const string ApiPage = "https://api.quotable.io";

		/// <summary>
		/// Default number of results on a response page.
		/// </summary>
		public const int DefaultResultsPerPage = 20;

		/// <summary>
		/// Link to page with documentation of the <c>quotable</c> API.
		/// </summary>
		public const string DocsPage = "https://github.com/lukePeavey/quotable/blob/master/README.md";

		/// <summary>
		/// Link to the GitHub page of the <c>quotable</c> source code.
		/// </summary>
		public const string GitHubPage = "https://github.com/lukePeavey/quotable";

		/// <summary>
		/// Maximum number of results on a response page.
		/// </summary>
		public const int MaxResultsPerPage = 150;

		/// <summary>
		/// Minimum number of results on a response page.
		/// </summary>
		public const int MinResultsPerPage = 1;

		internal const string AUTHOR_ID_OBSOLETE = "Author id was deprecated by the maintainers of quotable.io. ";

		internal const string BASE_ADDRESS = ApiPage;

		internal const string MAX_RESULTS_PER_PAGE_NAME = nameof(QuotableResources) + "." + nameof(MaxResultsPerPage);

		/// <inheritdoc cref="IQuotableModelConverter.GetName(SortOrder)"/>
		public static string GetName(SortOrder value)
		{
			return value switch
			{
				SortOrder.Ascending => "asc",
				SortOrder.Descending => "desc",
				_ => throw Error.InvalidEnumValue(value)
			};
		}

		/// <inheritdoc cref="IQuotableModelConverter.GetName(SortBy)"/>
		public static string GetName(SortBy value)
		{
			return value switch
			{
				SortBy.Name => "name",
				SortBy.DateAddded => "dateAdded",
				SortBy.DateModified => "dateModified",
				SortBy.QuoteCount => "quoteCount",
				_ => throw Error.InvalidEnumValue(value)
			};
		}

		/// <inheritdoc cref="IQuotableModelConverter.GetName(QuoteSortBy)"/>
		public static string GetName(QuoteSortBy value)
		{
			return value switch
			{
				QuoteSortBy.DateAddded => "dateAdded",
				QuoteSortBy.DateModified => "dateModified",
				QuoteSortBy.Author => "author",
				QuoteSortBy.Content => "content",
				_ => throw Error.InvalidEnumValue(value)
			};
		}

		internal static HttpResolver CreateDefaultResolver()
		{
			HttpClient client = Internals.CreateDefaultClient();
			client.BaseAddress = new Uri(BASE_ADDRESS);
			return new HttpResolver(client);
		}

		[DebuggerStepThrough]
		internal static SearchOperator EnsureValidOperator(this SearchOperator op)
		{
			if (op.IsValidOperator())
			{
				return op;
			}

			throw Exc_InvalidOperator(op);
		}

		[DebuggerStepThrough]
		internal static ArgumentException Exc_InvalidOperator(SearchOperator op, [CallerArgumentExpression("op")] string paramName = "")
		{
			return Error.Arg($"Invalid {nameof(SearchOperator)} value: '{op}'", paramName);
		}

		[DebuggerStepThrough]
		internal static bool IsValidOperator(this SearchOperator op)
		{
			return op == SearchOperator.Or || op == SearchOperator.And;
		}

		[DebuggerStepThrough]
		internal static char ToChar(this SearchOperator op)
		{
			return op switch
			{
				SearchOperator.And => ',',
				SearchOperator.Or => '|',
				_ => throw Exc_InvalidOperator(op)
			};
		}
	}
}
