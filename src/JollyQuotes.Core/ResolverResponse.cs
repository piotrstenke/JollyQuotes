using System;
using System.Diagnostics.CodeAnalysis;

namespace JollyQuotes
{
	/// <summary>
	/// A response from a <see cref="IResourceResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of returned data.</typeparam>
	public class ResolverResponse<T>
	{
		/// <summary>
		/// <see cref="System.Exception"/> that caused the request to fail.
		/// </summary>
		public Exception? Exception { get; }

		/// <summary>
		/// Determines whether <see cref="Result"/> is not <see langword="null"/>.
		/// </summary>
		[MemberNotNullWhen(true, nameof(Result))]
		public bool HasResult => Result is not null;

		/// <summary>
		/// Determines whether the response is a success.
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// Raw response from the resolver.
		/// </summary>
		public string? Response { get; }

		/// <summary>
		/// Result of the request.
		/// </summary>
		public T? Result { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ResolverResponse{T}"/> class.
		/// </summary>
		/// <param name="isSuccess">Determines whether the response is a success.</param>
		public ResolverResponse(bool isSuccess)
		{
			IsSuccess = isSuccess;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResolverResponse{T}"/> with an <paramref name="exception"/> specified.
		/// </summary>
		/// <param name="exception"><see cref="System.Exception"/> that caused the request to fail.</param>
		public ResolverResponse(Exception? exception)
		{
			IsSuccess = exception is null;
			Exception = exception;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResolverResponse{T}"/> class with underlaying <paramref name="response"/> and <paramref name="result"/> specified.
		/// </summary>
		/// <param name="isSuccess">Determines whether the response is a success.</param>
		/// <param name="response">Raw response from the resolver.</param>
		/// <param name="result">Result of the request.</param>
		public ResolverResponse(bool isSuccess, string? response = default, T? result = default)
		{
			IsSuccess = isSuccess;
			Response = response;
			Result = result;
		}
	}
}
