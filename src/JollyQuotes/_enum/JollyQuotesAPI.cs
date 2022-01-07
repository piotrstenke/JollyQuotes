using System;

namespace JollyQuotes
{
	/// <summary>
	/// Defines all existing JollyQuotes modules.
	/// </summary>
	[Flags]
	public enum JollyQuotesApi
	{
		/// <summary>
		/// API that is not supported by JollyQuotes.
		/// </summary>
		None = 0,

		/// <summary>
		/// The <c>kanye.rest</c> API.
		/// </summary>
		KanyeRest = 1,

		/// <summary>
		/// The <c>Tronald Dump</c> API.
		/// </summary>
		TronaldDump = 2,

		/// <summary>
		/// The <c>quotable.io</c> API.
		/// </summary>
		Quotable = 4,

		/// <summary>
		/// All <c>JollyQuotes</c> APIs.
		/// </summary>
		All = KanyeRest | TronaldDump | Quotable
	}
}
