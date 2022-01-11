using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Represents a tag expression found in a search query.
	/// </summary>
	[DebuggerDisplay("base.ToString()")]
	[Serializable]
	[JsonObject]
	public sealed record TagExpression
	{
		/// <summary>
		/// Child node of the left.
		/// </summary>
		[JsonProperty("left", Order = 2)]
		public TagExpression? Left { get; }

		/// <summary>
		/// Child node on the right.
		/// </summary>
		[JsonProperty("right", Order = 1)]
		public TagExpression? Right { get; }

		/// <summary>
		/// Determines whether the current expression is an end node (has no further branches).
		/// </summary>
		[MemberNotNullWhen(true, nameof(Value))]
		[MemberNotNullWhen(false, nameof(Left), nameof(Right))]
		[JsonIgnore]
		public bool IsEndNode => Value is not null;

		/// <summary>
		/// Text value of the expression or <see langword="null"/> if the expression is not an end node.
		/// </summary>
		[JsonProperty("value", Order = 2)]
		public string? Value { get; }

		/// <summary>
		/// Operator that is applied to the expression.
		/// </summary>
		/// <exception cref="ArgumentException">Value is not a valid <see cref="SearchOperator"/> value.</exception>
		[JsonProperty("operator", Order = 3)]
		public SearchOperator Operator { get; }

		[JsonConstructor]
		private TagExpression(TagExpression? left, TagExpression? right, SearchOperator @operator, string? value)
		{
			if (value is null)
			{
				if (left is null || right is null)
				{
					throw Error.Arg("Either value or child nodes must be specified", nameof(value));
				}

				if (!@operator.IsValidOperator())
				{
					throw QuotableResources.Exc_InvalidOperator(@operator);
				}

				Left = left;
				Right = right;
				Operator = @operator;

				return;
			}

			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.Arg("Value cannot be empty when no child nodes are specified", nameof(value));
			}

			if (left is not null || right is not null)
			{
				throw Error.Arg("Value and child nodes cannot be both specified", nameof(value));
			}

			if (@operator != SearchOperator.None)
			{
				throw Error.Arg($"{nameof(SearchOperator)} cannot be specified for an {nameof(TagExpression)} without nodes", nameof(@operator));
			}

			Value = value;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagExpression"/> class with <paramref name="left"/> and <paramref name="right"/> nodes an a target <see cref="SearchOperator"/>.
		/// </summary>
		/// <param name="left">Child node of the left.</param>
		/// <param name="right">Child node on the right.</param>
		/// <param name="op">Operator that is applied to the expression.</param>
		/// <exception cref="ArgumentNullException"><paramref name="left"/> is <see langword="null"/>. -or- <paramref name="right"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="op"/> is not a valid <see cref="SearchOperator"/> value.</exception>
		public TagExpression(TagExpression left, TagExpression right, SearchOperator op)
		{
			if (left is null)
			{
				throw Error.Null(nameof(left));
			}

			if (right is null)
			{
				throw Error.Null(nameof(right));
			}

			Operator = op.EnsureValidOperator();
			Left = left;
			Right = right;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagExpression"/> class with a text <paramref name="value"/> specified.
		/// </summary>
		/// <param name="value">Text value of the expression.</param>
		/// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/> or empty.</exception>
		public TagExpression(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw Error.NullOrEmpty(nameof(value));
			}

			Value = value;
		}

		/// <inheritdoc/>
		public bool Equals(TagExpression? other)
		{
			if (other is null)
			{
				return false;
			}

			if (IsEndNode)
			{
				if (other.IsEndNode)
				{
					return other.Value == Value;
				}

				return false;
			}

			return
				other.Left == Left &&
				other.Right == Right &&
				other.Operator == Operator;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			if (IsEndNode)
			{
				return Value.GetHashCode();
			}

			return HashCode.Combine(Left, Right, Operator);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			if (IsEndNode)
			{
				return Value;
			}

			return Left.ToString() + Operator.ToChar() + Right.ToString();
		}
	}
}
