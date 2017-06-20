using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ArrangementWithRepeats : Combinatorial<int>
	{
		private int[] _elements;

		#region Constructors

		public ArrangementWithRepeats(int cardinal, int variation)
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
			return Combinatorial.ArrangementWithRepeatsFirst(_elements);
		}

		public override bool Next()
		{
			return Combinatorial.ArrangementWithRepeatsNext(_elements, Cardinal);
		}

		#endregion
	}
}
