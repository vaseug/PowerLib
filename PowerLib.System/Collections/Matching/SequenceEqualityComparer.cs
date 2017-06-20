using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SequenceEqualityComparer : IEqualityComparer<IEnumerable>
	{
		private IEqualityComparer _equalityComparer;
		private Func<int, int, int> _hashComposer;
		private int _hashSeed;

		#region Constructors

		public SequenceEqualityComparer(IEqualityComparer equalityComparer)
			: this(equalityComparer, (a, c) => unchecked(a * 31 + c), 23)
		{
		}

		public SequenceEqualityComparer(IEqualityComparer equalityComparer, Func<int, int, int> hashComposer, int hashSeed)
		{
			if (equalityComparer == null)
				throw new ArgumentNullException("equalityComparer");
			if (hashComposer == null)
				throw new ArgumentNullException("hashComposer");

			_equalityComparer = equalityComparer;
			_hashComposer = hashComposer;
			_hashSeed = hashSeed;
		}

    #endregion
    #region Methods

    public bool Equals(IEnumerable first, IEnumerable second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");

			IEnumerator fe = first.GetEnumerator();
			try
			{
				IEnumerator se = second.GetEnumerator();
				try
				{
					bool result;
					for (bool ff = fe.MoveNext(), sf = se.MoveNext();
						(result = !ff && !sf || ff && sf && _equalityComparer.Equals(fe.Current, se.Current)) && ff && sf;
						ff = fe.MoveNext(), sf = se.MoveNext());
					return result;
				}
				finally
				{
					IDisposable disposable = se as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}
			}
			finally
			{
				IDisposable disposable = fe as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}
		}

		public int GetHashCode(IEnumerable sequence)
		{
			if (sequence == null)
				throw new ArgumentNullException("sequence");

			int hashCode = _hashSeed;
			foreach (object item in sequence)
				hashCode = _hashComposer(hashCode, (item == null ? 0 : item.GetHashCode()));
			return hashCode;
		}

		#endregion
	}
}
