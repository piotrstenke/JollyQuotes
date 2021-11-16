namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a result of search action.
	/// </summary>
	public interface ISearchResultModel
	{
		/// <summary>
		/// Number of values contained on the current page.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Links to all the pages in the search result.
		/// </summary>
		PageHierarchyModel? Links { get; }

		/// <summary>
		/// Total number of values contained in the search result.
		/// </summary>
		int Total { get; }
	}
}
