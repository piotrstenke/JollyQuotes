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
		Name = 0,

		/// <summary>
		/// Sorts the objects by the date they were added.
		/// </summary>
		[EnumMember(Value = "dateAdded")]
		DateAddded = 1,

		/// <summary>
		/// Sorts the objects by the date of their last modification.
		/// </summary>
		[EnumMember(Value = "dateModified")]
		DateModified = 2,

		/// <summary>
		/// Sorts the objects by the number of assigned quotes.
		/// </summary>
		[EnumMember(Value = "quoteCount")]
		QuoteCount = 3
	}
}
