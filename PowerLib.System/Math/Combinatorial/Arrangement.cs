using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Arrangement : Combinatorial<int>
	{
		private int[] _elements;

		#region Constructors

		public Arrangement(int cardinal, int variation)
			: base(cardinal, variation)
		{
			_elements = new int[variation];
		}

		#endregion
    #region Methods

    public override IEnumerator<int> GetEnumerator()
    {
      for (int i = 0; i < _elements.Length; i++)
        yield return _elements[i];
    }

    public override bool First()
		{
			return Combinatorial.ArrangementFirst(_elements);
		}

		public override bool Next()
		{
			return Combinatorial.ArrangementNext(_elements, Cardinal);
		}

		#endregion
	}
}
