using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Type of set item.</typeparam>
	public sealed class Subset<T> : Combinatorial<T>
	{
		private Subset _enumerator;
		private T[] _elements;

		#region Constructors

		public Subset(T[] elements)
			: base(elements == null ? 0 : elements.Length)
		{
			if (elements == null)
				throw new ArgumentNullException("elements");

			_enumerator = new Subset(elements.Length);
			_elements = (T[])elements.Clone();
		}

		#endregion
		#region Methods

    public override IEnumerator<T> GetEnumerator()
    {
      using (IEnumerator<bool> e = _enumerator.GetEnumerator())
        for (int i = 0; e.MoveNext(); i++)
          if (e.Current)
            yield return _elements[i];
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
