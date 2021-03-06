using System;
using System.Linq;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  /// <summary>
  /// .
  /// </summary>
  public sealed class JaggedArrayLongInfo : ArrayLongInfo
  {
    private int _rank = 0;
    private long _length = 0;
    private int[] _ranks;
    private ArrayInfoNode _node;
    private Accessor<int, int> _rankAccessor;
    private ParamsAccessor<long, RegularArrayLongInfo> _dimArrayInfosAccessor;
    private ParamsAccessor<long, RegularArrayLongInfo> _zeroBasedDimArrayInfosAccessor;
    private ParamsAccessor<long[], RegularArrayLongInfo> _rankedDimArrayInfosAccessor;
    private ParamsAccessor<long[], RegularArrayLongInfo> _zeroBasedRankedDimArrayInfosAccessor;

    #region Constructors

    public JaggedArrayLongInfo(Array array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      var ranks = new PwrList<int>();
      var biases = new PwrList<int>();
      var type = array.GetType();
      for (; type.IsArray; type = type.GetElementType())
      {
        int rank = type.GetArrayRank();
        ranks.Add(rank);
        biases.Add(_rank);
        _rank += rank;
      }
      _ranks = ranks.ToArray();

      int depth = 0;
      var arrayContexts = new PwrStack<ArrayInfoNodeContext>();
      var arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions());
      var arrayIndex = new ArrayLongIndex(arrayInfo);
      var node = new ArrayInfoNode(arrayInfo, _length, depth == _ranks.Length - 1);
      descent:
      while (depth < _ranks.Length - 1)
      {
        if (array.Length > 0)
          for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
        if (arrayIndex.Carry != 0 || array.Length == 0)
          break;
        depth++;
        arrayContexts.Push(new ArrayInfoNodeContext(node, array, arrayIndex));
        array = arrayIndex.GetValue<Array>(array);
        arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions());
        arrayIndex.SetValue<ArrayInfoNode>(node.Nodes, new ArrayInfoNode(arrayInfo, _length, depth == _ranks.Length - 1));
        if (depth == _ranks.Length - 1)
          _length += array.Length;
        else
        {
          node = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes);
          arrayIndex = new ArrayLongIndex(arrayInfo);
        }
      }
      ascent:
      if (depth != 0)
      {
        var nodeContext = arrayContexts.Pop();
        array = nodeContext.Array;
        node = nodeContext.Node;
        arrayIndex = nodeContext.Index;
        depth--;
        if (arrayIndex.IsMax)
          goto ascent;
        else
        {
          arrayIndex++;
          goto descent;
        }
      }
      _node = node;
    }

    public JaggedArrayLongInfo(int[] ranks, Func<int, long[], long[][], long[]> lensGetter, long[] flatIndices, long[][] dimIndices)
      : this(ranks, lensGetter != null ?
        (depth, rankedFlatIndices, rankedDimindices) => lensGetter(depth, rankedFlatIndices, rankedDimindices).Select<long, ArrayLongDimension>(length => new ArrayLongDimension(length)).ToArray() :
        default(Func<int, long[], long[][], ArrayLongDimension[]>), flatIndices, dimIndices)
    {
    }

    public JaggedArrayLongInfo(int[] ranks, Func<int, long[], long[][], ArrayLongDimension[]> dimsGetter, long[] flatIndices, long[][] dimIndices)
    {
      if (ranks == null)
        throw new ArgumentNullException("ranks");
      if (dimsGetter == null)
        throw new ArgumentNullException("dimsGetter");

      int rank = 0;
      for (int i = 0; i < ranks.Length; i++)
      {
        if (ranks[i] < 0)
          throw new ArgumentCollectionElementException("ranks", ArrayResources.Default.Strings[ArrayMessage.ArrayElementOutOfRange], i);

        if (dimIndices != null)
        {
          if (dimIndices[i] == null)
            throw new ArgumentCollectionElementException("rankedIndices", "Argument has NULL value.", i);
          if (dimIndices[i].Length != ranks[i])
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "rankedIndices");
        }
        rank += ranks[i];
      }
      if (flatIndices != null && flatIndices.Length != ranks.Length)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "flatIndices");

      _rank = rank;
      _ranks = (int[])ranks.Clone();
      int depth = 0;
      var arrayContexts = new PwrStack<ArrayInfoNodeContext>();
      var arrayInfo = new RegularArrayLongInfo(dimsGetter(depth, flatIndices, dimIndices));
      var arrayIndex = new ArrayLongIndex(arrayInfo);
      var node = new ArrayInfoNode(arrayInfo, _length, depth == _ranks.Length - 1);
      descent:
      while (depth < _ranks.Length - 1)
      {
        ArrayLongDimension[] arrayDims = null;
        if (arrayInfo.Length > 0)
          for (flatIndices[depth] = arrayIndex.FlatIndex, arrayIndex.GetDimIndices(dimIndices[depth]);
            arrayIndex.Carry == 0 && (arrayDims = dimsGetter(depth + 1, flatIndices, dimIndices)) == null;
            arrayIndex++, flatIndices[depth] = arrayIndex.FlatIndex, arrayIndex.GetDimIndices(dimIndices[depth])) ;
        if (arrayIndex.Carry != 0 || arrayInfo.Length == 0)
          break;
        depth++;
        arrayContexts.Push(new ArrayInfoNodeContext(node, null, arrayIndex));
        arrayInfo = new RegularArrayLongInfo(arrayDims);
        arrayIndex.SetValue<ArrayInfoNode>(node.Nodes, new ArrayInfoNode(arrayInfo, _length, depth == _ranks.Length - 1));
        if (depth == _ranks.Length - 1)
          _length += arrayInfo.Length;
        else
        {
          node = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes);
          arrayIndex = new ArrayLongIndex(arrayInfo);
        }
      }
      ascent:
      if (depth != 0)
      {
        var nodeContext = arrayContexts.Pop();
        node = nodeContext.Node;
        arrayIndex = nodeContext.Index;
        depth--;
        if (arrayIndex.IsMax)
          goto ascent;
        else
        {
          arrayIndex++;
          goto descent;
        }
      }
      _node = node;
    }

    #endregion
    #region Instance properties

    public override int Rank
    {
      get
      {
        return _rank;
      }
    }

    public override long Length
    {
      get
      {
        return _length;
      }
    }

    public int Depths
    {
      get
      {
        return _ranks.Length;
      }
    }

    public Accessor<int, int> Ranks
    {
      get
      {
        if (_rankAccessor == null)
          _rankAccessor = new Accessor<int, int>(dim => GetRank(dim));
        return null;
      }
    }

    public ParamsAccessor<long, RegularArrayLongInfo> DimArrayInfos
    {
      get
      {
        if (_dimArrayInfosAccessor == null)
          _dimArrayInfosAccessor = new ParamsAccessor<long, RegularArrayLongInfo>(dimIndices => GetDimArrayInfo(false, dimIndices));
        return _dimArrayInfosAccessor;
      }
    }

    public ParamsAccessor<long, RegularArrayLongInfo> ZeroBasedDimArrayInfos
    {
      get
      {
        if (_zeroBasedDimArrayInfosAccessor == null)
          _zeroBasedDimArrayInfosAccessor = new ParamsAccessor<long, RegularArrayLongInfo>(dimIndices => GetDimArrayInfo(true, dimIndices));
        return _zeroBasedDimArrayInfosAccessor;
      }
    }

    public ParamsAccessor<long[], RegularArrayLongInfo> RankedDimArrayInfos
    {
      get
      {
        if (_rankedDimArrayInfosAccessor == null)
          _rankedDimArrayInfosAccessor = new ParamsAccessor<long[], RegularArrayLongInfo>(dimIndices => GetDimArrayInfo(false, dimIndices));
        return _rankedDimArrayInfosAccessor;
      }
    }

    public ParamsAccessor<long[], RegularArrayLongInfo> ZeroBasedRankedDimArrayInfos
    {
      get
      {
        if (_zeroBasedRankedDimArrayInfosAccessor == null)
          _zeroBasedRankedDimArrayInfosAccessor = new ParamsAccessor<long[], RegularArrayLongInfo>(dimIndices => GetDimArrayInfo(true, dimIndices));
        return _zeroBasedRankedDimArrayInfosAccessor;
      }
    }

    #endregion
    #region Instance methods
    #region Internal methods

    private long CalcFlatIndexCore(bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null; i++)
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      return node.Total + node.ArrayInfo.CalcFlatIndex(zeroBased, indices[_ranks.Length - 1]);
    }

    private void CalcDimIndicesCore(long flatIndex, bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null; i++)
      {
        //
        ArrayLongIndex lowerIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        lowerIndex.SetMin();
        ArrayInfoNode lowerNode = null;
        for (; lowerIndex.Carry == 0 && (lowerNode = lowerIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; lowerIndex++) ;
        //
        ArrayLongIndex upperIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        upperIndex.SetMax();
        ArrayInfoNode upperNode = null;
        for (; upperIndex.Carry == 0 && (upperNode = upperIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; upperIndex--) ;
        //
        if (lowerNode == null && upperNode == null)
        {
          node = null;
          continue;
        }
        //
        while (flatIndex < upperNode.Total && upperIndex.FlatIndex - lowerIndex.FlatIndex > 1)
        {
          ArrayLongIndex middleIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased, FlatIndex = (lowerIndex.FlatIndex + upperIndex.FlatIndex) / 2 };
          ArrayInfoNode middleNode = middleIndex.GetValue<ArrayInfoNode>(node.Nodes);
          if (middleNode == null)
          {
            ArrayLongIndex searchIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
            searchIndex.SetFrom(middleIndex);
            for (searchIndex--; searchIndex.FlatIndex > lowerIndex.FlatIndex && (middleNode = searchIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; searchIndex--) ;
            if (middleNode == null)
            {
              searchIndex.SetFrom(middleIndex);
              for (searchIndex++; searchIndex.FlatIndex < upperIndex.FlatIndex && (middleNode = searchIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; searchIndex++) ;
              if (middleNode == null)
                break;
            }
            middleIndex.SetFrom(searchIndex);
          }
          //
          if (flatIndex < middleNode.Total)
          {
            upperIndex.SetFrom(middleIndex);
            upperIndex--;
            upperNode = upperIndex.GetValue<ArrayInfoNode>(node.Nodes);
          }
          else
          {
            lowerIndex.SetFrom(middleIndex);
            lowerNode = middleNode;
          }
        }
        //
        for (int j = 0; j < _ranks[i]; j++)
          indices[i][j] = flatIndex >= upperNode.Total ? upperIndex.DimIndices[j] : lowerIndex.DimIndices[j];
        node = flatIndex >= upperNode.Total ? upperNode : lowerNode;
      }
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      ArrayLongIndex arrayIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
      arrayIndex.FlatIndex = flatIndex - node.Total;
      for (int j = 0; j < _ranks[_ranks.Length - 1]; j++)
        indices[_ranks.Length - 1][j] = arrayIndex.DimIndices[j];
    }

    private void FirstDimIndicesCore(bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null; i++)
      {
        ArrayLongIndex thisIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        thisIndex.SetMin();
        ArrayInfoNode thisNode = null;
        for (; thisIndex.Carry == 0 && (thisNode = thisIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; thisIndex++) ;
        if (thisIndex.Carry != 0)
          node = null;
        else
        {
          //
          ArrayLongIndex nextIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
          nextIndex.SetFrom(thisIndex);
          ArrayInfoNode nextNode = null;
          for (nextIndex++; nextIndex.Carry == 0; nextIndex++)
          {
            nextNode = nextIndex.GetValue<ArrayInfoNode>(node.Nodes);
            if (nextNode == null)
              continue;
            if (nextNode.Total > thisNode.Total)
              break;
            thisNode = nextNode;
            thisIndex.SetFrom(nextIndex);
          }
          //
          thisIndex.GetDimIndices(indices[i]);
          node = thisIndex.GetValue<ArrayInfoNode>(node.Nodes);
        }
      }
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      ArrayLongIndex arrayIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
      arrayIndex.SetMin();
      arrayIndex.GetDimIndices(indices[_ranks.Length - 1]);
    }

    private void LastDimIndicesCore(bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null; i++)
      {
        ArrayLongIndex thisIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        thisIndex.SetMax();
        ArrayInfoNode thisNode = thisIndex.GetValue<ArrayInfoNode>(node.Nodes);
        for (; thisIndex.Carry == 0; thisIndex--)
        {
          thisNode = thisIndex.GetValue<ArrayInfoNode>(node.Nodes);
          if (thisNode == null)
            continue;
          if (thisNode.Total < _length)
            break;
        }
        if (thisIndex.Carry != 0)
          node = null;
        {
          //
          thisIndex.GetDimIndices(indices[i]);
          node = thisIndex.GetValue<ArrayInfoNode>(node.Nodes);
        }
      }
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      ArrayLongIndex arrayIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
      arrayIndex.SetMax();
      arrayIndex.GetDimIndices(indices[_ranks.Length - 1]);
    }

    private bool NextDimIndicesCore(bool zeroBased, long[][] indices)
    {
      PwrStack<ArrayInfoNodeContext> arrayContexts = new PwrStack<ArrayInfoNodeContext>();
      ArrayInfoNode node = _node;
      ArrayLongIndex arrayIndex = null;
      int depth = 0;
      for (; depth < _ranks.Length - 1 && node != null; depth++)
      {
        arrayIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        arrayIndex.SetDimIndices(indices[depth]);
        ArrayInfoNodeContext nodeContext = new ArrayInfoNodeContext(node, null, arrayIndex);
        node = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes);
        if (node != null)
          arrayContexts.Push(nodeContext);
      }
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      if (!node.ArrayInfo.IncDimIndices(zeroBased, indices[_ranks.Length - 1]))
        return false;
      long total = node.Total + node.ArrayInfo.Length;
      if (total == _length)
        return true;
      //
      descent:
      while (depth < _ranks.Length - 1)
      {
        ArrayInfoNode n = null;
        for (; arrayIndex.Carry == 0 && (n = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; arrayIndex++) ;
        if (n == null)
          break;
        arrayContexts.Push(new ArrayInfoNodeContext(node, null, arrayIndex));
        depth++;
        node = n;
        if (depth < _ranks.Length - 1)
        {
          arrayIndex = new ArrayLongIndex(node.ArrayInfo);
          arrayIndex.SetMin();
        }
        else if (node.Total == total && node.ArrayInfo.Length > 0)
        {
          node.ArrayInfo.GetMinDimIndices(zeroBased, indices[_ranks.Length - 1]);
          while (arrayContexts.Count > 0)
          {
            ArrayInfoNodeContext nodeContext = arrayContexts.Pop();
            nodeContext.Index.GetDimIndices(indices[arrayContexts.Count]);
          }
          return false;
        }
      }
      ascent:
      if (depth != 0)
      {
        ArrayInfoNodeContext nodeContext = arrayContexts.Pop();
        node = nodeContext.Node;
        arrayIndex = nodeContext.Index;
        depth--;
        if (arrayIndex.IsMax)
          goto ascent;
        else
        {
          arrayIndex++;
          goto descent;
        }
      }
      //
      return false;
    }

    private bool PrevDimIndicesCore(bool zeroBased, long[][] indices)
    {
      PwrStack<ArrayInfoNodeContext> arrayContexts = new PwrStack<ArrayInfoNodeContext>();
      ArrayInfoNode node = _node;
      ArrayLongIndex arrayIndex = null;
      int depth = 0;
      for (; depth < _ranks.Length - 1 && node != null; depth++)
      {
        arrayIndex = new ArrayLongIndex(node.ArrayInfo) { ZeroBased = zeroBased };
        arrayIndex.SetDimIndices(indices[depth]);
        ArrayInfoNodeContext nodeContext = new ArrayInfoNodeContext(node, null, arrayIndex);
        node = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes);
        if (node != null)
          arrayContexts.Push(nodeContext);
      }
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      if (!node.ArrayInfo.DecDimIndices(zeroBased, indices[_ranks.Length - 1]))
        return false;
      if (node.Total == 0)
        return true;
      long total = node.Total;
      //
      descent:
      while (depth < _ranks.Length - 1)
      {
        ArrayInfoNode n = null;
        for (; arrayIndex.Carry == 0 && (n = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes)) == null; arrayIndex--) ;
        if (n == null)
          break;
        arrayContexts.Push(new ArrayInfoNodeContext(node, null, arrayIndex));
        depth++;
        node = n;
        if (depth < _ranks.Length - 1)
        {
          arrayIndex = new ArrayLongIndex(node.ArrayInfo);
          arrayIndex.SetMax();
        }
        else if (node.Total == (total - node.ArrayInfo.Length) && node.ArrayInfo.Length > 0)
        {
          node.ArrayInfo.GetMaxDimIndices(zeroBased, indices[_ranks.Length - 1]);
          while (arrayContexts.Count > 0)
          {
            ArrayInfoNodeContext nodeContext = arrayContexts.Pop();
            nodeContext.Index.GetDimIndices(indices[arrayContexts.Count]);
          }
          return false;
        }
      }
      ascent:
      if (depth != 0)
      {
        ArrayInfoNodeContext nodeContext = arrayContexts.Pop();
        node = nodeContext.Node;
        arrayIndex = nodeContext.Index;
        depth--;
        if (arrayIndex.IsMin)
          goto ascent;
        else
        {
          arrayIndex--;
          goto descent;
        }
      }
      //
      return false;
    }

    private object GetValueCore(Array array, bool asRanges, bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null && array != null; i++)
      {
        array = node.ArrayInfo.GetValue<Array>(array, asRanges, zeroBased, indices[i]);
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      }
      //
      if (node == null || array == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      return node.ArrayInfo.GetValue(array, asRanges, zeroBased, indices[_ranks.Length - 1]);
    }

    private void SetValueCore(Array array, object value, bool asRanges, bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null && array != null; i++)
      {
        array = node.ArrayInfo.GetValue<Array>(array, asRanges, zeroBased, indices[i]);
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      }
      //
      if (node == null || array == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      node.ArrayInfo.SetValue(array, value, asRanges, zeroBased, indices[_ranks.Length - 1]);
    }

    private T GetValueCore<T>(Array array, bool asRanges, bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null && array != null; i++)
      {
        array = node.ArrayInfo.GetValue<Array>(array, asRanges, zeroBased, indices[i]);
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      }
      //
      if (node == null || array == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      return node.ArrayInfo.GetValue<T>(array, asRanges, zeroBased, indices[_ranks.Length - 1]);
    }

    private void SetValueCore<T>(Array array, T value, bool asRanges, bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < _ranks.Length - 1 && node != null && array != null; i++)
      {
        array = node.ArrayInfo.GetValue<Array>(array, asRanges, zeroBased, indices[i]);
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      }
      //
      if (node == null || array == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      node.ArrayInfo.SetValue<T>(array, value, asRanges, zeroBased, indices[_ranks.Length - 1]);
    }

    private RegularArrayLongInfo GetDimArrayInfoCore(bool zeroBased, long[][] indices)
    {
      ArrayInfoNode node = _node;
      for (int i = 0; i < indices.Length && node != null; i++)
        node = node.ArrayInfo.GetValue<ArrayInfoNode>(node.Nodes, false, zeroBased, indices[i]);
      //
      if (node == null)
        throw new InvalidOperationException("Invalid internal array index");
      //
      return node.ArrayInfo;
    }

    #endregion
    #region Public methods

    public int GetRank(int depth)
    {
      if (depth < 0 || depth >= _ranks.Length)
        throw new ArgumentOutOfRangeException("depth");
      //
      return _ranks[depth];
    }

    public override Array CreateArray(Type elementType)
    {
      if (elementType == null)
        throw new ArgumentNullException("elementType");
      //
      Type[] types = new Type[_ranks.Length];
      types[_ranks.Length - 1] = elementType;
      for (int i = _ranks.Length - 1; i > 0; i--)
        types[i - 1] = _ranks[i] == 1 ? types[i].MakeArrayType() : types[i].MakeArrayType(_ranks[i]);
      //
      int depth = 0;
      PwrStack<ArrayInfoNodeContext> arrayContexts = new PwrStack<ArrayInfoNodeContext>();
      ArrayInfoNode node = _node;
      Array array = node.ArrayInfo.CreateArray(types[depth]);
      ArrayLongIndex arrayIndex = new ArrayLongIndex(node.ArrayInfo);
      descent:
      while (depth < _ranks.Length - 1)
      {
        if (array.Length == 0)
          break;
        arrayContexts.Push(new ArrayInfoNodeContext(node, array, arrayIndex));
        depth++;
        node = arrayIndex.GetValue<ArrayInfoNode>(node.Nodes);
        Array a = node.ArrayInfo.CreateArray(types[depth]);
        arrayIndex.SetValue<Array>(array, a);
        array = a;
        if (depth < _ranks.Length - 1)
          arrayIndex = new ArrayLongIndex(node.ArrayInfo);
      }
      ascent:
      if (depth != 0)
      {
        ArrayInfoNodeContext nodeContext = arrayContexts.Pop();
        array = nodeContext.Array;
        node = nodeContext.Node;
        arrayIndex = nodeContext.Index;
        depth--;
        if (arrayIndex.IsMax)
          goto ascent;
        else
        {
          arrayIndex++;
          goto descent;
        }
      }
      //
      return array;
    }

    public override object GetValue(Array array, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "indices");
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      return GetValueCore(array, asRanges, zeroBased, indices);
    }

    public override void SetValue(Array array, object value, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (!array.GetJaggedArrayElementType().IsValueAssignable(value))
        throw new ArgumentException("Inassignable value", "value");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "indices");
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      SetValueCore(array, value, asRanges, zeroBased, indices);
    }

    public object GetValue(Array array, long[][] dimIndices)
    {
      return GetValue(array, false, false, dimIndices);
    }

    public object GetValue(Array array, bool asRanges, bool zeroBased, long[][] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array length", "indices");
      else
        for (int i = 0; i < dimIndices.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Value cannot be null", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Invalid array length", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      return GetValueCore(array, asRanges, zeroBased, dimIndices);
    }

    public void SetValue(Array array, object value, long[][] dimIndices)
    {
      SetValue(array, value, false, false, dimIndices);
    }

    public void SetValue(Array array, object value, bool asRanges, bool zeroBased, long[][] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (!array.GetJaggedArrayElementType().IsValueAssignable(value))
        throw new ArgumentException("Inassignable value", "value");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array length", "indices");
      else
        for (int i = 0; i < dimIndices.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Nulvalue ", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Invalid array length", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      SetValueCore(array, value, asRanges, zeroBased, dimIndices);
    }

    public override T GetValue<T>(Array array, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "indices");
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      return GetValueCore<T>(array, asRanges, zeroBased, indices);
    }

    public override void SetValue<T>(Array array, T value, bool asRanges, bool zeroBased, params long[] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (!array.GetJaggedArrayElementType().IsValueAssignable(value))
        throw new ArgumentException("Inassignable value", "value");
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "indices");
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");

      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      SetValueCore<T>(array, value, asRanges, zeroBased, indices);
    }

    public T GetValue<T>(Array array, long[][] dimIndices)
    {
      return GetValue<T>(array, false, false, dimIndices);
    }

    public T GetValue<T>(Array array, bool asRanges, bool zeroBased, long[][] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (typeof(T).IsAssignableFrom(array.GetJaggedArrayElementType()))
        throw new ArgumentNullException("Inconsistent array element type");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array length", "indices");
      else
        for (int i = 0; i < dimIndices.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Value cannot be null", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Invalid array length", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      return GetValueCore<T>(array, asRanges, zeroBased, dimIndices);
    }

    public void SetValue<T>(Array array, T value, long[][] dimIndices)
    {
      SetValue(array, value, false, false, dimIndices);
    }

    public void SetValue<T>(Array array, T value, bool asRanges, bool zeroBased, long[][] dimIndices)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      else if (array.GetJaggedArrayElementType().IsAssignableFrom(value != null ? value.GetType() : typeof(T)))
        throw new ArgumentNullException("Inconsistent array element type");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array length", "indices");
      else
        for (int i = 0; i < dimIndices.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Nulvalue ", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Invalid array length", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      SetValueCore(array, value, asRanges, zeroBased, dimIndices);
    }

    public override long CalcFlatIndex(bool zeroBased, params long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "indices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      //
      return CalcFlatIndexCore(zeroBased, indices);
    }

    public override void CalcDimIndices(long flatIndex, bool zeroBased, long[] dimIndices)
    {
      if (flatIndex < 0 || flatIndex >= _length)
        throw new ArgumentOutOfRangeException("flatIndex");
      else if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "dimIndices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
        indices[i] = new long[_ranks[i]];
      //
      CalcDimIndicesCore(flatIndex, zeroBased, indices);
      //
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        Array.Copy(indices[i], 0, dimIndices, j, _ranks[i]);
    }

    public override void GetMinDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        indices[i] = new long[_ranks[i]];
      //
      FirstDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        Array.Copy(indices[i], 0, dimIndices, j, _ranks[i]);
    }

    public override void GetMaxDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        indices[i] = new long[_ranks[i]];
      //
      LastDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        Array.Copy(indices[i], 0, dimIndices, j, _ranks[i]);
    }

    public override bool IncDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      //
      bool carry = NextDimIndicesCore(zeroBased, indices);
      if (carry)
        FirstDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        Array.Copy(indices[i], 0, dimIndices, j, _ranks[i]);
      return carry;
    }

    public override bool DecDimIndices(bool zeroBased, long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }
      //
      bool carry = PrevDimIndicesCore(zeroBased, indices);
      if (carry)
        LastDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0, j = 0; i < _ranks.Length && j < _rank; j += _ranks[i++])
        Array.Copy(indices[i], 0, dimIndices, j, _ranks[i]);
      return carry;
    }

    public long CalcFlatIndex(long[][] dimIndices)
    {
      return CalcFlatIndex(false, dimIndices);
    }

    public long CalcFlatIndex(bool zeroBased, long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array length", "indices");
      else
        for (int i = 0; i < dimIndices.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Value cannot be null", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("dimIndices", "Invalid array length", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      return CalcFlatIndexCore(zeroBased, dimIndices);
    }

    public void CalcDimIndices(long flatIndex, long[][] dimIndices)
    {
      CalcDimIndices(flatIndex, false, dimIndices);
    }

    public void CalcDimIndices(long flatIndex, bool zeroBased, long[][] dimIndices)
    {
      if (flatIndex < 0 || flatIndex >= _length)
        throw new ArgumentOutOfRangeException("flatIndex");
      else if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _rank)
        throw new ArgumentException("Invalid array length", "dimIndices");
      else
        for (int i = 0; i < _ranks.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("Value is null", "dimIndices", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("Invalid array size", "dimIndices", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
        indices[i] = new long[_ranks[i]];
      //
      CalcDimIndicesCore(flatIndex, zeroBased, indices);
      //
      for (int i = 0; i < _ranks.Length; i++)
        Array.Copy(indices[i], 0, dimIndices[i], 0, _ranks[i]);
    }

    public void GetMinDimIndices(long[][] dimIndices)
    {
      GetMinDimIndices(false, dimIndices);
    }

    public void GetMinDimIndices(bool zeroBased, long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else
        for (int i = 0; i < _ranks.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("Value is null", "dimIndices", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("Invalid array size", "dimIndices", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
        indices[i] = new long[_ranks[i]];
      //
      FirstDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0; i < _ranks.Length; i++)
        Array.Copy(indices[i], 0, dimIndices[i], 0, _ranks[i]);
    }

    public void GetMaxDimIndices(long[][] dimIndices)
    {
      GetMaxDimIndices(false, dimIndices);
    }

    public void GetMaxDimIndices(bool zeroBased, long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else
        for (int i = 0; i < _ranks.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("Value is null", "dimIndices", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("Invalid array size", "dimIndices", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
        indices[i] = new long[_ranks[i]];
      //
      LastDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0; i < _ranks.Length; i++)
        Array.Copy(indices[i], 0, dimIndices[i], 0, _ranks[i]);
    }

    public bool IncDimIndices(long[][] dimIndices)
    {
      return IncDimIndices(false, dimIndices);
    }

    public bool IncDimIndices(bool zeroBased, long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else
        for (int i = 0; i < _ranks.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("Value is null", "dimIndices", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("Invalid array size", "dimIndices", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices[i], 0, indices[i], 0, _ranks.Length);
      }
      //
      bool carry = NextDimIndicesCore(zeroBased, indices);
      if (carry)
        FirstDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0; i < _ranks.Length; i++)
        Array.Copy(indices[i], 0, dimIndices[i], 0, _ranks[i]);
      return carry;
    }

    public bool DecDimIndices(long[][] dimIndices)
    {
      return DecDimIndices(false, dimIndices);
    }

    public bool DecDimIndices(bool zeroBased, long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length != _ranks.Length)
        throw new ArgumentException("Invalid array size", "dimIndices");
      else
        for (int i = 0; i < _ranks.Length; i++)
          if (dimIndices[i] == null)
            throw new ArgumentRegularArrayLongElementException("Value is null", "dimIndices", i);
          else if (dimIndices[i].Length != _ranks[i])
            throw new ArgumentRegularArrayLongElementException("Invalid array size", "dimIndices", i);
      if (_length == 0)
        throw new InvalidOperationException("Array has zero length");
      //
      long[][] indices = new long[_ranks.Length][];
      for (int i = 0; i < _ranks.Length; i++)
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices[i], 0, indices[i], 0, _ranks.Length);
      }
      //
      bool carry = PrevDimIndicesCore(zeroBased, indices);
      if (carry)
        LastDimIndicesCore(zeroBased, indices);
      //
      for (int i = 0; i < _ranks.Length; i++)
        Array.Copy(indices[i], 0, dimIndices[i], 0, _ranks[i]);
      return carry;
    }

    public RegularArrayLongInfo GetDimArrayInfo(bool zeroBased, params long[] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length < 1 || dimIndices.Length > _rank - _ranks[_ranks.Length - 1])
        throw new ArgumentException("Invalid array length", "dimIndices");
      int length = dimIndices.Length;
      int depth = 0;
      for (; depth < _ranks.Length - 1 && length >= _ranks[depth]; length -= _ranks[depth], depth++) ;
      if (length != 0)
        throw new ArgumentException("Invalid array length", "dimIndices");

      long[][] indices = new long[depth][];
      for (int i = 0, j = 0; i < depth && j < dimIndices.Length; j += _ranks[i++])
      {
        indices[i] = new long[_ranks[i]];
        Array.Copy(dimIndices, j, indices[i], 0, _ranks[i]);
      }

      return GetDimArrayInfoCore(zeroBased, indices);
    }

    public RegularArrayLongInfo GetDimArrayInfo(bool zeroBased, params long[][] dimIndices)
    {
      if (dimIndices == null)
        throw new ArgumentNullException("dimIndices");
      else if (dimIndices.Length < 1 || dimIndices.Length > _ranks.Length - 1)
        throw new ArgumentException("Invalid array length", "dimIndices");
      else
        for (int j = 0; j < dimIndices.Length; j++)
          if (dimIndices[j] == null)
            throw new ArgumentRegularArrayLongElementException("Array is null", "dimIndices", j);
          else if (dimIndices[j].Length != _ranks[j])
            throw new ArgumentRegularArrayLongElementException("Invalid array length", "dimIndices", j);

      return GetDimArrayInfoCore(zeroBased, dimIndices);
    }

    #endregion
    #endregion
    #region Embedded types

    class ArrayInfoNodeContext
    {
      public ArrayInfoNodeContext(ArrayInfoNode node, Array array, ArrayLongIndex index)
      {
        Node = node;
        Array = array;
        Index = index;
      }

      public ArrayInfoNode Node
      {
        get;
        private set;
      }

      public Array Array
      {
        get;
        private set;
      }

      public ArrayLongIndex Index
      {
        get;
        private set;
      }
    }

    class ArrayInfoNode
    {
      public ArrayInfoNode(RegularArrayLongInfo arrayInfo, long total, bool elementary)
      {
        ArrayInfo = arrayInfo;
        Nodes = !elementary ? arrayInfo.CreateArray(typeof(ArrayInfoNode)) : null;
        Total = total;
      }

      public RegularArrayLongInfo ArrayInfo
      {
        get;
        private set;
      }

      public Array Nodes
      {
        get;
        private set;
      }

      public long Total
      {
        get;
        private set;
      }
    }

    #endregion
  }
}
