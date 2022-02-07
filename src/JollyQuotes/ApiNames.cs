using JollyQuotes.KanyeRest;
using JollyQuotes.Quotable;
using JollyQuotes.TronaldDump;

namespace JollyQuotes
{
	/// <summary>
	/// Contains names of all <c>JollyQuotes</c> APIs.
	/// </summary>
	public static class ApiNames
	{
		/// <summary>
		/// Name of the <c>JollyQuotes</c> API.
		/// </summary>
		public const string JollyQuotes = "JollyQuotes";

		/// <summary>
		/// Name of  the <c>kanye.rest</c> API.
		/// </summary>
		public const string KanyeRest = KanyeRestResources.ApiName;

		/// <summary>
		/// Name of the <c>quotable</c> API.
		/// </summary>
		public const string Quotable = QuotableResources.ApiName;

		/// <summary>
		/// Name of the <c>Tronald Dump</c> API.
		/// </summary>
		public const string TronaldDump = TronaldDumpResources.ApiName;

		internal static string[] GetAll()
		{
			return new[]
			{
				KanyeRest,
				TronaldDump,
				Quotable
			};
		}
	}
}
