using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.Collections.Context
{
	/// <summary>
	/// ElementContext
	/// </summary>
	public struct ElementContext<T, C> : IElementContext<T, C>
	{
		private readonly T _element;
		private readonly C _context;

		#region Constructors

		public ElementContext(T element, C context)
		{
			_element = element;
			_context = context;
		}

		#endregion
		#region Properties

		public T Element
		{
			get
			{
				return _element;
			}
		}

		public C Context
		{
			get
			{
				return _context;
			}
		}

		#endregion
	}

  public static class ElementContext
  {
    public static ElementContext<T, C> Create<T, C>(T element, C context)
    {
      return new ElementContext<T, C>(element, context);
    }
  }
}
