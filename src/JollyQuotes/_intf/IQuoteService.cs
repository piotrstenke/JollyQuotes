namespace JollyQuotes
{
	/// <summary>
	/// Performs actions on <see cref="IQuote"/>s using an external API accessed by an <see cref="IResourceResolver"/>.
	/// </summary>
	public interface IQuoteService
	{
		/// <summary>
		/// <see cref="IResourceResolver"/> that is used to access requested resources.
		/// </summary>
		IResourceResolver Resolver { get; }
	}
}
