using System;
using System.Diagnostics.CodeAnalysis;
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
			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			T? t = JsonConvert.DeserializeObject<T>(json, Settings.JsonSettings);

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
		public async Task<Stream> ResolveStreamAsync(string source)
		{
			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
		}

		/// <summary>
		/// Attempts to download json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <param name="resource">Resolved resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <returns><see langword="true"/> if the data was successfully downloaded, <see langword="false"/> otherwise.</returns>
		public virtual bool TryResolve<T>(string source, [NotNullWhen(true)] out T? resource)
		{
			resource = TryResolveAsync<T>(source).Result;

			return resource is not null;
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
		public virtual async Task<T?> TryResolveAsync<T>(string source)
		{
			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				return default;
			}

			string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			return JsonConvert.DeserializeObject<T>(json, Settings.JsonSettings);
		}

		/// <summary>
		/// Attempts to download a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <param name="stream"><see cref="Stream"/> that contains the downloaded data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <returns><see langword="true"/> if the data was successfully downloaded, <see langword="false"/> otherwise.</returns>
		public bool TryResolveStream(string source, [NotNullWhen(true)] out Stream? stream)
		{
			stream = TryResolveStreamAsync(source).Result;
			return stream is not null;
		}

		/// <summary>
		/// Attempts to asynchronously download a <see cref="Stream"/> of data from the specified <paramref name="source"/>.
		/// </summary>
		/// <param name="source">Source of data to download.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		public async Task<Stream?> TryResolveStreamAsync(string source)
		{
			HttpResponseMessage response = await GetResponse(source).ConfigureAwait(false);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
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

		private async Task<HttpResponseMessage> GetResponse(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Error.NullOrEmpty(nameof(source));
			}

			return await BaseClient.GetAsync(source).ConfigureAwait(false);
		}
	}
}
