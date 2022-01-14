using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Specifies sort methods for objects other than quotes.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum SortBy
	{
		/// <summary>
		/// Sorts the objects by their name.
		/// </summary>
		[EnumMember(Value = "name")]
		Name,

		/// <summary>
		/// Sorts the objects by the date they were added.
		/// </summary>
		[EnumMember(Value = "dateAdded")]
		DateAddded,

		/// <summary>
		/// Sorts the objects by the date of their last modification.
		/// </summary>
		[EnumMember(Value = "dateModified")]
		DateModified,

		/// <summary>
		/// Sorts the objects by the number of assigned quotes.
		/// </summary>
		[EnumMember(Value = "quoteCount")]
		QuoteCount
	}
}
