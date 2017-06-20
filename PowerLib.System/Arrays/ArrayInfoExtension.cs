using System;
using System.Collections.Generic;
using System.Linq;
using PowerLib.System.Collections;
using PowerLib.System.Linq;

namespace PowerLib.System
{
	public static class ArrayInfoExtension
	{
		public static void ValidateRanges(this RegularArrayInfo arrayInfo, params Range[] ranges)
		{
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (ranges == null)
        throw new ArgumentNullException("ranges");
      if (ranges.Length != arrayInfo.Rank)
        throw new ArgumentException("Invalid array length", "ranges");
      if (Enumerable.Range(0, ranges.Length)
        .Any(i => ranges[i].Index < arrayInfo.LowerBounds[i] || ranges[i].Index + ranges[i].Count > arrayInfo.LowerBounds[i] + arrayInfo.Lengths[i]))
				throw new ArgumentException("Out of range dimension exists");
			//return new RegularArrayInfo(ranges.Select(r => new ArrayDimension(r.Count, r.Index)).ToArray());
		}

		public static void ValidateRanges(this RegularArrayLongInfo arrayInfo, params LongRange[] ranges)
		{
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (ranges == null)
        throw new ArgumentNullException("ranges");
      if (ranges.Length != arrayInfo.Rank)
        throw new ArgumentException("Invalid array length", "ranges");
      if (Enumerable.Range(0, ranges.Length)
        .Any(i => ranges[i].Index < arrayInfo.LowerBounds[i] || ranges[i].Index + ranges[i].Count > arrayInfo.LowerBounds[i] + arrayInfo.Lengths[i]))
				throw new ArgumentException("Out of range dimension exists");
			//return new RegularArrayLongInfo (ranges.Select(r => new ArrayLongDimension(r.Count, r.Index)).ToArray());
		}

		public static void GetDimIndices(this ArrayIndex arrayIndex, int[][] dimIndices)
		{
			if (arrayIndex == null)
				throw new NullReferenceException("arrayIndex");
			JaggedArrayInfo arrayInfo = arrayIndex.ArrayInfo as JaggedArrayInfo;
			if (arrayInfo == null)
				throw new InvalidOperationException("Indexearray is nojagged");
			//
			if (dimIndices == null)
				throw new ArgumentNullException("dimIndices");
			else if (dimIndices.Length != arrayInfo.Depths)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
			else
				for (int i = 0; i < arrayInfo.Depths; i++)
					if (dimIndices[i] == null)
						throw new ArgumentRegularArrayElementException("dimIndices", "Value is null", i);
					else if (dimIndices[i].Length != arrayInfo.GetRank(i))
						throw new ArgumentRegularArrayElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], i);
			//
			for (int i = 0, j = 0; i < arrayInfo.Depths; j += arrayInfo.GetRank(i), i++)
				for (int k = 0; k < arrayInfo.GetRank(i); k++)
					dimIndices[i][k] = arrayIndex.GetDimIndex(j + k);
		}

		public static void GetDimIndices(this ArrayLongIndex arrayIndex, long[][] dimIndices)
		{
			if (arrayIndex == null)
				throw new NullReferenceException("arrayIndex");
			JaggedArrayLongInfo arrayInfo = arrayIndex.ArrayInfo as JaggedArrayLongInfo;
			if (arrayInfo == null)
				throw new InvalidOperationException("Indexearray is nojagged");
			//
			if (dimIndices == null)
				throw new ArgumentNullException("dimIndices");
			else if (dimIndices.Length != arrayInfo.Depths)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
			else
				for (int i = 0; i < arrayInfo.Depths; i++)
					if (dimIndices[i] == null)
						throw new ArgumentRegularArrayLongElementException("dimIndices", "Value is null", i);
					else if (dimIndices[i].Length != arrayInfo.GetRank(i))
						throw new ArgumentRegularArrayLongElementException("dimIndices", ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], i);
			//
			for (int i = 0, j = 0; i < arrayInfo.Depths; j += arrayInfo.GetRank(i), i++)
				for (int k = 0; k < arrayInfo.GetRank(i); k++)
					dimIndices[i][k] = arrayIndex.GetLongDimIndex(j + k);
		}

    public static int TransformFlatIndex(this RegularArrayInfo arrayInfo, int flatIndex, params Range[] ranges)
    {
      arrayInfo.ValidateRanges(ranges);
      int[] dimIndices = new int[arrayInfo.Rank];
      RegularArrayInfo ai = new RegularArrayInfo(ranges.Select(r => new ArrayDimension(r.Count, r.Index)).ToArray());
      ai.CalcDimIndices(flatIndex, dimIndices);
      return arrayInfo.CalcFlatIndex(dimIndices);
    }

    public static long TransformFlatIndex(this RegularArrayLongInfo arrayInfo, long flatIndex, params LongRange[] ranges)
    {
      arrayInfo.ValidateRanges(ranges);
      long[] dimIndices = new long[arrayInfo.Rank];
      RegularArrayLongInfo ai = new RegularArrayLongInfo(ranges.Select(r => new ArrayLongDimension(r.Count, r.Index)).ToArray());
      ai.CalcDimIndices(flatIndex, dimIndices);
      return arrayInfo.CalcFlatIndex(dimIndices);
    }

    public static IEnumerable<int> EnumerateFlatIndex(this RegularArrayInfo arrayInfo, params Range[] ranges)
    {
      arrayInfo.ValidateRanges(ranges);
      int[] dimIndices = new int[arrayInfo.Rank];
      for (ArrayIndex ai = new ArrayIndex(new RegularArrayInfo(ranges.Select(r => new ArrayDimension(r.Count, r.Index)).ToArray())); ai.Carry == 0; ai++)
      {
        ai.GetDimIndices(dimIndices);
        yield return arrayInfo.CalcFlatIndex(dimIndices);
      }
    }

    public static IEnumerable<long> EnumerateFlatIndex(this RegularArrayLongInfo arrayInfo, params LongRange[] ranges)
    {
      arrayInfo.ValidateRanges(ranges);
      long[] dimIndices = new long[arrayInfo.Rank];
      for (ArrayLongIndex ai = new ArrayLongIndex(new RegularArrayLongInfo(ranges.Select(r => new ArrayLongDimension(r.Count, r.Index)).ToArray())); ai.Carry == 0; ai++)
      {
        ai.GetDimIndices(dimIndices);
        yield return arrayInfo.CalcFlatIndex(dimIndices);
      }
    }

    public static Array CreateArray<T>(this RegularArrayInfo arrayInfo, ICollection<T> coll)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (coll.Count != arrayInfo.Length)
        throw new ArgumentException("Invalid collection length", "coll");

      var indices = new int[arrayInfo.Rank];
      var array = arrayInfo.CreateArray<T>();
      coll.ForEach((t, i) =>
      {
        arrayInfo.CalcDimIndices(i, indices);
        arrayInfo.SetValue(array, t, indices);
      });
      return array;
    }

    public static Array CreateArray<T>(this RegularArrayLongInfo arrayInfo, ICollection<T> coll)
    {
      if (arrayInfo == null)
        throw new ArgumentNullException("arrayInfo");
      if (coll == null)
        throw new ArgumentNullException("coll");
      if (coll.Count != arrayInfo.Length)
        throw new ArgumentException("Invalid collection length", "coll");

      var indices = new long[arrayInfo.Rank];
      var array = arrayInfo.CreateArray<T>();
      coll.ForEach((t, i) =>
      {
        arrayInfo.CalcDimIndices(i, indices);
        arrayInfo.SetValue(array, t, indices);
      });
      return array;
    }
  }
}
