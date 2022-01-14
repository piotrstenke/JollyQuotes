using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

namespace JollyQuotes
{
	/// <summary>
	/// <see cref="ResolverResponse{T}"/> that was created as a result of a HTTP request to a <see cref="IResourceResolver"/>.
	/// </summary>
	/// <typeparam name="T">Type of returned data.</typeparam>
	public class HttpResolverResponse<T> : ResolverResponse<T>
	{
		/// <summary>
		/// Text content of the response.
		/// </summary>
		public string? Content => base.Response;

		/// <inheritdoc cref="ResolverResponse{T}.IsSuccess"/>
		[MemberNotNullWhen(true, nameof(Response))]
		public new bool IsSuccess => base.IsSuccess;

		/// <summary>
		/// <see cref="HttpResponseMessage"/> that was created as a result of a HTTP request.
		/// </summary>
		public new HttpResponseMessage? Response { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolverResponse{T}"/> class with an underlaying <paramref name="response"/> specified.
		/// </summary>
		/// <param name="response"><see cref="HttpResponseMessage"/> that was created as a result of a HTTP request.</param>
		/// <exception cref="ArgumentNullException"><paramref name="response"/> is <see langword="null"/>.</exception>
		public HttpResolverResponse(HttpResponseMessage response) : this(response, default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolverResponse{T}"/> class with an underlaying <paramref name="response"/> and <paramref name="exception"/> specified.
		/// </summary>
		/// <param name="response"><see cref="HttpResponseMessage"/> that was created as a result of a HTTP request.</param>
		/// <param name="exception"><see cref="Exception"/> that caused the request to fail.</param>
		/// <exception cref="ArgumentNullException"><paramref name="response"/> is <see langword="null"/>.</exception>
		public HttpResolverResponse(HttpResponseMessage response, Exception? exception) : base(exception)
		{
			if (response is null)
			{
				throw Error.Null(nameof(response));
			}

			Response = response;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolverResponse{T}"/> class with an underlaying <paramref name="response"/>, <paramref name="content"/> and <paramref name="result"/> specified.
		/// </summary>
		/// <param name="response"><see cref="HttpResponseMessage"/> that was created as a result of a HTTP request.</param>
		/// <param name="content">Text content of the response.</param>
		/// <param name="result">Result of the request.</param>
		/// <exception cref="ArgumentNullException"><paramref name="response"/> is <see langword="null"/>.</exception>
		public HttpResolverResponse(HttpResponseMessage response, string? content = default, T? result = default)
			: base(response.IsSuccessStatusCode, content, result)
		{
			if (response is null)
			{
				throw Error.Null(nameof(response));
			}

			Response = response;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResolverResponse{T}"/> class with an underlaying <paramref name="exception"/> specified.
		/// </summary>
		/// <param name="exception"><see cref="Exception"/> that caused the request to fail.</param>
		public HttpResolverResponse(Exception exception) : base(exception)
		{
			if (exception is null)
			{
				throw Error.Null(nameof(exception));
			}
		}
	}
}
