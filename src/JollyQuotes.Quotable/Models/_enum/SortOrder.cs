using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Specifies sort order of a collection.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SortOrder
	{
		/// <summary>
		/// The collection is sorted ascending.
		/// </summary>
		[EnumMember(Value = "asc")]
		Ascending,

		/// <summary>
		/// The collection is sorted descending.
		/// </summary>
		[EnumMember(Value = "desc")]
		Descending
	}
}
