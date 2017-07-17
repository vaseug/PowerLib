using System;
using System.Collections.Generic;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.Collections.Context
{
  /// <summary>
  /// HierarchicalContext
  /// </summary>
  public class HierarchicalContext<T> : IHierarchicalContext<T>
  {
    private readonly IList<T> _ancestors;
    private readonly PwrList<T> _innerList;

    #region Constructors

    public HierarchicalContext()
    {
      _innerList = new PwrList<T>();
      _ancestors = new PwrListView<T>(_innerList);
    }

    #endregion
    #region Properties

    public IList<T> Ancestors
    {
      get
      {
        return _ancestors;
      }
    }

    #endregion
    #region Methods

    public int PushAncestor(T ancestor)
    {
      return _innerList.PushFirst(ancestor);
    }

    public T PopAncestor()
    {
      return _innerList.PopFirst();
    }

    #endregion
  }
}
