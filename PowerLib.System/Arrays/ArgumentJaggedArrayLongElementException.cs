using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace PowerLib.System
{
  /// <summary>
  /// ArgumentJaggedArrayLongElementException exception
  /// </summary>
  public class ArgumentJaggedArrayLongElementException : ArgumentException
  {
    private static readonly string DefaultMessage;
    private long[][] _indices;

    #region Constructors

    static ArgumentJaggedArrayLongElementException()
    {
      DefaultMessage = ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement];
    }

    public ArgumentJaggedArrayLongElementException(params long[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsLongJaggedIndices(indices)))
    {
      _indices = (long[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayLongElementException(string paramName, params long[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsLongJaggedIndices(indices)), paramName)
    {
      _indices = (long[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayLongElementException(string paramName, Exception innerException, params long[][] indices)
      : base(string.Format(DefaultMessage, PwrArray.FormatAsLongJaggedIndices(indices)), paramName, innerException)
    {
      _indices = (long[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayLongElementException(string paramName, string message, params long[][] indices)
      : base(string.Format(message, PwrArray.FormatAsLongJaggedIndices(indices)), paramName)
    {
      _indices = (long[][])indices.CloneAsJagged();
    }

    public ArgumentJaggedArrayLongElementException(string paramName, string message, Exception innerException, params long[][] indices)
      : base(string.Format(message, PwrArray.FormatAsLongJaggedIndices(indices)), paramName, innerException)
    {
      _indices = (long[][])indices.CloneAsJagged();
    }

    protected ArgumentJaggedArrayLongElementException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      if (info == null)
        throw new ArgumentNullException("info");

      _indices = (long[][])info.GetValue("Indices", typeof(long[][]));
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

    public IReadOnlyList<long> GetIndices(int level)
    {
      if (level < 0 || level >= _indices.Length)
        throw new ArgumentOutOfRangeException("level");
      //
      return _indices[level] != null ? new ReadOnlyCollection<long>(_indices[level]) : null;
    }

    #endregion
  }
}
