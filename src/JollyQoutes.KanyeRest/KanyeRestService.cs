using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes.KanyeRest
{
	/// <inheritdoc cref="IKanyeRestService"/>
	public class KanyeRestService : IKanyeRestService, IQuoteService
	{
		/// <summary>
		/// <see cref="IResourceResolver"/> that is used to access requested <c>kanye.rest</c> resources.
		/// </summary>
		public IResourceResolver Resolver { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeRestService(HttpClient client)
		{
			Resolver = new HttpResolver(client);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested <c>kanye.rest</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public KanyeRestService(IResourceResolver resolver)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			Resolver = resolver;
		}

		/// <inheritdoc/>
		public Task<List<KanyeQuote>> GetAllQuotes()
		{
			return Resolver.ResolveAsync<List<KanyeQuote>>(KanyeResources.Database);
		}

		/// <inheritdoc/>
		public Task<KanyeQuote> GetRandomQuote()
		{
			return Resolver.ResolveAsync<KanyeQuote>(KanyeResources.APIPage);
		}
	}
}
