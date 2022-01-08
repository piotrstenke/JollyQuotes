using System;
using System.Collections.Generic;

namespace JollyQuotes
{
	/// <summary>
	/// Randomly picks an <see cref="NamedOption"/> from a set of available options with specific possibilities of occurring.
	/// </summary>
	public interface IOptionalPossibility : IPossibility, IEnumerable<NamedOption>
	{
		/// <summary>
		/// Returns a randomly picked <see cref="NamedOption"/>.
		/// </summary>
		new NamedOption Determine();

		/// <summary>
		/// Returns an <see cref="NamedOption"/> with the specified <paramref name="name"/>.
		/// </summary>
		/// <param name="name">Name of <see cref="NamedOption"/> to return.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// Option with the specified <paramref name="name"/> does not exist.
		/// </exception>
		NamedOption GetOption(string name);

		/// <summary>
		/// Returns an array of all non-default <see cref="NamedOption"/>s sorted from lowest to greatest.
		/// </summary>
		NamedOption[] GetOptions();
	}
}
