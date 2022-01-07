using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JollyQuotes.KanyeRest
{
	/// <inheritdoc cref="IKanyeRestService"/>
	public class KanyeRestService : QuoteService, IKanyeRestService
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestService"/> class.
		/// </summary>
		public KanyeRestService() : base(HttpResolver.CreateDefault())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="QuoteService.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public KanyeRestService(HttpClient client) : base(client)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="KanyeRestService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested <c>kanye.rest</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public KanyeRestService(IResourceResolver resolver) : base(resolver)
		{
		}

		/// <inheritdoc/>
		public Task<List<KanyeRestQuote>> GetAllQuotes()
		{
			return Resolver.ResolveAsync<List<string>>(KanyeRestResources.Database).ContinueWith(t => t.Result.ConvertAll(q => new KanyeRestQuote(q)));
		}

		/// <inheritdoc/>
		public Task<KanyeRestQuote> GetRandomQuote()
		{
			return Resolver.ResolveAsync<KanyeRestQuote>(KanyeRestResources.APIPage);
		}
	}
}
