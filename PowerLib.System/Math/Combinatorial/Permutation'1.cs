using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of set item.</typeparam>
	public sealed class Permutation<T> : Combinatorial<T>
	{
		private Permutation _enumerator;
		private T[] _elements;

		#region Constructors

		public Permutation(T[] elements)
			: base(elements == null ? 0 : elements.Length)
		{
			if (elements == null)
				throw new ArgumentNullException("elements");

			_enumerator = new Permutation(elements.Length);
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
