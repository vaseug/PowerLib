using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of set item.</typeparam>
	public sealed class ArrangementWithRepeats<T> : Combinatorial<T>
	{
		private ArrangementWithRepeats _enumerator;
		private T[] _elements;

		#region Constructors

		public ArrangementWithRepeats(T[] elements, int variation)
			: base(elements == null ? 0 : elements.Length, variation)
		{
			if (elements == null)
				throw new ArgumentNullException("elements");
			//
			_enumerator = new ArrangementWithRepeats(elements.Length, variation);
			_elements = (T[])elements.Clone();
		}

		#endregion
		#region Methods

    public override IEnumerator<T> GetEnumerator()
    {
      using (IEnumerator<int> e = _enumerator.GetEnumerator())
        while (e.MoveNext())
          yield return _elements[e.Current];
    }

    public override bool First()
		{
			return _enumerator.First();
		}

		public override bool Next()
		{
			return _enumerator.Next();
		}

		#endregion
	}
}
