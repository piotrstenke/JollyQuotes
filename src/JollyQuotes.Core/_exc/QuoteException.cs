using System;
using System.Runtime.Serialization;

namespace JollyQuotes
{
	/// <summary>
	/// An exception thrown when there was an error associated with generating quotes.
	/// </summary>
	public class QuoteException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteException"/> class.
		/// </summary>
		public QuoteException()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="QuoteException"/> class with a specified error <paramref name="message"/>.
		/// </summary>
		/// <param name="message">Message that describes the reason for the exception.</param>
		public QuoteException(string? message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteException"/> class with a specified error <paramref name="message"/>
		/// and a reference to the <paramref name="innerException"/> that is the cause of this exception.
		/// </summary>
		/// <param name="message">Message that describes the reason for the exception.</param>
		/// <param name="innerException"><see cref="Exception"/> that is the cause of the current exception.</param>
		public QuoteException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuoteException"/> class with serialized data.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context"><see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="ArgumentNullException"><paramref name="info"/> is <see langword="null"/>.</exception>
		/// <exception cref="SerializationException">The class name is <see langword="null"/> or <see cref="Exception.HResult"/> is zero (<c>0</c>).</exception>
		protected QuoteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
