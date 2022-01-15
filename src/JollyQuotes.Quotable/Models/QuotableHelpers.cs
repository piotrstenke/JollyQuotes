using System;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Contains various helper methods for enums.
	/// </summary>
	public static class QuotableHelpers
	{
		/// <summary>
		/// Returns the default <see cref="SortOrder"/> for the specified <paramref name="sortBy"/> field.
		/// </summary>
		/// <param name="sortBy">Field to get the default sort order for.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		public static SortOrder GetDefaultSortOrder(this SortBy sortBy)
		{
			return sortBy switch
			{
				SortBy.Name => SortOrder.Ascending,
				SortBy.DateAddded or SortBy.DateModified or SortBy.QuoteCount => SortOrder.Descending,
				_ => throw Error.InvalidEnumValue(sortBy)
			};
		}

		/// <summary>
		/// Returns the default <see cref="SortOrder"/> for the specified <paramref name="sortBy"/> field.
		/// </summary>
		/// <param name="sortBy">Field to get the default sort order for.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		public static SortOrder GetDefaultSortOrder(this QuoteSortBy sortBy)
		{
			return sortBy switch
			{
				QuoteSortBy.Author or QuoteSortBy.Content => SortOrder.Ascending,
				QuoteSortBy.DateAddded or QuoteSortBy.DateModified => SortOrder.Descending,
				_ => throw Error.InvalidEnumValue(sortBy)
			};
		}

		/// <summary>
		/// Returns a <see cref="string"/> representation of the specified enum <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		public static string GetName(this SortOrder value)
		{
			return value switch
			{
				SortOrder.Ascending => "asc",
				SortOrder.Descending => "desc",
				_ => throw Error.InvalidEnumValue(value)
			};
		}

		/// <summary>
		/// Returns a <see cref="string"/> representation of the specified enum <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		public static string GetName(this SortBy value)
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

		/// <summary>
		/// Returns a <see cref="string"/> representation of the specified enum <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to get the <see cref="string"/> representation of.</param>
		/// <exception cref="ArgumentException">Invalid enum value.</exception>
		public static string GetName(this QuoteSortBy value)
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

		/// <summary>
		/// Determines whether the specified enum value has any flag set.
		/// </summary>
		/// <param name="value"><see cref="QuoteSearchFields"/> value to check if has any flag.</param>
		public static bool HasAnyFlag(this QuoteSearchFields value)
		{
			return QuoteSearchFields.All.HasFlag(value);
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="SortBy"/> value to check if is valid.</param>
		public static bool IsValid(this SortBy value)
		{
			return value >= default(SortBy) && value <= SortBy.QuoteCount;
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="QuoteSortBy"/> value to check if is valid.</param>
		public static bool IsValid(this QuoteSortBy value)
		{
			return value >= default(QuoteSortBy) && value <= QuoteSortBy.Content;
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="MatchThreshold"/> value to check if is valid.</param>
		public static bool IsValid(this MatchThreshold value)
		{
			return value >= MatchThreshold.Min && value <= MatchThreshold.Max;
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="SortOrder"/> value to check if is valid.</param>
		public static bool IsValid(this SortOrder value)
		{
			return value is SortOrder.Ascending or SortOrder.Descending;
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="SearchOperator"/> value to check if is valid.</param>
		public static bool IsValid(this SearchOperator value)
		{
			return value is SearchOperator.Or or SearchOperator.And;
		}

		/// <summary>
		/// Determines whether the specified enum <paramref name="value"/> is valid.
		/// </summary>
		/// <param name="value"><see cref="FuzzyMatchingTreshold"/> value to check if is valid.</param>
		public static bool IsValid(this FuzzyMatchingTreshold value)
		{
			return value >= FuzzyMatchingTreshold.Min && value <= FuzzyMatchingTreshold.Max;
		}

		/// <summary>
		/// Converts the specified <see cref="SearchOperator"/> <paramref name="value"/> to its <see cref="char"/> representation.
		/// </summary>
		/// <param name="value"><see cref="SearchOperator"/> to convert to a c<see cref="char"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="value"/> is not a valid enum value.</exception>
		public static char ToChar(this SearchOperator value)
		{
			return value switch
			{
				SearchOperator.And => ',',
				SearchOperator.Or => '|',
				_ => throw Error.InvalidEnumValue(value)
			};
		}
	}
}
