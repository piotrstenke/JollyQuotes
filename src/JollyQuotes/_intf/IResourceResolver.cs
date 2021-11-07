using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <summary>
	/// Provides a mechanism for resolving resources of type <c>T</c> from a specified, <see cref="string"/>-defined source.
	/// </summary>
	public interface IResourceResolver
	{
		/// <summary>
		/// Resolves a resource of type <typeparamref name="T"/> using data found in the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">Type of resource to resolve.</typeparam>
		/// <param name="source">Source of data used to resolve the resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		T Resolve<T>(string source);

		/// <summary>
		/// Asynchronously resolves a resource of type <typeparamref name="T"/> using data found in the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">Type of resource to resolve.</typeparam>
		/// <param name="source">Source of data used to resolve the resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Task<T> ResolveAsync<T>(string source);

		/// <summary>
		/// Attempts to resolve a resource of type <typeparamref name="T"/> using data found in the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">Type of resource to resolve.</typeparam>
		/// <param name="source">Source of data used to resolve the resource.</param>
		/// <param name="resource">Resolved resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		/// <returns><see langword="true"/> if the resource was successfully resolved, <see langword="false"/> otherwise.</returns>
		bool TryResolve<T>(string source, [NotNullWhen(true)] out T? resource);

		/// <summary>
		/// Attempts to asynchronously resolve a resource of type <typeparamref name="T"/> using data found in the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">Type of resource to resolve.</typeparam>
		/// <param name="source">Source of data used to resolve the resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Task<T?> TryResolveAsync<T>(string source);
	}
}
