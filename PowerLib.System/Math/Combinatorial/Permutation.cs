using System;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class Permutation : Combinatorial<int>
	{
		private int[] _elements;

		#region Constructors

		public Permutation(int cardinal)
			: base(cardinal)
		{
			_elements = new int[cardinal];
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
			return Combinatorial.PermutationFirst(_elements);
		}

		public override bool Next()
		{
			return Combinatorial.PermutationNext(_elements);
		}

		#endregion
	}
}
