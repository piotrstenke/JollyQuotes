using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="IResourceResolver"/> that provides mechanism for resolving data into a <see cref="Stream"/> instead of a hard <see cref="Type"/>.
	/// </summary>
	public interface IStreamResolver : IResourceResolver
	{
		/// <summary>
		/// Resolves a resource from the specified <paramref name="source"/> into a <see cref="Stream"/> of data.
		/// </summary>
		/// <param name="source">Source of data to resolve into a <see cref="Stream"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Stream ResolveStream(string source);

		/// <summary>
		/// Asynchronously resolves a resource from the specified <paramref name="source"/> into a <see cref="Stream"/> of data.
		/// </summary>
		/// <param name="source">Source of data to resolve into a <see cref="Stream"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Task<Stream> ResolveStreamAsync(string source);

		/// <summary>
		/// Attempts to resolve a resource from the specified <paramref name="source"/> into a <see cref="Stream"/> of data.
		/// </summary>
		/// <param name="source">Source of data to resolve into a <see cref="Stream"/>.</param>
		/// <param name="stream"><see cref="Stream"/> containing the resolved data.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <returns><see langword="true"/> if the resource was successfully resolved, <see langword="false"/> otherwise.</returns>
		bool TryResolveStream(string source, [NotNullWhen(true)] out Stream? stream);

		/// <summary>
		/// Attempts to asynchronously resolve a resource from the specified <paramref name="source"/> into a <see cref="Stream"/> of data.
		/// </summary>
		/// <param name="source">Source of data to resolve into a <see cref="Stream"/>.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Task<Stream?> TryResolveStreamAsync(string source);
	}
}
