using System;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a link that was used to access the current resource.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record SelfLinkModel
	{
		/// <summary>
		/// Link that was used to access the current resource.
		/// </summary>
		[JsonProperty("self", Required = Required.Always)]
		public LinkModel Self { get; }

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
				throw new ArgumentNullException(nameof(self));
			}

			Self = self;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SelfLinkModel"/> class with a <paramref name="self"/> link specified.
		/// </summary>
		/// <param name="self">Link that was used to access the current resource.</param>
		/// <exception cref="ArgumentException"><paramref name="self"/> is null or empty.</exception>
		public SelfLinkModel(string self)
		{
			Self = new LinkModel(self);
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return Self.ToString();
		}
	}
}
