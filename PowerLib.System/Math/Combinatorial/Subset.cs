using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Subset : Combinatorial<bool>
	{
		private bool[] _elements;

		#region Constructors

		public Subset(int cardinal)
			: base(cardinal)
		{
			_elements = new bool[cardinal];
		}

    #endregion
    #region Methods

    public override IEnumerator<bool> GetEnumerator()
    {
      for (int i = 0; i < _elements.Length; i++)
        yield return _elements[i];
    }

    public override bool First()
		{
			return Combinatorial.SubsetFirst(_elements);
		}

		public override bool Next()
		{
			return Combinatorial.SubsetNext(_elements);
		}

		#endregion
	}
}
