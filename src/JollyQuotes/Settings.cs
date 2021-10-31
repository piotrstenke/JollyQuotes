using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JollyQuotes
{
	/// <summary>
	/// Contains read-only properties that define how the <c>JollyQuotes</c> libraries behave internally.
	/// </summary>
	public static class Settings
	{
		/// <summary>
		/// <see cref="JsonConverter"/> that formats <see cref="DateTime"/> and <see cref="DateTimeOffset"/> values to use only the <c>date</c> part.
		/// </summary>
		public sealed class DateOnlyConverter : IsoDateTimeConverter
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="DateOnlyConverter"/> class.
			/// </summary>
			public DateOnlyConverter()
			{
				DateTimeFormat = Settings.DateTimeFormat;
			}
		}

		/// <summary>
		/// Format of date time values used by <c>JollyQuotes</c> libraries.
		/// </summary>
		public const string DateTimeFormat = "yyyy-MM-dd";

		/// <summary>
		/// <see cref="JsonSerializerSettings"/> used to internally deserialize json objects.
		/// </summary>
		public static JsonSerializerSettings JsonSettings { get; } = new()
		{
			DateFormatString = DateTimeFormat
		};
	}
}
