using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IResourceResolver"/> that uses a <see cref="HttpClient"/> to access the requested resources.
	/// </summary>
	public class HttpResolver : IStreamResolver, IDisposable
	{
		private bool _disposed;

		/// <summary>
		/// <see cref="HttpClient"/> that is used to access the requested resources.
		/// </summary>
		public HttpClient BaseClient { get; }

		/// <summary>
		/// Determines whether to dispose the <see cref="BaseClient"/> when <see cref="Dispose()"/> is called.
		/// </summary>
		/// <remarks>The default value is <see langword="false"/>.</remarks>
		public bool DisposeClient { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolver"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public HttpResolver(HttpClient client) : this(client, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolver"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used to access the requested resources.</param>
		/// <param name="dispose">Determines whether to dispose the <paramref name="client"/> when <see cref="Dispose()"/> is called.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public HttpResolver(HttpClient client, bool dispose)
		{
			if (client is null)
			{
				throw Error.Null(nameof(client));
			}

			BaseClient = client;
			DisposeClient = dispose;
		}

		/// <summary>
		/// Returns a new instance of <see cref="HttpResolver"/> with default options applied.
		/// </summary>
		public static HttpResolver CreateDefault()
		{
			HttpClient client = Internals.CreateDefaultClient();
			return new HttpResolver(client);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="HttpResolver"/> and optionally disposes of the managed resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Downloads json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="HttpRequestException">
		/// The HTTP response is unsuccessful. -or-
		/// Object could not be deserialized from response.
		/// </exception>
		public virtual T Resolve<T>(string source)
		{
			return ResolveAsync<T>(source).Result;
		}

		/// <summary>
		/// Asynchronously downloads json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="HttpRequestException">
		/// The HTTP response is unsuccessful. -or-
		/// Object could not be deserialized from response.
		/// </exception>
		public virtual async Task<T> ResolveAsync<T>(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			T? t = JsonConvert.DeserializeObject<T>(json, Quote.JsonSettings);

			if (t is null)
			{
				throw new HttpRequestException($"Object could not be deserialized from source '{source}'");
			}

			return t;
		}

		/// <summary>
		/// Downloads a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
		public Stream ResolveStream(string source)
		{
			return ResolveStreamAsync(source).Result;
		}

		/// <summary>
		/// Asynchronously downloads a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="HttpRequestException">The HTTP response is unsuccessful.</exception>
		public virtual async Task<Stream> ResolveStreamAsync(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Attempts to download json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		public virtual HttpResolverResponse<T> TryResolve<T>(string source)
		{
			return TryResolveAsync<T>(source).Result;
		}

		/// <summary>
		/// Attempts to asynchronously download json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="HttpRequestException">
		/// The HTTP response is unsuccessful. -or-
		/// Object could not be deserialized from response.
		/// </exception>
		public virtual Task<HttpResolverResponse<T>> TryResolveAsync<T>(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			return TryResolve(source, response => response.Content.ReadAsStringAsync().ContinueWith(t =>
			{
				string json = t.Result;
				T? result = JsonConvert.DeserializeObject<T>(json, Quote.JsonSettings);

				return new HttpResolverResponse<T>(response, json, result);
			}));
		}

		/// <summary>
		/// Attempts to download a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <returns><see langword="true"/> if the data was successfully downloaded, <see langword="false"/> otherwise.</returns>
		public virtual HttpResolverResponse<Stream> TryResolveStream(string source)
		{
			return TryResolveStreamAsync(source).Result;
		}

		/// <summary>
		/// Attempts to asynchronously download a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		public virtual Task<HttpResolverResponse<Stream>> TryResolveStreamAsync(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			return TryResolve(source, response => response.Content.ReadAsStreamAsync().ContinueWith(t =>
			{
				Stream stream = t.Result;
				return new HttpResolverResponse<Stream>(response, result: stream);
			}));
		}

		ResolverResponse<T> IResourceResolver.TryResolve<T>(string source)
		{
			return ((IResourceResolver)this).TryResolveAsync<T>(source).Result;
		}

		async Task<ResolverResponse<T>> IResourceResolver.TryResolveAsync<T>(string source)
		{
			HttpResolverResponse<T> response = await TryResolveAsync<T>(source).ConfigureAwait(false);
			return response;
		}

		ResolverResponse<Stream> IStreamResolver.TryResolveStream(string source)
		{
			return ((IStreamResolver)this).TryResolveStreamAsync(source).Result;
		}

		async Task<ResolverResponse<Stream>> IStreamResolver.TryResolveStreamAsync(string source)
		{
			HttpResolverResponse<Stream> response = await TryResolveStreamAsync(source).ConfigureAwait(false);
			return response;
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="HttpResolver"/> and optionally disposes of the managed resources.
		/// </summary>
		/// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources;<see langword="false"/> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing && DisposeClient)
				{
					BaseClient.Dispose();
				}

				_disposed = true;
			}
		}

		/// <summary>
		/// Provides default mechanism for accessing web data.
		/// </summary>
		/// <typeparam name="T">Type of data to access.</typeparam>
		/// <param name="source">Source of the data.</param>
		/// <param name="onSuccess">Function performed when a <see cref="HttpResponseMessage"/> is successful.</param>
		protected async Task<HttpResolverResponse<T>> TryResolve<T>(string source, Func<HttpResponseMessage, Task<HttpResolverResponse<T>>> onSuccess)
		{
			try
			{
				HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);

				try
				{
					if (response.IsSuccessStatusCode)
					{
						return await onSuccess(response).ConfigureAwait(false);
					}

					return new HttpResolverResponse<T>(response);
				}
				catch (Exception e)
				{
					return new HttpResolverResponse<T>(response, e);
				}
			}
			catch (Exception e)
			{
				return new HttpResolverResponse<T>(e);
			}
		}

		private async Task<HttpResponseMessage> GetResponse(string source)
		{
			return await BaseClient.GetAsync(source).ConfigureAwait(false);
		}
	}
}
