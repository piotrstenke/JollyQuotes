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
		/// <remarks>The default value is <see langword="true"/>.</remarks>
		public bool DisposeClient { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolver"/> class with an underlaying <paramref name="client"/> specified.
		/// </summary>
		/// <param name="client"><see cref="HttpClient"/> that is used to access the requested resources.</param>
		/// <exception cref="ArgumentNullException"><paramref name="client"/> is <see langword="null"/>.</exception>
		public HttpResolver(HttpClient client) : this(client, true)
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
				throw Throw.Null(nameof(client));
			}

			BaseClient = client;
			DisposeClient = dispose;
		}

		/// <summary>
		/// Finalizes the current instance.
		/// </summary>
		~HttpResolver()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases unmanaged resources held by the current instance.
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
			HttpResponseMessage response = await GetResponse(source);
			response.EnsureSuccessStatusCode();

			string json = await response.Content.ReadAsStringAsync();
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
			HttpResponseMessage response = await GetResponse(source);
			response.EnsureSuccessStatusCode();

			return await response.Content.ReadAsStreamAsync();
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
			HttpResponseMessage response = await GetResponse(source);

			if (!response.IsSuccessStatusCode)
			{
				return default;
			}

			string json = await response.Content.ReadAsStringAsync();
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
			HttpResponseMessage response = await GetResponse(source);

			if (!response.IsSuccessStatusCode)
			{
				return null;
			}

			return await response.Content.ReadAsStreamAsync();
		}

		/// <summary>
		/// Releases unmanaged resources held by the current instance.
		/// </summary>
		/// <param name="disposing">Determines whether this method was called from the <see cref="Dispose()"/> or by a finalizer.</param>
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
				throw Throw.NullOrEmpty(nameof(source));
			}

			return await BaseClient.GetAsync(source);
		}
	}
}
