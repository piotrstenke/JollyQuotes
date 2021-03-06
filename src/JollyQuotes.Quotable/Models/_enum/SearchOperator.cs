using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Defines logical operators that can be used in a search query.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SearchOperator
	{
		/// <summary>
		/// No operator.
		/// </summary>
		[EnumMember(Value = "none")]
		None = 0,

		/// <summary>
		/// Represents the AND operator.
		/// </summary>
		[EnumMember(Value = "and")]
		And = 1,

		/// <summary>
		/// Represents the OR operator.
		/// </summary>
		[EnumMember(Value = "or")]
		Or = 2
	}
}
