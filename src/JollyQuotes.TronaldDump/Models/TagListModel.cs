using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JollyQuotes.TronaldDump.Models
{
	/// <summary>
	/// Represents a collection of tags.
	/// </summary>
	[Serializable]
	[JsonObject]
	public sealed record TagListModel : IEnumerable<TagModel>
	{
		private readonly TagModel[] _tags;

		/// <summary>
		/// An array of underlaying tags.
		/// </summary>
		/// <exception cref="ArgumentNullException">Value is <see langword="null"/>.</exception>
		[JsonProperty("tag", Required = Required.Always)]
		public TagModel[] Tags
		{
			get => _tags;
			init
			{
				if (value is null)
				{
					throw Error.Null(nameof(value));
				}

				_tags = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TagListModel"/> class with an array of underlaying <paramref name="tags"/> specified.
		/// </summary>
		/// <param name="tags">An array of underlaying tags.</param>
		/// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.</exception>
		[JsonConstructor]
		public TagListModel(TagModel[] tags)
		{
			if (tags is null)
			{
				throw Error.Null(nameof(tags));
			}

			_tags = tags;
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

		/// <inheritdoc/>
		public IEnumerator<TagModel> GetEnumerator()
		{
			return ((IEnumerable<TagModel>)_tags).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _tags.GetEnumerator();
		}
	}
}
