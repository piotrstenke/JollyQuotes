using System;
using System.Collections;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <inheritdoc cref="ISearchResultModel"/>
	/// <typeparam name="T">Type of data searched for.</typeparam>
	[Serializable]
	[JsonObject]
	public sealed record SearchResultModel<T> : ISearchResultModel where T : IEnumerable
	{
		private readonly int _total;
		private readonly int _count;
		private readonly T _embedded;

		/// <inheritdoc/>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>. -or- Value must be less than or equal to <see cref="Total"/>.</exception>
		[JsonProperty("count", Order = 0, Required = Required.Always)]
		public int Count
		{
			get => _count;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if (_count > _total)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), nameof(Total));
				}

				_count = value;
			}
		}

		/// <inheritdoc cref="ISearchResultModel.Embedded"/>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("_embedded", Order = 2, Required = Required.Always)]
		public T Embedded
		{
			get => _embedded;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_embedded = value;
			}
		}

		/// <inheritdoc/>
		[JsonProperty("_links", Order = 3)]
		public PageHierarchyModel? Links { get; init; }

		/// <inheritdoc/>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than or equal to <c>0</c>.</exception>
		[JsonProperty("total", Order = 1, Required = Required.Always)]
		public int Total
		{
			get => _total;
			init
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				_total = value;
			}
		}

		IEnumerable ISearchResultModel.Embedded => Embedded;

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with <paramref name="embedded"/> data,
		/// number of its elements and their <paramref name="total"/> number.
		/// </summary>
		/// <param name="count">Number of values contained on the current page.</param>
		/// <param name="total">Total number of values contained in the search result.</param>
		/// <param name="embedded">Values present in the current page of the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="embedded"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="total"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be less than or equal to <paramref name="total"/>.
		/// </exception>
		public SearchResultModel(int count, int total, T embedded) : this(count, total, embedded, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SearchResultModel{T}"/> class with <paramref name="embedded"/> data,
		/// number of its elements and their <paramref name="total"/> number and <paramref name="links"/> to all pages in the search result.
		/// </summary>
		/// <param name="count">Number of values contained on the current page.</param>
		/// <param name="total">Total number of values contained in the search result.</param>
		/// <param name="embedded">Values present in the current page of the search result.</param>
		/// <param name="links">Links to all the pages in the search result.</param>
		/// <exception cref="ArgumentNullException"><paramref name="embedded"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="total"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be greater than or equal to <c>0</c>. -or-
		/// <paramref name="count"/> must be less than or equal <paramref name="total"/>.
		/// </exception>
		[JsonConstructor]
		public SearchResultModel(int count, int total, T embedded, PageHierarchyModel? links)
		{
			if (embedded is null)
			{
				throw Error.Null(nameof(embedded));
			}

			if (total < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(total), 0);
			}

			if (count < 0)
			{
				throw Error.MustBeGreaterThanOrEqualTo(nameof(count), 0);
			}

			if (count > total)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(count), nameof(total));
			}

			_count = count;
			_total = total;
			Links = links;
			_embedded = embedded;
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

			if (other.Count != Count || other.Links != Links)
			{
				return false;
			}

			if (other.Embedded is IEquatable<T> eq)
			{
				return eq.Equals(Embedded);
			}

			IEnumerator e1 = other.Embedded.GetEnumerator();
			IEnumerator e2 = Embedded.GetEnumerator();

			while (true)
			{
				bool move1 = e1.MoveNext();
				bool move2 = e2.MoveNext();

				if (move1 && move2)
				{
					if (e1.Current != e2.Current)
					{
						return false;
					}
				}
				else
				{
					return !move1 && !move2;
				}
			}
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode code = new();

			code.Add(Count);
			code.Add(Links);

			if (Embedded is IEquatable<T>)
			{
				code.Add(Embedded);
			}
			else
			{
				foreach (T obj in Embedded)
				{
					code.Add(obj);
				}
			}

			return code.ToHashCode();
		}
	}
}
