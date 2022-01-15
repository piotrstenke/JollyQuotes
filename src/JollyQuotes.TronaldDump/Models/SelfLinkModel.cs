using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a link that was used to access the current resource.
	/// </summary>
	[JsonObject]
	public sealed record SelfLinkModel
	{
		private readonly LinkModel _self;

		/// <summary>
		/// Link that was used to access the current resource.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("self", Order = 0, Required = Required.Always)]
		public LinkModel Self
		{
			get => _self;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_self = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelfLinkModel"/> class with a <paramref name="self"/> link model specified.
		/// </summary>
		/// <param name="self">Link that was used to access the current resource.</param>
		/// <exception cref="ArgumentNullException"><paramref name="self"/> is <see langword="null"/>.</exception>
		[JsonConstructor]
		public SelfLinkModel(LinkModel self)
		{
			if (self is null)
			{
				throw Error.Null(nameof(self));
			}

			_self = self;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelfLinkModel"/> class with a <paramref name="self"/> link specified.
		/// </summary>
		/// <param name="self">Link that was used to access the current resource.</param>
		/// <exception cref="ArgumentException"><paramref name="self"/> is null or empty.</exception>
		public SelfLinkModel(string self)
		{
			_self = new LinkModel(self);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Self.ToString();
		}
	}
}
