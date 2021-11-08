using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of tags.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TagListModel
	{
		/// <summary>
		/// An array of underlaying tags.
		/// </summary>
		[JsonProperty("tag", Required = Required.Always)]
		public TagModel[] Tags { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TagListModel"/> class with an array of underlaying <paramref name="tags"/> specified.
		/// </summary>
		/// <param name="tags">An array of underlaying tags.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException"><paramref name="tags"/> is empty.</exception>
		[JsonConstructor]
		public TagListModel(TagModel[] tags)
		{
			if (tags is null)
			{
				throw Error.Null(nameof(tags));
			}

			if (tags.Length == 0)
			{
				throw Error.Empty(nameof(tags));
			}

			Tags = tags;
		}

		/// <inheritdoc/>
		public bool Equals(TagListModel? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return
				other.Tags.Length == Tags.Length &&
				other.Tags.SequenceEqual(Tags);
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			HashCode hash = new();

			foreach (TagModel tag in Tags)
			{
				hash.Add(tag);
			}

			return hash.ToHashCode();
		}

#pragma warning disable IDE0051 // Remove unused private members
		private bool PrintMembers(StringBuilder builder)
#pragma warning restore IDE0051 // Remove unused private members
		{
			Internals.PrintArray(builder, nameof(Tags), Tags);
			return true;
		}
	}
}
