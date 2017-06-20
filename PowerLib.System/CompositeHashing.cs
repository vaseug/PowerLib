using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerLib.System
{
	public sealed class CompositeHashing
	{
		private Func<dynamic, int> _hasher;
		private Func<int, int, int> _composer;
		private int _seed;
		private static readonly Lazy<CompositeHashing> _default = new Lazy<CompositeHashing>(() => new CompositeHashing((a, c) => unchecked(a * 31 + c), 23));

		#region Constructors

		private CompositeHashing()
		{
		}

		public CompositeHashing(Func<int, int, int> composer, int seed)
			: this(null, composer, seed)
		{
		}

		public CompositeHashing(Func<dynamic, int> hasher, Func<int, int, int> composer, int seed)
		{
			if (composer == null)
				throw new ArgumentNullException("composer");

			_hasher = hasher;
			_composer = composer;
			_seed = seed;
		}

		#endregion
		#region Properties

		public static CompositeHashing Default
		{
			get { return _default.Value; }
		}

		#endregion
		#region Methods

		public int GetHashCode(params object[] objects)
		{
			return objects
				.Select(item => _hasher != null ? _hasher(item) : item == null ? 0 : item.GetHashCode())
				.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		public int GetHashCode(IEnumerable<object> objects)
		{
			return objects
				.Select(item => _hasher != null ? _hasher(item) : item == null ? 0 : item.GetHashCode())
				.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		public int GetHashCode<T>(params T[] objects)
		{
			return objects
				.Select(item => _hasher != null ? _hasher(item) : item == null ? 0 : item.GetHashCode())
				.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		public int GetHashCode<T>(IEnumerable<T> objects)
		{
			return objects
				.Select(item => _hasher != null ? _hasher(item) : item == null ? 0 : item.GetHashCode())
				.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		public int ComposeHashCodes(params int[] hashCodes)
		{
			return hashCodes.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		public int ComposeHashCodes(IEnumerable<int> hashCodes)
		{
			return hashCodes.Aggregate(_seed, (accumHash, itemHash) => _composer(accumHash, itemHash));
		}

		#endregion
	}
}
