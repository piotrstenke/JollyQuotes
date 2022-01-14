using System;
using System.Threading.Tasks;

namespace JollyQuotes
{
	/// <summary>
	/// Contains various utility extension methods widely used through the <c>JollyQuotes</c> libraries.
	/// </summary>
	public static class QuoteExtensions
	{
		/// <summary>
		/// Performs an <paramref name="action"/> on result of the specified <paramref name="task"/>.
		/// </summary>
		/// <typeparam name="T">Type of data the task returns.</typeparam>
		/// <param name="task"><see cref="Task{TResult}"/> that returns the result to perform the <paramref name="action"/> on.</param>
		/// <param name="action">Action to perform.</param>
		/// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>. -or- <paramref name="action"/> is <see langword="null"/>.</exception>
		public static Task<ResolverResponse<T>> OnResponse<T>(this Task<ResolverResponse<T>> task, Action<ResolverResponse<T>> action)
		{
			if (task is null)
			{
				throw Error.Null(nameof(task));
			}

			if (action is null)
			{
				throw Error.Null(nameof(action));
			}

			return task.ContinueWith(t =>
			{
				ResolverResponse<T> response = t.Result;
				action(response);
				return response;
			});
		}

		/// <summary>
		/// Performs an <paramref name="function"/> on result of the specified <paramref name="task"/> and returns a new object.
		/// </summary>
		/// <typeparam name="T">Type of data the task returns.</typeparam>
		/// <typeparam name="U">Type of data that the <paramref name="function"/> returns.</typeparam>
		/// <param name="task"><see cref="Task{TResult}"/> that returns the result to perform the <paramref name="function"/> on.</param>
		/// <param name="function">Function to use to mutate the result of the target <paramref name="task"/>.</param>
		/// <exception cref="ArgumentNullException"><paramref name="task"/> is <see langword="null"/>. -or- <paramref name="function"/> is <see langword="null"/>.</exception>
		public static Task<U> OnResponse<T, U>(this Task<ResolverResponse<T>> task, Func<ResolverResponse<T>, U> function)
		{
			if (task is null)
			{
				throw Error.Null(nameof(task));
			}

			if (function is null)
			{
				throw Error.Null(nameof(function));
			}

			return task.ContinueWith(t =>
			{
				ResolverResponse<T> response = t.Result;
				return function(response);
			});
		}

		/// <summary>
		/// Converts the specified <paramref name="quote"/> to a new instance of the <see cref="Quote"/> class.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to convert.</param>
		public static Quote ToGeneric(this IQuote quote)
		{
			return new Quote(
				quote.Value,
				quote.Author,
				quote.Source,
				quote.Date,
				quote.Tags
			);
		}

		/// <summary>
		/// Converts the specified <paramref name="quote"/> to a new instance of the <see cref="QuoteWithId"/> class with the given <paramref name="id"/> as parameter.
		/// </summary>
		/// <param name="quote"><see cref="IQuote"/> to convert.</param>
		/// <param name="id">Id to assign to the <paramref name="quote"/>.</param>
		public static QuoteWithId WithId(this IQuote quote, Id id)
		{
			if (quote is QuoteWithId q && q.Id == id)
			{
				return q;
			}

			return new QuoteWithId(
				id,
				quote.Value,
				quote.Author,
				quote.Source,
				quote.Date,
				quote.Tags
			);
		}
	}
}
