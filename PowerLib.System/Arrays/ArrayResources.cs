using System;
using System.Globalization;
using PowerLib.System.Resources;

namespace PowerLib.System
{
  internal sealed class ArrayResources : EnumTypeResourceAccessor<ArrayMessage, ArrayResources>
  {
    private static Lazy<ArrayResources> _default = new Lazy<ArrayResources>(() => new ArrayResources());

    private ArrayResources()
      : base(t => t.ToString())
    {
    }

    internal ArrayResources(CultureInfo ci)
      : base(t => t.ToString(), ci)
    {
    }

    public static ArrayResources Default
    {
      get { return _default.Value; }
    }
  }

  internal enum ArrayMessage
  {
    IndexOpenBracket,
    IndexCloseBracket,
    IndexItemDelimiter,
    IndexItemFormat,
    IndexLevelDelimiter,
    ArrayIsEmpty,
    InvalidArrayRank,
    InvalidArrayLength,
    InvalidArrayDimLength,
    InvalidArrayDimBase,
    InvalidArrayElementType,
    InvalidArrayElement,

    OneOrMoreInvalidArrayElements,
    OneOrMoreArrayElementsOutOfRange,

    TypeIsNotArray,
    ArrayIsNotJagged,
    ArrayElementOutOfRange,
    ArrayIndexOutOfRange,
    ArrayDimIndicesOutOfRange,

    ArrayElementIndexOutOfRange,
    RegularArrayElementIndexOutOfRange,
    JaggedArrayElementIndexOutOfRange,
    JaggedRegularArrayElementIndexOutOfRange,

    JaggedArrayElementIndexLevelNotSpecified,
  }
}
