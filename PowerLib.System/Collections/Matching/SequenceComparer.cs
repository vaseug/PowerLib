using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerLib.System.Linq;

namespace PowerLib.System.Collections.Matching
{
	public sealed class SequenceComparer : IComparer<IEnumerable>
	{
		private IComparer _сomparer;

		#region Constructors

		public SequenceComparer(IComparer сomparer)
		{
			if (сomparer == null)
				throw new ArgumentNullException("сomparer");

			_сomparer = сomparer;
		}

    #endregion
    #region Methods

    public int Compare(IEnumerable first, IEnumerable second)
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
					int result;
					for (bool ff = fe.MoveNext(), sf = se.MoveNext();
						(result = ff && !sf ? 1 : !ff && sf ? -1 : !ff && !sf ? 0 : _сomparer.Compare(fe.Current, se.Current)) == 0 && ff && sf;
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

		#endregion
	}
}
