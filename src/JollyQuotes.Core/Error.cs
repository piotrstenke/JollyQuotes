using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace JollyQuotes
{
	internal static class Error
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException Arg(string? message)
		{
			return new ArgumentException(message);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentException Arg(string? message, string paramName)
		{
			return new ArgumentException(message, paramName);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static InvalidOperationException InvOp(string? message)
		{
			return new InvalidOperationException(message);
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeGreaterThan(string paramName, object? value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than {value}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeGreaterThan(string paramName1, string paramName2)
		{
			return new ArgumentOutOfRangeException(paramName1, $"value must be greater than the value of '{paramName2}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeGreaterThanOrEqualTo(string paramName, object? value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be greater than or equal to {value}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeGreaterThanOrEqualTo(string paramName1, string paramName2)
		{
			return new ArgumentOutOfRangeException(paramName1, $"value must be greater than or equal to the value of '{paramName2}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeLessThan(string paramName, object? value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be less than {value}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeLessThan(string paramName1, string paramName2)
		{
			return new ArgumentOutOfRangeException(paramName1, $"value must be less than the value of '{paramName2}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeLessThanOrEqualTo(string paramName, object? value)
		{
			return new ArgumentOutOfRangeException(paramName, $"value must be less than or equal to {value}");
		}

		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException MustBeLessThanOrEqualTo(string paramName1, string paramName2)
		{
			return new ArgumentOutOfRangeException(paramName1, $"value must be less than or equal to the value of '{paramName2}");
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ArgumentNullException Null(string paramName)
		{
			return new ArgumentNullException(paramName);
		}

		[DebuggerStepThrough]
		public static ArgumentException NullOrEmpty(string paramName)
		{
			return new ArgumentException($"{paramName} cannot be null or empty", paramName);
		}
	}
}
