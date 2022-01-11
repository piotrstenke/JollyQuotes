using System.Net.Http;
using System;
using System.Threading.Tasks;
using JollyQuotes.Quotable.Models;

namespace JollyQuotes.Quotable
{
	/// <inheritdoc cref="IQuotableService"/>
	public class QuotableService : QuoteService, IQuotableService
	{
		/// <summary>
		/// <see cref="IQuotableModelConverter"/> used to convert data received from the <c>quotable</c> API to usable objects.
		/// </summary>
		public IQuotableModelConverter ModelConverter { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableService"/> class.
		/// </summary>
		public QuotableService() : this(QuotableResources.CreateDefaultResolver())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="QuoteService.Resolver"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public QuotableService(HttpClient client) : this(client, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableService"/> class with a <paramref name="client"/> as the target <see cref="IResourceResolver"/> and a <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that will be used as the target <see cref="QuoteService.Resolver"/>.</param>
		/// <param name="modelConverter"><see cref="IQuotableModelConverter"/> used to convert data received from the <c>quotable</c> API to usable objects.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public QuotableService(HttpClient client, IQuotableModelConverter? modelConverter) : this(new HttpResolver(client), modelConverter)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableService"/> class with an underlaying <paramref name="resolver"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested <c>quotable</c> resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public QuotableService(IResourceResolver resolver) : this(resolver, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuotableService"/> class with an underlaying <paramref name="resolver"/> and a <paramref name="modelConverter"/> specified.
		/// </summary>
		/// <param name="resolver"><see cref="IResourceResolver"/> that is used to access requested <c>quotable</c> resources.</param>
		/// <param name="modelConverter"><see cref="IQuotableModelConverter"/> used to convert data received from the <c>quotable</c> API to usable objects.</param>
		/// <exception cref="ArgumentNullException"><paramref name="resolver"/> is <see langword="null"/>.</exception>
		public QuotableService(IResourceResolver resolver, IQuotableModelConverter? modelConverter) : base(resolver)
		{
			ModelConverter = modelConverter ?? new QuotableModelConverter();
		}

		/// <inheritdoc/>
		public Task<QuoteModel> GetQuote(string id)
		{
			if(string.IsNullOrWhiteSpace(id))
			{
				throw Error.NullOrEmpty(nameof(id));
			}

			return Resolver.TryResolveAsync<QuoteModel>($"quotes/{id}").ContinueWith(t =>
			{
				if(t.Result is null)
				{
					throw Error.Quote($"Quote with id '{id}' does not exist");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public Task<QuoteModel> GetRandomQuote()
		{
			return Resolver.ResolveAsync<QuoteModel>("random");
		}

		/// <inheritdoc/>
		public Task<QuoteModel> GetRandomQuote(QuoteSearchModel searchModel)
		{
			if(searchModel is null)
			{
				throw Error.Null(nameof(searchModel));
			}

			string link = "random";
			string query = ModelConverter.GetSearchQuery(searchModel);
			Internals.ApplyQuery(ref link, query);

			return Resolver.TryResolveAsync<QuoteModel>(link).ContinueWith(t =>
			{
				if(t.Result is null)
				{
					throw Error.Quote("Could not find any matching quote");
				}

				return t.Result;
			});
		}
	}
}
