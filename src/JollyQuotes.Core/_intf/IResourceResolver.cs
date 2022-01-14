using System;
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
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		ResolverResponse<T> TryResolve<T>(string source);

		/// <summary>
		/// Attempts to asynchronously resolve a resource of type <typeparamref name="T"/> using data found in the <paramref name="source"/>.
		/// </summary>
		/// <typeparam name="T">Type of resource to resolve.</typeparam>
		/// <param name="source">Source of data used to resolve the resource.</param>
		/// <exception cref="ArgumentException"><paramref name="source"/> is <see langword="null"/> or empty.</exception>
		Task<ResolverResponse<T>> TryResolveAsync<T>(string source);
	}
}
