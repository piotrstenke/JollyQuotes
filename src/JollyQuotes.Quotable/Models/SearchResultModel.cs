using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Represents a result of a search request.
	/// </summary>
	/// <typeparam name="T">Type of data searched for.</typeparam>
	[JsonObject]
	public sealed record SearchResultModel<T> where T : IEquatable<T>
	{
		private readonly int _page;
		private readonly int _totalCount;
		private readonly int _totalPages;
		private readonly T[] _results;

		/// <summary>
		/// Number of results included in the current <see cref="Page"/>.
		/// </summary>
		[JsonProperty("count", Order = 0, Required = Required.Always)]
		public int Count { get; private set; }

		/// <summary>
		/// Total number of results matching the request.
		/// </summary>
		/// <exception cref="ArgumentException">Value cannot equal to <c>0</c> when <see cref="Count"/> is greater than <c>0</c> and <see cref="Page"/> is equal to <c>1</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be greater than or equal to <see cref="Count"/>.</exception>
		[JsonProperty("totalCount", Order = 1, Required = Required.Always)]
		public int TotalCount
		{
			get => _totalCount;
			init
			{
				if (value == _totalCount)
				{
					return;
				}

				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if (value < Count)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(Count));
				}

				if (value == 0 && Count > 0 && Page == 1)
				{
					throw Exc_CannotBeSetTo0WhenPageIs1(nameof(value));
				}

				_totalCount = value;
			}
		}

		/// <summary>
		/// Current page number.
		/// </summary>
		/// <exception cref="ArgumentException">Value cannot be equal to <c>1</c> when either <see cref="TotalCount"/> or <see cref="TotalPages"/> is greater than <c>0</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>.</exception>
		[JsonProperty("page", Order = 2, Required = Required.Always)]
		public int Page
		{
			get => _page;
			init
			{
				if (value == _page)
				{
					return;
				}

				if (value <= 0)
				{
					throw Error.MustBeGreaterThan(nameof(value), 0);
				}

				if (value == 1 && Count == 0)
				{
					if (TotalPages > 0 || TotalCount > 0)
					{
						throw Exc_InvalidValueAtPage1(nameof(value), nameof(TotalCount), nameof(TotalPages), nameof(Count));
					}
				}

				_page = value;
			}
		}

		/// <summary>
		/// Total number of pages matching the request.
		/// </summary>
		/// <exception cref="ArgumentException">Value cannot equal to <c>0</c> when <see cref="Count"/> is greater than <c>0</c> and <see cref="Page"/> is equal to <c>1</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be greater than or equal to <see cref="Count"/>.</exception>
		[JsonProperty("totalPages", Order = 3, Required = Required.Always)]
		public int TotalPages
		{
			get => _totalPages;
			init
			{
				if (value == _totalPages)
				{
					return;
				}

				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if (value < Page)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(Page));
				}

				if (value == 0 && Count > 0 && Page == 1)
				{
					throw Exc_CannotBeSetTo0WhenPageIs1(nameof(value));
				}

				_totalPages = value;
			}
		}

		/// <summary>
		/// 1-based index of the last result in the response.
		/// </summary>
		[JsonProperty("lastItemIndex", Order = 4)]
		public int? LastItemIndex { get; private set; }

		/// <summary>
		/// Results of the request.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Length of value must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>. -or-
		/// Length of value must be less than or equal to <see cref="TotalCount"/>.</exception>
		[JsonProperty("results", Order = 5, Required = Required.Always)]
		public T[] Results
		{
			get => _results;
			init
			{
				if (value == _results)
				{
					return;
				}

				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				if (value.Length != _results.Length)
				{
					if (value.Length > QuotableResources.ResultsPerPageMax)
					{
						throw Exc_LengthOfResultsMustBeLessThanOrEqualToMaxResultsPerPage(nameof(value));
					}

					if (value.Length > TotalCount)
					{
						throw Exc_LengthOfResultsMustBeLessThanOrEqualToTotalCount(nameof(value), nameof(TotalCount));
					}

					Count = value.Length;
					LastItemIndex = Count;
				}

				_results = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class.
		/// </summary>
		public SearchResultModel() : this((T[]?)default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with a collection of <paramref name="results"/> specified.
		/// </summary>
		/// <param name="results">Results of the request.</param>
		/// <exception cref="ArgumentException">Length of <paramref name="results"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>.</exception>
		public SearchResultModel(T[]? results)
		{
			if (results is null || results.Length == 0)
			{
				_results = results ?? Array.Empty<T>();
				_page = 1;
				TotalPages = 1;
				Count = 0;
				TotalCount = 0;
			}
			else if (results.Length > QuotableResources.ResultsPerPageMax)
			{
				throw Exc_LengthOfResultsMustBeLessThanOrEqualToMaxResultsPerPage(nameof(results));
			}
			else
			{
				_results = results;
				_page = 1;
				TotalPages = 1;
				TotalCount = results.Length;
				Count = results.Length;
				LastItemIndex = results.Length;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with a collection of <paramref name="results"/>, current <paramref name="page"/>, <paramref name="totalPages"/> and <paramref name="totalCount"/>.
		/// </summary>
		/// <param name="results">Results of the request.</param>
		/// <param name="page">Current page number.</param>
		/// <param name="totalPages">Total number of pages matching the request.</param>
		/// <param name="totalCount">Total number of results matching the request.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="page"/> cannot be equal to <c>1</c> when length of <paramref name="results"/> is null or empty and either <paramref name="totalCount"/> or <paramref name="totalPages"/> is greater than <c>0</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Length of <paramref name="results"/> must be less than or equal to <see cref="QuotableResources.ResultsPerPageMax"/>. -or-
		/// Length of <paramref name="results"/> must be less than or equal to <paramref name="totalCount"/>. -or-
		/// <paramref name="page"/> must be greater than <c>0</c>. -or-
		/// <paramref name="totalCount"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="totalPages"/> must be greater than or equal to <c>0</c>.
		/// </exception>
		public SearchResultModel(T[]? results, int page, int totalPages, int totalCount)
			: this(results, page, totalPages, totalCount, default, default, false)
		{
		}

		[JsonConstructor]
#pragma warning disable IDE0051 // Remove unused private members
		private SearchResultModel(
#pragma warning restore IDE0051 // Remove unused private members
			T[]? results,
			int page,
			int totalPages,
			int totalCount,
			int count,
			int? lastItemIndex
		) : this(results, page, totalPages, totalCount, count, lastItemIndex, true)
		{
		}

		private SearchResultModel(
			T[]? results,
			int page,
			int totalPages,
			int totalCount,
			int count,
			int? lastItemIndex,
			bool serialized
		)
		{
			if (page <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(page), 0);
			}

			if (totalPages < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(totalPages), 0);
			}

			if (totalCount < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(totalCount), 0);
			}

			if (results is null || results.Length == 0)
			{
				ValidateEmptyResults(page, totalPages, totalCount, count, lastItemIndex, serialized);

				_results = results ?? Array.Empty<T>();
				_page = page;
				TotalPages = totalPages;
				TotalCount = totalCount;
				Count = 0;
			}
			else
			{
				ValidateResults(results, totalCount, count, lastItemIndex, serialized);

				if (serialized)
				{
					Count = count;
					LastItemIndex = lastItemIndex;
				}
				else
				{
					Count = results.Length;
					LastItemIndex = Count;
				}

				TotalCount = totalCount;
				TotalPages = totalPages;
				_page = page;
				_results = results;
			}
		}

		/// <inheritdoc/>
		public bool Equals(SearchResultModel<T>? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return
				other._page == _page &&
				other._totalPages == _totalPages &&
				other._totalCount == _totalCount &&
				other._results.Length == _results.Length &&
				other._results.SequenceEqual(_results);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			hash.Add(_page);
			hash.Add(_totalCount);
			hash.Add(_totalPages);
			hash.AddSequence(_results);

			return hash.ToHashCode();
		}

		[DebuggerStepThrough]
		private static ArgumentOutOfRangeException Exc_LengthOfResultsMustBeLessThanOrEqualToMaxResultsPerPage(string paramName)
		{
			return Error.ArgOutOfRange($"Length of '{paramName}' must be less than or equal to {QuotableResources.MAX_RESULTS_PER_PAGE_NAME}", paramName);
		}

		[DebuggerStepThrough]
		private static ArgumentOutOfRangeException Exc_LengthOfResultsMustBeLessThanOrEqualToTotalCount(string paramName1, string paramName2)
		{
			return Error.ArgOutOfRange(paramName1, $"The length of '{paramName1}' must be less than or equal to '{paramName2}'");
		}

		[DebuggerStepThrough]
		private static ArgumentException Exc_InvalidValueAtPage1(string pageName, string totalCountName, string totalPagesName, string countName)
		{
			return Error.Arg($"'{pageName}' cannot be equal to 1 when '{countName}' is equal to 0 and  either '{totalCountName}' or '{totalPagesName} is greater than 0", pageName);
		}

		[DebuggerStepThrough]
		private static ArgumentException Exc_CannotBeSetTo0WhenPageIs1(string paramName)
		{
			return Error.Arg($"Value cannot be set to 0 when '{nameof(Page)}' is equal to 1 and '{nameof(Count)}' is greater than 0", paramName);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateResults(
			T[] results,
			int totalCount,
			int count,
			int? lastItemIndex,
			bool serialized
		)
		{
			if (results.Length > totalCount)
			{
				throw Exc_LengthOfResultsMustBeLessThanOrEqualToTotalCount(nameof(results), nameof(totalCount));
			}

			if (serialized)
			{
				if (!lastItemIndex.HasValue || lastItemIndex <= 0)
				{
					throw Error.Arg($"'{nameof(lastItemIndex)}' must be larger than 0 if '{nameof(results)}' is not empty", nameof(lastItemIndex));
				}

				if (count != results.Length)
				{
					throw Error.Arg($"'{nameof(count)}' must be equal to '{nameof(results)}.Length' if '{nameof(results)}' is not empty", nameof(count));
				}
			}

			if (results.Length > QuotableResources.ResultsPerPageMax)
			{
				if (serialized)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(count), QuotableResources.MAX_RESULTS_PER_PAGE_NAME);
				}
				else
				{
					throw Exc_LengthOfResultsMustBeLessThanOrEqualToMaxResultsPerPage(nameof(results));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ValidateEmptyResults(
			int page,
			int totalPages,
			int totalCount,
			int count,
			int? lastItemIndex,
			bool serialized
		)
		{
			if (page == 1)
			{
				if (totalCount > 0 || totalPages > 0)
				{
					throw Exc_InvalidValueAtPage1(nameof(page), nameof(totalCount), nameof(totalPages), nameof(count));
				}
			}

			if (serialized)
			{
				if (lastItemIndex.HasValue)
				{
					throw Error.Arg($"'{nameof(lastItemIndex)}' must be null if 'results' is null or empty", nameof(lastItemIndex));
				}

				if (count != 0)
				{
					throw Error.Arg($"'{nameof(count)}' must be equal to 0 if 'results' is null or empty", nameof(count));
				}
			}
		}
	}
}
