using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JollyQuotes
{
	internal static class Internals
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException MustBeGreaterThan(string paramName, object value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than '{value}'");
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentOutOfRangeException MustBeGreaterThanOrEqualTo(string paramName, object value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than or equal to '{value}'");
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentNullException Null(string paramName)
		{
			return new ArgumentNullException(paramName);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException NullOrEmpty(string paramName)
		{
			return new ArgumentException($"{paramName} cannot be null or empty", paramName);
		}
	}
}
