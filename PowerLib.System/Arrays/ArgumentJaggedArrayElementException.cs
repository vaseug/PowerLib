using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PowerLib.System
{
  /// <summary>
  /// ArgumentJaggedArrayElementException exception
  /// </summary>
  public class ArgumentJaggedArrayElementException : ArgumentException
  {
    private static readonly string DefaultMessage;
    private int[][] _indices;

    #region Constructors

    static ArgumentJaggedArrayElementException()
    {
      DefaultMessage = ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement];
    }

    public ArgumentJaggedArrayElementException(params int[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsJaggedIndices(indices)))
    {
      _indices = (int[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayElementException(string paramName, params int[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsJaggedIndices(indices)), paramName)
    {
      _indices = (int[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayElementException(string paramName, Exception innerException, params int[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsJaggedIndices(indices)), paramName, innerException)
    {
      _indices = (int[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayElementException(string paramName, string message, params int[][] indices)
      : base(string.Format(message, PwrArray.FormatAsJaggedIndices(indices)), paramName)
    {
      _indices = (int[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayElementException(string paramName, string message, Exception innerException, params int[][] indices)
      : base(string.Format(message, PwrArray.FormatAsJaggedIndices(indices)), paramName, innerException)
    {
      _indices = (int[][])indices.CloneAsJagged();
    }

    protected ArgumentJaggedArrayElementException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      _indices = (int[][])info.GetValue("Indices", typeof(int[][]));
    }

    #endregion
    #region Properties

    public int Depth
    {
      get
      {
        return _indices.Length;
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

    public int GetRank(int level)
    {
      if (level < 0 || level >= _indices.Length)
        throw new ArgumentOutOfRangeException("level");
      //
      return _indices[level] != null ? _indices[level].Length : 0;
    }

    public IReadOnlyList<int> GetIndices(int level)
    {
      if (level < 0 || level >= _indices.Length)
        throw new ArgumentOutOfRangeException("level");
      //
      return _indices[level] != null ? new ReadOnlyCollection<int>(_indices[level]) : null;
    }

    #endregion
  }
}
