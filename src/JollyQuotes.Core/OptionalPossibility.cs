using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace JollyQuotes
{
	/// <summary>
	/// Randomly picks an <see cref="NamedOption"/> from a set of available options with specific possibilities of occurring.
	/// </summary>
	public class OptionalPossibility : IOptionalPossibility
	{
		/// <summary>
		/// Default value of <see cref="Capacity"/>.
		/// </summary>
		public const int DefaultCapacity = 16;

		/// <summary>
		/// Default value of <see cref="Divisor"/>/.
		/// </summary>
		public const int DefaultDivisor = 2;

		/// <summary>
		/// Default value of <see cref="Max"/>.
		/// </summary>
		public const int DefaultMax = 100;

		/// <summary>
		/// Default value of <see cref="DefaultOptionName"/>.
		/// </summary>
		public const string DefaultOption = "Default";

		private const int DEFAULT_OPTION_INDEX = -1;
		private const int STEP_OR_DIVIDER_SWITCH = -1;
		private readonly List<NamedOption> _list;
		private readonly HashSet<string> _names;
		private string _defaultOptionName;
		private int _divisor;
		private int _max;
		private int _step;

		/// <summary>
		/// Number of possibilities that can be added without resizing of internal data structure.
		/// </summary>
		/// <remarks>The default value is <c>16</c>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException"><see cref="Capacity"/> must be greater than or equal to <see cref="Count"/>.</exception>
		public int Capacity
		{
			get => _list.Capacity;
			set => _list.Capacity = value;
		}

		/// <summary>
		/// Number of existing options. The default option is also included if it exists.
		/// </summary>
		public int Count
		{
			get
			{
				int length = _list.Count;

				if (Remaining > 0)
				{
					length++;
				}

				return length;
			}
		}

		/// <summary>
		/// Name of the default option with possibility equal to the value <see cref="Remaining"/>.
		/// </summary>
		/// <remarks>The default value is <c>Default</c>.</remarks>
		/// <exception cref="ArgumentException">
		/// Value is <see langword="null"/> or empty. -or-
		/// Option with the specified name already exists.
		/// </exception>
		public string DefaultOptionName
		{
			get => _defaultOptionName;
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw Error.NullOrEmpty(nameof(value));
				}

				if (_defaultOptionName == value)
				{
					return;
				}

				if (!_names.Add(value))
				{
					throw Exc_NameAlreadyExists(value);
				}

				_names.Remove(_defaultOptionName);
				_defaultOptionName = value;
			}
		}

		/// <summary>
		/// Value that is used to automatically modify the value of <see cref="Step"/> when the value of <see cref="Count"/> is changed.
		/// <para>The formula is: <see cref="Step"/> = <see cref="Count"/> / <see cref="Divisor"/></para>
		/// </summary>
		/// <remarks>If <see cref="Step"/> is set manually, value of this property is automatically set to <c>-1</c>.
		/// <para>The default value is 2.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Value must be greater than <c>0</c>.</exception>
		public int Divisor
		{
			get => _divisor;
			set
			{
				if (value <= 0)
				{
					throw Error.MustBeGreaterThan(nameof(value), 0);
				}

				_divisor = value;
				CalculateStep();
			}
		}

		/// <summary>
		/// Determines whether the default <see cref="NamedOption"/> has possibility of occurring larger than <c>0</c>.
		/// </summary>
		public bool HasDefaultOption => Remaining < Max;

		/// <summary>
		/// Value all the possibilities sum to.
		/// </summary>
		/// <remarks>The default value is <c>100</c>.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Value must be greater than <c>0</c>. -or-
		/// Value must be greater than or equal to <see cref="Sum"/>.
		/// </exception>
		public int Max
		{
			get => _max;
			set
			{
				// If there are no NamedOptions registered, Max can be changed freely.

				if (Remaining == _max)
				{
					if (value < 0)
					{
						throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
					}

					Remaining = value;
				}
				else
				{
					if (Sum > value)
					{
						throw Error.MustBeGreaterThanOrEqualTo(nameof(value), nameof(Sum));
					}

					int diff = value - _max;
					Remaining += diff;
				}

				_max = value;

				if (Remaining == 0)
				{
					CalculateStep();
				}
			}
		}

		/// <summary>
		/// Number of existing options excluding the default option.
		/// </summary>
		public int ProperCount => _list.Count;

		/// <summary>
		/// <see cref="IRandomNumberGenerator"/> used to determine the <see cref="int"/> value that should be returned by the <see cref="Determine()"/> method.
		/// </summary>
		public IRandomNumberGenerator Random { get; }

		/// <summary>
		/// Difference between <see cref="Max"/> and sum of all registered <see cref="NamedOption"/>s.
		/// </summary>
		public int Remaining { get; private set; }

		/// <summary>
		/// If an index returned by <see cref="Determine(out int)"/> is greater than this value, this type's implementation of <see cref="IPossibility.Determine()"/> returns <see langword="true"/>.
		/// </summary>
		/// <remarks>If this property is set manually, value of <see cref="Divisor"/> is automatically set to <c>-1</c>.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Value must be greater than or equal to <c>0</c>. -or-
		/// Value must be less than or equal to <see cref="Count"/>.
		/// </exception>
		public int Step
		{
			get => _step;
			set
			{
				if (value < 0)
				{
					throw Error.MustBeGreaterThanOrEqualTo(nameof(value), 0);
				}

				if (value > Count)
				{
					throw Error.MustBeLessThanOrEqualTo(nameof(value), nameof(Count));
				}

				_step = value;
				_divisor = STEP_OR_DIVIDER_SWITCH;
			}
		}

		/// <summary>
		/// Sum of possibilities of all registered <see cref="NamedOption"/>'s.
		/// </summary>
		public int Sum => _max - Remaining;

		/// <inheritdoc cref="GetOption(int)"/>
		public NamedOption this[int index]
		{
			get
			{
				return GetOption(index);
			}
		}

		/// <inheritdoc cref="GetOption(string)"/>
		public NamedOption this[string name]
		{
			get
			{
				return GetOption(name);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OptionalPossibility"/> class.
		/// </summary>
		public OptionalPossibility() : this(new ThreadRandom())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OptionalPossibility"/> class with <paramref name="capacity"/> specified.
		/// </summary>
		/// <param name="capacity">Number of possibilities that can be added without resizing of internal data structure.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> must be greater than or equal to <see cref="Count"/>.</exception>
		public OptionalPossibility(int capacity) : this(new ThreadRandom(), capacity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OptionalPossibility"/> class with an underlaying random number generator specified.
		/// </summary>
		/// <param name="random"><see cref="IRandomNumberGenerator"/> used to determine the <see cref="int"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
		public OptionalPossibility(IRandomNumberGenerator random) : this(random, DefaultCapacity)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OptionalPossibility"/> class with an underlaying random number generator specified.
		/// </summary>
		/// <param name="random"><see cref="IRandomNumberGenerator"/> used to determine the <see cref="int"/> value that should be returned by the <see cref="Determine()"/> method.</param>
		/// <param name="capacity">Number of possibilities that can be added without resizing of internal data structure.</param>
		/// <exception cref="ArgumentNullException"><paramref name="random"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> must be greater than or equal to <see cref="Count"/>.</exception>
		public OptionalPossibility(IRandomNumberGenerator random, int capacity)
		{
			if (random is null)
			{
				throw Error.Null(nameof(random));
			}

			_list = new List<NamedOption>(capacity);
			_names = new();
			_defaultOptionName = DefaultOption;
			_names.Add(_defaultOptionName);
			Random = random;
			_max = DefaultMax;
			_divisor = DefaultDivisor;
			Remaining = _max;
		}

		/// <summary>
		/// Creates a new <see cref="OptionalPossibility"/> with the specified <paramref name="options"/> having equal possibility of occurring.
		/// </summary>
		/// <remarks>The possibility of each <see cref="NamedOption"/> is calculated with the following formula: <see cref="Max"/> / <paramref name="options"/><c>.Length</c>.
		/// <para>If the remainder is larger than <c>0</c>, it is added to the last <see cref="NamedOption"/>'s <see cref="NamedOption.Possibility"/>.</para></remarks>
		/// <param name="options">Array of options to include.</param>
		public static OptionalPossibility EqualOptions(string[]? options)
		{
			if (options is null)
			{
				return new();
			}

			int length = options.Length;
			OptionalPossibility possibility = new();

			if (length == 0)
			{
				return possibility;
			}

			int value = DefaultMax / length;

			foreach (string option in options)
			{
				possibility.AddOption(option, value);
			}

			int diff = DefaultMax - (value * length);

			if (diff > 0)
			{
				Span<NamedOption> span = CollectionsMarshal.AsSpan(possibility._list);
				ref NamedOption last = ref span[^1];
				last += diff;
			}

			return possibility;
		}

		/// <summary>
		/// Creates a new <see cref="OptionalPossibility"/> with the specified <paramref name="options"/> having equal <paramref name="possibility"/> of occurring.
		/// </summary>
		/// <param name="options">Array of options to include.</param>
		/// <param name="possibility">Possibility that is applied to all the <paramref name="options"/>.</param>
		/// <remarks>If <paramref name="options"/><c>.Length</c> is larger than <c>0</c>,
		/// <see cref="Max"/> is calculated with the following formula: <see cref="Max"/> = <paramref name="options"/><c>.Length * </c> <paramref name="possibility"/>.</remarks>
		/// <exception cref="ArgumentNullException"><paramref name="options"/> is <see langword="null"/>.</exception>
		public static OptionalPossibility EqualOptions(string[] options, int possibility)
		{
			if (options is null)
			{
				throw Error.Null(nameof(options));
			}

			if (possibility <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(possibility), 0);
			}

			int length = options.Length;
			OptionalPossibility op = new();

			if (length == 0)
			{
				return op;
			}

			op.Max = possibility * length;

			foreach (string option in options)
			{
				op.AddOption(option, possibility);
			}

			return op;
		}

		/// <summary>
		/// Registers the specified <paramref name="option"/>.
		/// </summary>
		/// <param name="option"><see cref="NamedOption"/> to register.</param>
		/// <returns>The current <see cref="OptionalPossibility"/> instance.</returns>
		/// <exception cref="ArgumentOutOfRangeException"><see cref="NamedOption.Possibility"/> of <paramref name="option"/> must be greater than <c>0</c>.</exception>
		public OptionalPossibility AddOption(NamedOption option)
		{
			return AddOption(option.Name, option.Possibility);
		}

		/// <summary>
		/// Registers a new <see cref="NamedOption"/> with the specified <paramref name="name"/> and <paramref name="possibility"/>.
		/// </summary>
		/// <param name="name">Name of <see cref="NamedOption"/> to register.</param>
		/// <param name="possibility">Possibility of the <see cref="NamedOption"/> occurring.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// Option with the specified <paramref name="name"/> already exists.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="possibility"/> must be greater than <c>0</c>. -or-
		/// <paramref name="possibility"/> must be less than or equal to <see cref="Remaining"/>.
		/// </exception>
		/// <exception cref="InvalidOperationException">Cannot add new <see cref="NamedOption"/> when the value of <see cref="Remaining"/> is equal to <c>0</c>.</exception>
		public OptionalPossibility AddOption(string name, int possibility)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if (possibility <= 0)
			{
				throw Error.MustBeGreaterThan(nameof(possibility), 0);
			}

			if (possibility < Remaining)
			{
				throw Error.MustBeLessThanOrEqualTo(nameof(possibility), nameof(Remaining));
			}

			if (Remaining <= 0)
			{
				throw Error.InvOp($"Cannot add new option when the value of '{nameof(Remaining)}' is equal to 0");
			}

			if (!_names.Add(name))
			{
				throw Exc_NameAlreadyExists(name);
			}

			Remaining -= possibility;
			AddToList();

			CalculateStep();

			return this;

			void AddToList()
			{
				NamedOption option = new(name, possibility);

				for (int i = 0; i < _list.Count; i++)
				{
					if (_list[i] > possibility)
					{
						_list.Insert(i, option);
						return;
					}
				}

				_list.Add(option);
			}
		}

		/// <summary>
		/// Removes all options.
		/// </summary>
		public void Clear()
		{
			_list.Clear();
			_names.Clear();
			Remaining = _max;
		}

		/// <summary>
		/// Returns a randomly picked <see cref="NamedOption"/>.
		/// </summary>
		public NamedOption Determine()
		{
			return Determine(out _);
		}

		/// <summary>
		/// Returns a randomly picked <see cref="NamedOption"/>.
		/// </summary>
		/// <param name="index">Index of the returned <see cref="NamedOption"/>.</param>
		public NamedOption Determine(out int index)
		{
			if (_list.Count == 0)
			{
				index = DEFAULT_OPTION_INDEX;
				return GetDefaultOption();
			}

			int n = Random.RandomNumber(0, _max);
			int length = _list.Count;

			int sum = 0;

			Span<NamedOption> span = CollectionsMarshal.AsSpan(_list);

			for (int i = 0; i < length; i++)
			{
				ref readonly NamedOption op = ref span[i];
				sum += op.Possibility;

				if (sum > n)
				{
					index = i;
					return op;
				}
			}

			index = DEFAULT_OPTION_INDEX;
			return GetDefaultOption();
		}

		/// <summary>
		/// Registers a new <see cref="NamedOption"/> with the specified <paramref name="name"/> and <see cref="NamedOption.Possibility"/> equal to <see cref="Remaining"/>.
		/// </summary>
		/// <param name="name">Name of <see cref="NamedOption"/> to register.</param>
		/// <remarks>The option is not added if <see cref="Remaining"/> is equal to <c>0</c>.</remarks>
		public OptionalPossibility FillRemaining(string name)
		{
			if (HasDefaultOption)
			{
				return AddOption(name, Remaining);
			}

			return this;
		}

		/// <summary>
		/// Returns an <see cref="NamedOption"/> with <see cref="NamedOption.Name"/> set to <see cref="DefaultOptionName"/> and <see cref="NamedOption.Possibility"/> equal to <see cref="Remaining"/>.
		/// </summary>
		public NamedOption GetDefaultOption()
		{
			return new NamedOption(DefaultOptionName, Remaining);
		}

		/// <inheritdoc/>
		public IEnumerator<NamedOption> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// Returns an <see cref="NamedOption"/> with the specified <paramref name="name"/>.
		/// </summary>
		/// <remarks>If <paramref name="name"/> is equal to <see cref="DefaultOptionName"/>, result of calling <see cref="GetDefaultOption"/> is returned.</remarks>
		/// <param name="name">Name of <see cref="NamedOption"/> to return.</param>
		/// <exception cref="ArgumentException">
		/// <paramref name="name"/> is <see langword="null"/> or empty. -or-
		/// Option with the specified <paramref name="name"/> does not exist.
		/// </exception>
		public NamedOption GetOption(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw Error.NullOrEmpty(nameof(name));
			}

			if (name == DefaultOptionName)
			{
				return GetDefaultOption();
			}

			Span<NamedOption> span = CollectionsMarshal.AsSpan(_list);

			for (int i = 0; i < _list.Count; i++)
			{
				ref readonly NamedOption op = ref span[i];

				if (op.Name == name)
				{
					return op;
				}
			}

			throw Error.Arg($"Option with name '{name}' does not exist", nameof(name));
		}

		/// <summary>
		/// Returns the <see cref="NamedOption"/> at the specified <paramref name="index"/>.
		/// </summary>
		/// <remarks>If index is equal to <c>-1</c>, the result of calling <see cref="GetDefaultOption()"/> is returned.</remarks>
		/// <param name="index">Index to get the <see cref="NamedOption"/> at.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> must be greater than or equal to <c>-1</c>. -or-
		/// <paramref name="index"/> must be less than <see cref="Count"/>.
		/// </exception>
		public NamedOption GetOption(int index)
		{
			if (index < 0)
			{
				if (index == DEFAULT_OPTION_INDEX)
				{
					return GetDefaultOption();
				}

				throw Error.MustBeGreaterThanOrEqualTo(nameof(index), -1);
			}

			if (index >= Count)
			{
				throw Error.MustBeLessThan(nameof(index), nameof(Count));
			}

			return _list[index];
		}

		/// <summary>
		/// Returns an array of all non-default <see cref="NamedOption"/>s sorted from lowest to greatest.
		/// </summary>
		/// <remarks>To access the default <see cref="NamedOption"/>, use <see cref="GetDefaultOption()"/> instead.</remarks>
		public NamedOption[] GetOptions()
		{
			return _list.ToArray();
		}

		/// <summary>
		/// Removes all options and resets all properties to their default values.
		/// </summary>
		public void Reset()
		{
			_max = DefaultMax;
			Clear();
			_divisor = DefaultDivisor;
			_step = 0;
			Capacity = DefaultCapacity;
			_defaultOptionName = DefaultOption;
		}

		/// <summary>
		/// Multiplies <see cref="Max"/>, <see cref="Remaining"/> and all possibilities by the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="value">Value to scale the possibilities by.</param>
		/// <param name="includeStep">Determines whether to scale the <see cref="Step"/> or <see cref="Divisor"/> as well.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> must be greater than 0.</exception>
		public OptionalPossibility Scale(int value, bool includeStep = true)
		{
			if (value < 0)
			{
				throw Error.MustBeGreaterThan(nameof(value), 0);
			}

			_max *= value;
			Remaining *= value;

			Span<NamedOption> span = CollectionsMarshal.AsSpan(_list);

			for (int i = 0; i < _list.Count; i++)
			{
				span[i] *= value;
			}

			if (includeStep)
			{
				GetDivisorOrStep() *= value;
			}

			return this;
		}

		bool IPossibility.Determine()
		{
			Determine(out int index);

			return index > _step;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		[DebuggerStepThrough]
		private static ArgumentException Exc_NameAlreadyExists(string name, [CallerArgumentExpression("name")] string? paramName = default)
		{
			return new ArgumentException($"Option with name '{name}' already exists", paramName);
		}

		private void CalculateStep()
		{
			if (_divisor != STEP_OR_DIVIDER_SWITCH)
			{
				_step = Count / _divisor;
			}
		}

		private ref int GetDivisorOrStep()
		{
			if (_divisor == STEP_OR_DIVIDER_SWITCH)
			{
				return ref _step;
			}

			return ref _divisor;
		}
	}
}
