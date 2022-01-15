using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes.Quotable.Models
{
	/// <summary>
	/// Defines fields that a quote can be searched by.
	/// </summary>
	[Flags]
	[JsonConverter(typeof(StringEnumConverter))]
	public enum QuoteSearchFields
	{
		/// <summary>
		/// No fields.
		/// </summary>
		[EnumMember(Value = "none")]
		None = 0,

		/// <summary>
		/// Searches by the quotes content.
		/// </summary>
		[EnumMember(Value = "content")]
		Content = 1,

		/// <summary>
		/// Searches by the quotes author's name.
		/// </summary>
		[EnumMember(Value = "author")]
		Author = 2,

		/// <summary>
		/// Searches by the quotes tags.
		/// </summary>
		[EnumMember(Value = "tags")]
		Tags = 4,

		/// <summary>
		/// Searches by all fields.
		/// </summary>
		All = Content | Author | Tags
	}
}
