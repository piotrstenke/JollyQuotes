using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace JollyQuotes
{
	/// <summary>
	/// Provides methods for creation of quote-related objects for custom quote APIs.
	/// </summary>
	public class QuoteApiHandler : IBuiltInQuoteApiHandler, IEnumerable<string>
	{
		private sealed class QuoteEntry
		{
			private readonly Func<QuoteApiDescription> _descriptionFactory;
			private readonly Func<IResourceResolver, IQuoteGenerator> _generatorFactory;

			public string ApiName { get; }

			public QuoteEntry(
				string apiName,
				Func<QuoteApiDescription> descriptionFactory,
				Func<IResourceResolver, IQuoteGenerator> generatorFactory
			)
			{
				ApiName = apiName;
				_descriptionFactory = descriptionFactory;
				_generatorFactory = generatorFactory;
			}

			public QuoteApiDescription CreateDescription()
			{
				return _descriptionFactory();
			}

			public IQuoteGenerator CreateGenerator(IResourceResolver resolver)
			{
				return _generatorFactory(resolver);
			}
		}

		private readonly Dictionary<string, QuoteEntry> _entries;

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteApiHandler"/> class.
		/// </summary>
		public QuoteApiHandler()
		{
			_entries = new();
		}

		/// <summary>
		/// Adds an API with the specified <paramref name="apiName"/> to the handler.
		/// </summary>
		/// <param name="apiName">Name of the API to add.</param>
		/// <param name="descriptionFactory">Factory method that creates a new <see cref="QuoteApiDescription"/> for the target API.</param>
		/// <param name="generatorFactory">Factory method that creates a new <see cref="IQuoteGenerator"/> for the target API.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="descriptionFactory"/> is <see langword="null"/>. -or-
		/// <paramref name="generatorFactory"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> already exists.
		/// </exception>
		public void AddApi(
			string apiName,
			Func<QuoteApiDescription> descriptionFactory,
			Func<IResourceResolver, IQuoteGenerator> generatorFactory
		)
		{
			if (!TryAddApi(apiName, descriptionFactory, generatorFactory))
			{
				throw Error.Arg($"API with name '{apiName}' already exists", nameof(apiName));
			}
		}

		/// <inheritdoc/>
		public QuoteApiDescription CreateDescription(string apiName)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			return CreateDescription_Internal(apiName);
		}

		/// <inheritdoc/>
		public IQuoteGenerator CreateGenerator(string apiName, IResourceResolver resolver)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			return CreateGenerator_Internal(apiName, resolver);
		}

		/// <inheritdoc/>
		public IQuoteGenerator CreateGenerator(QuoteApiDescription description, IResourceResolver resolver)
		{
			if (description is null)
			{
				throw Error.Null(nameof(description));
			}

			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			return CreateGenerator_Internal(description.Name, resolver);
		}

		/// <summary>
		/// Converts the specified <paramref name="apiName"/> into most appropriate API name.
		/// </summary>
		/// <param name="apiName">Name of API to convert to the most appropriate form.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="apiName"/> is <see langword="null"/> or empty. -or-
		/// API with the specified <paramref name="apiName"/> not found.
		/// </exception>
		public string GetActualName(string apiName)
		{
			if (!TryGetActualName(apiName, out string? actualName))
			{
				throw QuoteUtility.Exc_UnknownApiName(apiName);
			}

			return actualName;
		}

		/// <inheritdoc/>
		public IEnumerable<string> GetApis()
		{
			return _entries.Keys;
		}

		/// <inheritdoc/>
		public bool HasApi(string apiName)
		{
			if (!TryGetActualName(apiName, out _))
			{
				if (string.IsNullOrWhiteSpace(apiName))
				{
					throw Error.NullOrEmpty(nameof(apiName));
				}

				return false;
			}

			return true;
		}

		/// <summary>
		/// Attempts to remove information about API with the specified <paramref name="apiName"/>.
		/// </summary>
		/// <param name="apiName"></param>
		/// <returns><see langword="true"/> is the API was successfully removed, <see langword="false"/> if no such API found.</returns>
		/// <exception cref="ArgumentException"><paramref name="apiName"/> is <see langword="null"/> or empty.</exception>
		public bool RemoveApi(string apiName)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			string key = GetApiKey(apiName);
			return _entries.Remove(key);
		}

		/// <summary>
		/// Attempts to add an API with the specified <paramref name="apiName"/> to the handler.
		/// </summary>
		/// <param name="apiName">Name of the API to add.</param>
		/// <param name="descriptionFactory">Factory method that creates a new <see cref="QuoteApiDescription"/> for the target API.</param>
		/// <param name="generatorFactory">Factory method that creates a new <see cref="IQuoteGenerator"/> for the target API.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="descriptionFactory"/> is <see langword="null"/>. -or-
		/// <paramref name="generatorFactory"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException"><paramref name="apiName"/> is <see langword="null"/> or empty.</exception>
		public bool TryAddApi(
			string apiName,
			Func<QuoteApiDescription> descriptionFactory,
			Func<IResourceResolver, IQuoteGenerator> generatorFactory
		)
		{
			if (string.IsNullOrWhiteSpace(apiName))
			{
				throw Error.NullOrEmpty(nameof(apiName));
			}

			if (descriptionFactory is null)
			{
				throw Error.Null(nameof(descriptionFactory));
			}

			if (generatorFactory is null)
			{
				throw Error.Null(nameof(generatorFactory));
			}

			if (_entries.ContainsKey(apiName))
			{
				return false;
			}

			QuoteEntry entry = new(apiName, descriptionFactory, generatorFactory);

			string key = GetApiKey(apiName);
			_entries.Add(key, entry);

			return true;
		}

		/// <summary>
		/// Attempts to convert the specified <paramref name="apiName"/> into most appropriate API name.
		/// </summary>
		/// <param name="apiName">Name of API to convert to the most appropriate form.</param>
		/// <param name="actualName">Converted API name.</param>
		/// <returns><see langword="true"/> if the <paramref name="apiName"/> was successfully converted, <see langword="false"/> otherwise.</returns>
		public bool TryGetActualName(string? apiName, [NotNullWhen(true)] out string? actualName)
		{
			if (!string.IsNullOrWhiteSpace(apiName))
			{
				string name = GetApiKey(apiName);

				if (_entries.TryGetValue(name, out QuoteEntry? entry))
				{
					actualName = entry.ApiName;
					return true;
				}
			}

			actualName = default;
			return false;
		}

		QuoteApiDescription IBuiltInQuoteApiHandler.CreateDescription(JollyQuotesApi api)
		{
			string name = QuoteUtility.GetActualApiName(api);
			return CreateDescription_Internal(name);
		}

		IQuoteGenerator IBuiltInQuoteApiHandler.CreateGenerator(JollyQuotesApi api, IResourceResolver resolver)
		{
			if (resolver is null)
			{
				throw Error.Null(nameof(resolver));
			}

			string name = QuoteUtility.GetActualApiName(api);

			return CreateGenerator_Internal(name, resolver);
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return GetApis().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<string>)this).GetEnumerator();
		}

		/// <summary>
		/// Converts the specified <paramref name="apiName"/> into its data key.
		/// </summary>
		/// <param name="apiName">Name of API to convert to the most appropriate form.</param>
		protected virtual string GetApiKey(string apiName)
		{
			return apiName.ToLowerInvariant().Trim();
		}

		private QuoteApiDescription CreateDescription_Internal(string apiName)
		{
			QuoteEntry entry = GetEntry(apiName);
			return entry.CreateDescription();
		}

		private IQuoteGenerator CreateGenerator_Internal(string apiName, IResourceResolver resolver)
		{
			QuoteEntry entry = GetEntry(apiName);

			return entry.CreateGenerator(resolver);
		}

		private QuoteEntry GetEntry(string apiName)
		{
			string apiKey = GetApiKey(apiName);

			if (!_entries.TryGetValue(apiKey, out QuoteEntry? entry))
			{
				throw QuoteUtility.Exc_UnknownApiName(apiName);
			}

			return entry;
		}
	}
}
