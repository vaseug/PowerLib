using System;
using System.Collections;
using System.Collections.Generic;

namespace PowerLib.System.Math.Combinatorial
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class Combinatorial<T> : IEnumerable<T>
	{
		private int _cardinal;
		private int _variation;

		#region Constructors

		protected Combinatorial(int cardinal)
			: this(cardinal, cardinal)
		{
		}

		protected Combinatorial(int cardinal, int variation)
		{
			if (cardinal < 0)
				throw new ArgumentOutOfRangeException("cardinal");
			if (variation < 0 || variation > cardinal)
				throw new ArgumentOutOfRangeException("variance");

			_cardinal = cardinal;
			_variation = variation;
		}

		#endregion
		#region Properties

		public int Cardinal
		{
			get
			{
				return _cardinal;
			}
		}

		public int Variation
		{
			get
			{
				return _variation;
			}
		}

    #endregion
    #region Methods

    public abstract IEnumerator<T> GetEnumerator();

		public abstract bool First();

		public abstract bool Next();

    #endregion
    #region IEnumerable<T> implementation

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
    #region IEnumerable implementation

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion
  }
}
