using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PowerLib.System
{
  /// <summary>
  /// ArgumentRegularArrayElementException exception
  /// </summary>
  public class ArgumentRegularArrayElementException : ArgumentException
  {
    private static readonly string DefaultMessage;
    private int[] _indices;
    private IReadOnlyList<int> _indicesAccessor;

    #region Constructors

    static ArgumentRegularArrayElementException()
    {
      DefaultMessage = ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement];
    }

    public ArgumentRegularArrayElementException(params int[] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(indices)))
    {
      _indices = (int[])indices.Clone();
    }

    public ArgumentRegularArrayElementException(int index)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(new int[] { index })))
    {
      _indices = new int[] { index };
    }

    public ArgumentRegularArrayElementException(string paramName, params int[] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(indices)), paramName)
    {
      _indices = (int[])indices.Clone();
    }

    public ArgumentRegularArrayElementException(string paramName, int index)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(new int[] { index })), paramName)
    {
      _indices = new int[] { index };
    }

    public ArgumentRegularArrayElementException(string paramName, Exception innerException, params int[] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(indices)), paramName, innerException)
    {
      _indices = (int[])indices.Clone();
    }

    public ArgumentRegularArrayElementException(string paramName, Exception innerException, int index)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsRegularIndices(new int[] { index })), paramName, innerException)
    {
      _indices = new int[] { index };
    }

    public ArgumentRegularArrayElementException(string paramName, string message, params int[] indices)
      : base(string.Format(message, PwrArray.FormatAsRegularIndices(indices)), paramName)
    {
      _indices = (int[])indices.Clone();
    }

    public ArgumentRegularArrayElementException(string paramName, string message, int index)
      : base(string.Format(message, PwrArray.FormatAsRegularIndices(new int[] { index })), paramName)
    {
      _indices = new int[] { index };
    }

    public ArgumentRegularArrayElementException(string paramName, string message, Exception innerException, params int[] indices)
      : base(string.Format(message, PwrArray.FormatAsRegularIndices(indices)), paramName, innerException)
    {
      _indices = (int[])indices.Clone();
    }

    public ArgumentRegularArrayElementException(string paramName, string message, Exception innerException, int index)
      : base(string.Format(message, PwrArray.FormatAsRegularIndices(new int[] { index })), paramName, innerException)
    {
      _indices = new int[] { index };
    }

    protected ArgumentRegularArrayElementException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      _indices = (int[])info.GetValue("Indices", typeof(int[]));
    }

    #endregion
    #region Properties

    /// <summary>
    /// Array elemenindices
    /// </summary>
    public IReadOnlyList<int> Indices
    {
      get
      {
        if (_indicesAccessor == null)
          _indicesAccessor = new ReadOnlyCollection<int>(_indices);
        return _indicesAccessor;
      }
    }

    #endregion
    #region Methods

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      base.GetObjectData(info, context);
      info.AddValue("Indices", _indices);
    }

    #endregion
  }
}
