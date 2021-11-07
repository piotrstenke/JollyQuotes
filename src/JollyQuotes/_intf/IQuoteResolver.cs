namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IRandomQuoteGenerator"/> that provides mechanism for generating quotes through an external API using an <see cref="IResourceResolver"/>.
	/// </summary>
	public interface IQuoteResolver : IRandomQuoteGenerator
	{
		/// <summary>
		/// <see cref="IResourceResolver"/> that is used to access requested resources.
		/// </summary>
		IResourceResolver Resolver { get; }
	}
}
