using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IResourceResolver"/> that uses a <see cref="HttpClient"/> to access the requested resources.
	/// </summary>
	public class HttpResolver : IResourceResolver, IDisposable
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
		/// <exception cref="InvalidOperationException">Object could not be deserialized from <paramref name="source"/>.</exception>
		public T Resolve<T>(string source)
		{
			return ResolveAsync<T>(source).Result;
		}

		/// <summary>
		/// Asynchronously downloads json data from the specified <paramref name="source"/> and deserializes it into a new <typeparamref name="T"/> object.
		/// </summary>
		/// <param name="source">Link to the json data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <exception cref="InvalidOperationException">Object could not be deserialized from <paramref name="source"/>.</exception>
		public Task<T> ResolveAsync<T>(string source)
		{
			return TryResolveAsync<T>(source).ContinueWith(t =>
			{
				if (t.Result is null)
				{
					throw new InvalidOperationException($"Object could not be deserialized from source '{source}'");
				}

				return t.Result;
			});
		}

		/// <inheritdoc/>
		public bool TryResolve<T>(string source, [NotNullWhen(true)] out T? resource)
		{
			resource = TryResolveAsync<T>(source).Result;

			return resource is not null;
		}

		/// <inheritdoc/>
		public async Task<T?> TryResolveAsync<T>(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
			{
				throw Throw.NullOrEmpty(nameof(source));
			}

			HttpResponseMessage response = await BaseClient.GetAsync(source);
			string json = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(json, Settings.JsonSettings);
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
	}
}
