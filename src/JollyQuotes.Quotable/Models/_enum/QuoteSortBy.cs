using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Specifies sort methods for quotes.
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	public enum QuoteSortBy
	{
		/// <summary>
		/// Sorts the quotes by the date they were added.
		/// </summary>
		[EnumMember(Value = "dateAdded")]
		DateAddded = 0,

		/// <summary>
		/// Sorts the quotes by the date of their last modification.
		/// </summary>
		[EnumMember(Value = "dateModified")]
		DateModified = 1,

		/// <summary>
		/// Sorts the quotes by their author.
		/// </summary>
		[EnumMember(Value = "author")]
		Author = 2,

		/// <summary>
		/// Sorts the quotes by their content.
		/// </summary>
		[EnumMember(Value = "content")]
		Content = 3
	}
}
