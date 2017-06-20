using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PowerLib.System.Collections;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;


namespace PowerLib.System
{
	/// <summary>
	/// Array extensiomethods
	/// <remarks>
	/// Almethods twork with multidimensioanjagged arrays arnonrecursive.
	/// </remarks>
	/// </summary>^
	public static class PwrArray
	{
    //  Pattern formatting parameters: 0 - delimiters, 1 - spaces, 2 - escapes, 3 - open brackets, 4 - close brackets
    //private const string arrayItemsFormat = @"(?:[^{0}{1}{2}]|(?:[{2}][{0}{1}]))*";
    private const string arrayItemsFormat = @"(?:(?:[^{0}{1}{2}{3}{4}]|(?:[{2}].))(?:(?:[^{0}{2}{3}{4}]|(?:[{2}].))*(?:[^{0}{1}{2}{3}{4}]|(?:[{2}].)))?)?";
    //  Pattern formatting parameters: 0 - delimiters, 1 - spaces, 2 - escapes, 3 - open brackets, 4 - close brackets
    private const string arrayDelimiterFormat = @"[{1}]*(?<![{2}])[{0}][{1}]*";
    //  Pattern formatting parameters: 0 - item pattern, 1 - delimiters, 2 - spaces, 3 - escapes, 4 - open brackets, 5 - close brackets
    private const string arrayPatternFormat =
      @"^[{2}]*(?'Open'[{4}][{2}]*)(?'Items'(?:(?:(?<![^{3}][{4}])[{2}]*(?<![{3}])[{1}][{2}]*)?{0})*)[{2}]*(?'Close-Open'[{5}][{2}]*)[{2}]*(?(Open)(?!))$";
    //  Pattern formatting parameters: 0 - item pattern, 1 - delimiters, 2 - spaces, 3 - escapes, 4 - open brackets, 5 - close brackets
    private const string regularArrayPatternFormat = @"^[{2}]*(?:(?:(?:(?<=[^{3}][{5}][{2}]*)[{1}][{2}]*)?" +
      @"(?'Openings'(?'Open'[{4}][{2}]*)+))(?'Items'(?:(?:(?<![^{3}][{4}])[{2}]*(?<![{3}])[{1}][{2}]*)?{0})*)[{2}]*(?'Closings'(?'Close-Open'[{5}][{2}]*)+))*[{2}]*(?(Open)(?!))$";

    private static readonly string IndexOpenBracket;
		private static readonly string IndexCloseBracket;
		private static readonly string IndexLevelDelimiter;
		private static readonly string IndexItemDelimiter;
		private static readonly string IndexItemFormat;

		#region Constructor

		static PwrArray()
		{
      IndexOpenBracket = "";// ArrayResources.Default.Strings[ArrayMessage.IndexOpenBracket];
      IndexCloseBracket = "";// ArrayResources.Default.Strings[ArrayMessage.IndexCloseBracket];
      IndexLevelDelimiter = "";// ArrayResources.Default.Strings[ArrayMessage.IndexLevelDelimiter];
      IndexItemDelimiter = "";// ArrayResources.Default.Strings[ArrayMessage.IndexItemDelimiter];
      IndexItemFormat = "";// ArrayResources.Default.Strings[ArrayMessage.IndexItemFormat];
		}

		#endregion
		#region Regular array extensions
		#region Miscellaneous regular extensions

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Type GetRegularArrayElementType(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      return array.GetType().GetElementType();
		}

		public static int[] GetRegularArrayLowerBounds(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      return Enumerable.Range(0, array.Rank)
				.Select(dim => array.GetLowerBound(dim))
				.ToArray();
		}

		public static long[] GetRegularArrayLongLowerBounds(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      return Enumerable.Range(0, array.Rank)
				.Select(dim => 0L)
				.ToArray();
		}

		public static int[] GetRegularArrayLengths(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      return Enumerable.Range(0, array.Rank)
				.Select(dim => array.GetLength(dim))
				.ToArray();
		}

		public static long[] GetRegularArrayLongLengths(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      return Enumerable.Range(0, array.Rank)
				.Select(dim => array.GetLongLength(dim))
				.ToArray();
		}

    public static ArrayDimension[] GetRegularArrayDimensions(this Array array, bool zeroBased, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges != null && ranges.Length > 0)
			{
				if (ranges.Length != array.Rank)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "ranges");
				for (int i = 0; i < ranges.Length; i++)
				{
					int length = array.GetLength(i);
					int lowerBound = array.GetLowerBound(i);
					if (ranges[i].Index < (zeroBased ? 0 : lowerBound) || ranges[i].Index > (zeroBased ? 0 : lowerBound) + length)
						throw new ArgumentRegularArrayElementException("ranges", i);
					else if (ranges[i].Index + ranges[i].Count > (zeroBased ? 0 : lowerBound) + length)
						throw new ArgumentRegularArrayElementException("ranges", i);
				}
			}

			return Enumerable.Range(0, array.Rank)
				.Select(dim => ranges != null && ranges.Length > 0 ?
					new ArrayDimension(ranges[dim].Count, ranges[dim].Index + (zeroBased ? array.GetLowerBound(dim) : 0)) :
					new ArrayDimension(array.GetLength(dim)))
				.ToArray();
		}

    public static ArrayDimension[] GetRegularArrayDimensions(this Array array, params Range[] ranges)
    {
      return array.GetRegularArrayDimensions(false, ranges);
    }

    public static ArrayLongDimension[] GetRegularArrayLongDimensions(this Array array, params LongRange[] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges != null && ranges.Length > 0)
			{
				if (ranges.Length != array.Rank)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "ranges");
				for (int i = 0; i < ranges.Length; i++)
				{
					long length = array.GetLongLength(i);
					if (ranges[i].Index < 0 || ranges[i].Index > length)
						throw new ArgumentRegularArrayElementException("ranges", i);
					else if (ranges[i].Index + ranges[i].Count > length)
						throw new ArgumentRegularArrayElementException("ranges", i);
				}
			}

			return Enumerable.Range(0, array.Rank)
				.Select(dim => ranges != null && ranges.Length > 0 ?
					new ArrayLongDimension(ranges[dim].Count, ranges[dim].Index) :
					new ArrayLongDimension(array.GetLongLength(dim), 0L))
				.ToArray();
		}

		#endregion
		#region Validation methods

		public static void ValidateRange(this Array array, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");
		}

		public static void ValidateRange(this Array array, long index, long count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0|| index > array.LongLength)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0|| count > array.LongLength - index)
				throw new ArgumentOutOfRangeException("count");
		}

		public static void ValidateRange(this Array array, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("Index is out of range", "range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("Count is out of range", "range");
		}

		public static void ValidateRange(this Array array, LongRange range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.Index < 0|| range.Index > array.LongLength)
				throw new ArgumentOutOfRangeException("Index is out of range", "range");
			if (range.Count < 0|| range.Count > array.LongLength - range.Index)
				throw new ArgumentOutOfRangeException("Count is out of range", "range");
		}

		public static void ValidateRanges(this Array array, Range[] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
				throw new ArgumentNullException("ranges");
			if (ranges.Length != array.Rank)
				throw new ArgumentException("Invalid array length", "ranges");

			for (int i = 0; i < array.Rank; i++)
			{
				try
				{
					int bias = array.GetLowerBound(i);
					int length = array.GetLength(i);
					if (ranges[i].Index < bias || ranges[i].Index > bias + length)
						throw new ArgumentOutOfRangeException(null, "Index is out of range");
					if (ranges[i].Count < 0 || ranges[i].Count > bias + length - ranges[i].Index)
						throw new ArgumentOutOfRangeException(null, "Count is out of range");
				}
				catch (Exception ex)
				{
					throw new ArgumentRegularArrayElementException("ranges", ex, i);
				}
			}
		}

		public static void ValidateRanges(this Array array, LongRange[] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
				throw new ArgumentNullException("ranges");
			if (ranges.Length != array.Rank)
				throw new ArgumentException("Invalid array length", "ranges");

			for (int i = 0; i < array.Rank; i++)
			{
				try
				{
					long longLength = array.GetLongLength(i);
					if (ranges[i].Index < 0|| ranges[i].Index > longLength)
						throw new ArgumentOutOfRangeException(null, "Index is out of range");
					if (ranges[i].Count < 0|| ranges[i].Count > longLength - ranges[i].Index)
						throw new ArgumentOutOfRangeException(null, "Count is out of range");
				}
				catch (Exception ex)
				{
					throw new ArgumentRegularArrayElementException("ranges", ex, i);
				}
			}
		}

		#endregion
		#region Create regular array

		public static Array CreateAsRegular<T>(ArrayDimension[] dimensions)
		{
			if (dimensions == null)
				throw new ArgumentNullException("arrayDims");
			if (dimensions.Length == 0)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.ArrayIsEmpty], "dimensions");

			return Array.CreateInstance(typeof(T), dimensions.Select(d => d.Length).ToArray(), dimensions.Select(d => d.LowerBound).ToArray());
		}

		public static Array CreateAsRegular<T>(int[] lengths, int[] lowerBounds = null)
		{
			if (lengths == null)
				throw new ArgumentNullException("lengths");
			if (lengths.Length == 0)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.ArrayIsEmpty], "lengths");
			if (lowerBounds != null && lengths.Length != lowerBounds.Length)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "lowerBounds");

			return lowerBounds != null ? Array.CreateInstance(typeof(T), lengths, lowerBounds) : Array.CreateInstance(typeof(T), lengths);
		}

		public static Array CreateAsLongRegular<T>(long[] lengths)
		{
			if (lengths == null)
				throw new ArgumentNullException("lengths");
			if (lengths.Length == 0)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.ArrayIsEmpty], "lengths");

			return Array.CreateInstance(typeof(T), lengths);
		}

    #endregion
    #region Enumerate regular array

    public static IEnumerable<T> EnumerateAsRegular<T>(this Array array, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      ArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.Length != 0)
				for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
          yield return arrayIndex.GetValue<T>(array);
		}

    public static IEnumerable<T> EnumerateAsLongRegular<T>(this Array array, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      Type type = array.GetType().GetElementType();
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      ArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.LongLength != 0)
				for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
					yield return arrayIndex.GetValue<T>(array);
		}

		#endregion
		#region Clone regular array

		public static Array CloneAsRegular<T>(this Array array)
		{
			return array.RangeAsRegular();
		}

		public static Array CloneAsLongRegular<T>(this Array array)
		{
			return array.RangeAsLongRegular();
		}

    #endregion
    #region Parse regular array

    public static Array ParseAsRegular<T>(string s, Func<string, T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars, char[] openingChars, char[] closingChars)
    {
      if (itemParser == null)
        throw new ArgumentNullException("itemParser");

      return ParseAsRegular(s, (t, fi, di) => itemParser(t), itemPattern, delimitChars, spaceChars, escapeChars, openingChars, closingChars);
    }

    public static Array ParseAsRegular<T>(string s, Func<string, int, int[], T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars, char[] openingChars, char[] closingChars)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      if (itemParser == null)
        throw new ArgumentNullException("itemParser");
      if (delimitChars == null)
        throw new ArgumentNullException("delimitChars");
      if (openingChars == null)
        throw new ArgumentNullException("openingChars");
      if (closingChars == null)
        throw new ArgumentNullException("closingChars");

      string pattern = string.Format(regularArrayPatternFormat,
        !string.IsNullOrEmpty(itemPattern) ? itemPattern : string.Format(arrayItemsFormat,
          Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
          spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty,
          Regex.Escape(new string(openingChars)).Escape('\\', false, ']', '}'), Regex.Escape(new string(closingChars)).Escape('\\', false, ']', '}')),
        Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
        spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty,
        Regex.Escape(new string(openingChars)).Escape('\\', false, ']', '}'), Regex.Escape(new string(closingChars)).Escape('\\', false, ']', '}'));
      Match match = Regex.Match(s, pattern, RegexOptions.Multiline);
      string splitter = string.Format(arrayDelimiterFormat,
        Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
        spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty);
      Regex split = new Regex(splitter, RegexOptions.Multiline);

      int rank = 0;
      int[] lengths = null;
      int[] indices = null;
      CaptureCollection openings = match.Groups["Openings"].Captures;
      CaptureCollection closings = match.Groups["Closings"].Captures;
      CaptureCollection items = match.Groups["Items"].Captures;
      for (int index = 0, openingCount = 0, closingCount = 0, count = items.Count; index < count; index++)
      {
        openingCount = Enumerable.Range(openings[index].Index, openings[index].Length).Count(i => openingChars.Contains(s[i]));
        if (closingCount == 0)
        {
          rank = openingCount;
          lengths = new int[rank];
          indices = new int[rank];
        }
        else if (closingCount != openingCount)
          throw new ArgumentException("Invalid array string data format", "s");
        closingCount = Enumerable.Range(closings[index].Index, closings[index].Length).Count(i => closingChars.Contains(s[i]));
        if ((closingCount == rank || indices[rank - 1 - closingCount] == 0) && closingCount > 1)
          lengths[rank - closingCount] = indices[rank -closingCount] + 1;
        if (closingCount < rank)
          indices[rank - 1 - closingCount] += 1;
        for (int i = rank - closingCount; i < rank; i++)
          indices[i] = 0;
        if (closingCount == lengths.Length)
          break;
      }
      Array array = null;
      if (items.Count > 0)
      {
        string[] data = split.Split(items[0].Value);
        lengths[rank - 1] = data.Length;
        RegularArrayInfo arrayInfo = new RegularArrayInfo(lengths);
        array = arrayInfo.CreateArray<T>();
        for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = 0 }; arrayIndex.Carry == 0; arrayIndex++)
        {
          arrayIndex.GetDimIndices(indices);
          if (arrayIndex.FlatIndex > 0 && indices[rank - 1] == 0)
            data = split.Split(items[arrayIndex.FlatIndex / lengths[rank - 1]].Value);
          if (data.Length != lengths[rank - 1])
            throw new InvalidOperationException("Invalid items length");
          arrayIndex.SetValue(array, itemParser(data[indices[rank - 1]], arrayIndex.FlatIndex, indices));
        }
      }
      return array;
    }

    #endregion
    #region Format regular array

    public static string FormatAsRegular<T>(this Array array, Func<T, string> itemFormatter, Func<int, string> itemDelimiter, Func<int, string> openBracket, Func<int, string> closeBracket,
      bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      return array.FormatAsRegular<T>((t, fi, di) => itemFormatter(t), itemDelimiter, openBracket, closeBracket, null, zeroBased, range, ranges);
    }

    public static string FormatAsRegular<T>(this Array array, Func<T, int, int[], string> itemFormatter, Func<int, string> itemDelimiter, Func<int, string> openBracket, Func<int, string> closeBracket,
      int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (itemFormatter == null)
        throw new ArgumentNullException("itemFormatter");
      if (itemDelimiter == null)
        throw new ArgumentNullException("itemDelimiter");
      if (openBracket == null)
        throw new ArgumentNullException("openBracket");
      if (closeBracket == null)
        throw new ArgumentNullException("closeBracket");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      StringBuilder sb = new StringBuilder();
      if (range.HasValue)
      {
        sb.Append(openBracket);
        if (array.Length != 0)
        {
          for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.Value.Index, ZeroBased = zeroBased, AsRanges = ranges != null };
            arrayIndex.Carry == 0 && arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count); arrayIndex++)
          {
            if (arrayIndex.FlatIndex > 0)
              sb.Append(itemDelimiter);
            if (indices != null)
              arrayIndex.GetDimIndices(indices);
            sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices));
          }
        }
        sb.Append(closeBracket);
      }
      else
      {
        if (array.Length == 0)
        {
          for (int i = 0; i < array.Rank; i++)
            sb.Append(openBracket(i));
          for (int i = array.Rank - 1; i >= 0; i--)
            sb.Append(closeBracket(i));
        }
        else
        {
          for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { ZeroBased = zeroBased, AsRanges = ranges != null };
            arrayIndex.Carry == 0; arrayIndex++)
          {
            int brackets;
            for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
            if (arrayIndex.FlatIndex > 0)
              sb.Append(itemDelimiter(arrayInfo.Rank - 1 - brackets));
            for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
              sb.Append(openBracket(i));
            if (indices != null)
              arrayIndex.GetDimIndices(indices);
            sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices));
            for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
            for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
              sb.Append(closeBracket(i));
          }
        }
      }
      return sb.ToString();
    }

    public static string FormatAsLongRegular<T>(this Array array, Func<T, string> itemFormatter, Func<int, string> itemDelimiter, Func<int, string> openBracket, Func<int, string> closeBracket,
      LongRange? range = null, params LongRange[] ranges)
    {
      return array.FormatAsLongRegular<T>((t, fi, di) => itemFormatter(t), itemDelimiter, openBracket, closeBracket, null, false, range, ranges);
    }

    public static string FormatAsLongRegular<T>(this Array array, Func<T, long, long[], string> itemFormatter, Func<int, string> itemDelimiter, Func<int, string> openBracket, Func<int, string> closeBracket,
      long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (itemFormatter == null)
				throw new ArgumentNullException("itemFormatter");
			if (itemDelimiter == null)
				throw new ArgumentNullException("itemDelimiter");
			if (openBracket == null)
				throw new ArgumentNullException("openBracket");
			if (closeBracket == null)
				throw new ArgumentNullException("closeBracket");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");

      Type type = array.GetType().GetElementType();
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      StringBuilder sb = new StringBuilder();
      if (range.HasValue)
      {
        sb.Append(openBracket);
        if (array.Length != 0)
        {
          for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
            arrayIndex.Carry == 0 && arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count); arrayIndex++)
          {
            if (arrayIndex.FlatIndex > 0)
              sb.Append(itemDelimiter);
            if (indices != null)
              arrayIndex.GetDimIndices(indices);
            sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices));
          }
        }
        sb.Append(closeBracket);
      }
      else
      {
        if (array.LongLength == 0)
        {
          for (int i = 0; i < arrayInfo.Rank; i++)
            sb.Append(openBracket(i));
          for (int i = arrayInfo.Rank - 1; i >= 0; i--)
            sb.Append(closeBracket(i));
        }
        else
        {
          for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { ZeroBased = zeroBased, AsRanges = ranges != null };
            arrayIndex.Carry == 0; arrayIndex++)
          {
            int brackets;
            for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
            if (arrayIndex.FlatIndex > 0)
              sb.Append(itemDelimiter(arrayInfo.Rank - 1 - brackets));
            for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
              sb.Append(openBracket(i));
            if (indices != null)
              arrayIndex.GetDimIndices(indices);
            sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices));
            for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
            for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
              sb.Append(closeBracket(i));
          }
        }
      }
			return sb.ToString();
		}

    #endregion
    #region Select regular array

    public static IEnumerable<TResult> SelectAsRegular<TSource, TResult>(this Array array, Func<TSource, TResult> selector, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      return array.SelectAsRegular<TSource, TResult>((t, fi, di) => selector(t), null, zeroBased, range, ranges);
    }

    public static IEnumerable<TResult> SelectAsRegular<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> selector, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (selector == null)
				throw new ArgumentNullException("selector");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

			Type type = array.GetType().GetElementType();
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.Length != 0)
			{
        for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
        {
          if (indices != null)
						arrayIndex.GetDimIndices(indices);
					yield return selector(arrayIndex.GetValue<TSource>(array), arrayIndex.FlatIndex, indices);
				}
			}
		}

    public static IEnumerable<TResult> SelectAsLongRegular<TSource, TResult>(this Array array, Func<TSource, TResult> selector, LongRange? range = null, params LongRange[] ranges)
    {
      return array.SelectAsLongRegular<TSource, TResult>((t, fi, di) => selector(t), null, false, range, ranges);
    }

    public static IEnumerable<TResult> SelectAsLongRegular<TSource, TResult>(this Array array, Func<TSource, long, long[], TResult> selector, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (selector == null)
				throw new ArgumentNullException("selector");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      Type type = array.GetType().GetElementType();
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.LongLength != 0)
			{
        for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count)); arrayIndex++)
        {
          if (indices != null)
						arrayIndex.GetDimIndices(indices);
					yield return selector(arrayIndex.GetValue<TSource>(array), arrayIndex.FlatIndex, indices);
				}
			}
		}

    #endregion
    #region Where regular array

    public static IEnumerable<T> WhereAsRegular<T>(this Array array, Func<T, bool> predicate, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      return array.WhereAsRegular<T>((t, fi, di) => predicate(t), null, zeroBased, range, ranges);
    }

    public static IEnumerable<T> WhereAsRegular<T>(this Array array, Func<T, int, int[], bool> predicate, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (predicate == null)
				throw new ArgumentNullException("predicate");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "dimIndices");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.Length != 0)
			{
        for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          T value = arrayIndex.GetValue<T>(array);
          if (predicate(value, arrayIndex.FlatIndex, indices))
					  yield return value ;
				}
			}
		}

    public static IEnumerable<T> WhereAsLongRegular<T>(this Array array, Func<T, bool> predicate, LongRange? range = null, params LongRange[] ranges)
    {
      return array.WhereAsLongRegular<T>((t, fi, di) => predicate(t), null, false, range, ranges);
    }

    public static IEnumerable<T> WhereAsLongRegular<T>(this Array array, Func<T, long, long[], bool> predicate, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (predicate == null)
				throw new ArgumentNullException("predicate");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      Type type = array.GetType().GetElementType();
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.LongLength != 0)
			{
        for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count)); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          T value = arrayIndex.GetValue<T>(array);
          if (predicate(value, arrayIndex.FlatIndex, indices))
					  yield return value ;
				}
			}
		}

    #endregion
    #region Apply regular array

    public static void ApplyAsRegular<T>(this Array array, Action<T> action, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      array.ApplyAsRegular<T>((t, fi, di) => action(t), null, zeroBased, range, ranges);
    }

    public static void ApplyAsRegular<T>(this Array array, Action<T, int, int[]> action, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
        throw new ArgumentNullException("action");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.Length != 0)
      {
        for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          action(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices);
        }
      }
    }

    public static void ApplyAsLongRegular<T>(this Array array, Action<T> action, LongRange? range = null, params LongRange[] ranges)
    {
      array.ApplyAsLongRegular<T>((t, fi, di) => action(t), null, false, range, ranges);
    }

    public static void ApplyAsLongRegular<T>(this Array array, Action<T, long, long[]> action, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
        throw new ArgumentNullException("action");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.LongLength != 0)
      {
        for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count)); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          action(arrayIndex.GetValue<T>(array), arrayIndex.FlatIndex, indices);
        }
      }
    }

    #endregion
    #region Fill regular array

    public static void FillAsRegular<T>(this Array array, T value, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      array.FillAsRegular<T>((fi, di) => value, null, zeroBased, range, ranges);
    }

    public static void FillAsRegular<T>(this Array array, Func<int, int[], T> valuator, int[] indices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
        throw new ArgumentNullException("valuator");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.Length != 0)
      {
        for (ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < range.Value.Index + range.Value.Count); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          arrayIndex.SetValue<T>(array, valuator(arrayIndex.FlatIndex, indices));
        }
      }
    }

    public static void FillAsLongRegular<T>(this Array array, T value, LongRange? range = null, params LongRange[] ranges)
    {
      array.FillAsLongRegular<T>((fi, di) => value, null, false, range, ranges);
    }

    public static void FillAsLongRegular<T>(this Array array, Func<long, long[], T> valuator, long[] indices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
        throw new ArgumentNullException("valuator");
      if (indices != null && indices.Length != array.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "indices");
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (range.HasValue && (range.Value.Index > arrayInfo.Length || range.Value.Count > arrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      if (array.LongLength != 0)
      {
        for (ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0L, ZeroBased = zeroBased, AsRanges = ranges != null };
          arrayIndex.Carry == 0 && (!range.HasValue || arrayIndex.FlatIndex < (range.Value.Index + range.Value.Count)); arrayIndex++)
        {
          if (indices != null)
            arrayIndex.GetDimIndices(indices);
          arrayIndex.SetValue<T>(array, valuator(arrayIndex.FlatIndex, indices));
        }
      }
    }

    #endregion
    #region Convert regular array

    public static Array ConvertAsRegular<TSource, TResult>(this Array array, Func<TSource, TResult> converter, int[] transposition = null, int[] lowerBounds = null,
      bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      return array.ConvertAsRegular<TSource, TResult>((t, sfi, sdi, tfi, tdi) => converter(t), transposition, lowerBounds, null, null, zeroBased, range, ranges);
    }

    public static Array ConvertAsRegular<TSource, TResult>(this Array array, Func<TSource, int, int[], int, int[], TResult> converter, int[] transposition = null, int[] lowerBounds = null,
      int[] targetIndices = null, int[] sourceIndices = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (converter == null)
				throw new ArgumentNullException("converter");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      Type type = array.GetType().GetElementType();
      if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayInfo sourceArrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (transposition != null)
      {
        if (transposition.Length == 0 || transposition.Length > sourceArrayInfo.Rank)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "transposition");
        if (transposition.Count(d => d < 0 || d >= sourceArrayInfo.Rank) > 0)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreArrayElementsOutOfRange], "transposition");
        for (int i = 0; i < sourceArrayInfo.Rank; i++)
        {
          int count = transposition.Count(d => d == i);
          if (count > 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "transposition");
          else if (count == 0 && ranges != null && ranges.Length > 0 && ranges[i].Count != 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "ranges");
        }
      }
      if (lowerBounds != null && lowerBounds.Length != (transposition != null ? transposition.Length : sourceArrayInfo.Rank))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "lowerBounds");
      if (targetIndices != null && targetIndices.Length != (transposition != null ? transposition.Length : sourceArrayInfo.Rank))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "targetIndices");
      if (sourceIndices != null && sourceIndices.Length != sourceArrayInfo.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "sourceIndices");

      RegularArrayInfo targetArrayInfo = lowerBounds != null ?
        new RegularArrayInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray(), lowerBounds) :
        new RegularArrayInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray());
      if (range.HasValue && (range.Value.Index > targetArrayInfo.Length || range.Value.Count > targetArrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      Array target = range.HasValue ? Array.CreateInstance(typeof(TResult), range.Value.Count) : targetArrayInfo.CreateArray<TResult>();
      if (array.Length != 0)
      {
        int[] indices = new int[sourceArrayInfo.Rank];
        for (ArrayIndex targetArrayIndex = new ArrayIndex(targetArrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = zeroBased, AsRanges = false };
          targetArrayIndex.Carry == 0 && (!range.HasValue || targetArrayIndex.FlatIndex < range.Value.Index + range.Value.Count); targetArrayIndex++)
        {
          targetArrayIndex.DimIndices.Apply((t, i) =>
          {
            indices[transposition != null ? transposition[i] : i] = targetArrayIndex.GetDimIndex(i) - targetArrayInfo.GetLowerBound(i);
          });
          TSource source = sourceArrayInfo.GetValue<TSource>(array, true, true, indices);
          int sourceIndex = sourceArrayInfo.CalcFlatIndex(true, indices);
          if (sourceIndices != null)
            sourceArrayInfo.CalcDimIndices(sourceIndex, zeroBased, sourceIndices);
          if (targetIndices != null)
            targetArrayIndex.GetDimIndices(targetIndices);
          targetArrayIndex.SetValue<TResult>(target, converter(source, sourceIndex, sourceIndices, targetArrayIndex.FlatIndex, targetIndices));
        }
      }
			return target;
		}

    public static Array ConvertAsLongRegular<TSource, TResult>(this Array array, Func<TSource, TResult> converter, int[] transposition = null,
      LongRange? range = null, params LongRange[] ranges)
    {
      return array.ConvertAsLongRegular<TSource, TResult>((t, sfi, sdi, tfi, tdi) => converter(t), transposition, null, null, false, range, ranges);
    }

    public static Array ConvertAsLongRegular<TSource, TResult>(this Array array, Func<TSource, long, long[], long, long[], TResult> converter, int[] transposition = null,
      long[] targetIndices = null, long[] sourceIndices = null, bool zeroBased = false, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (converter == null)
				throw new ArgumentNullException("converter");

      Type type = array.GetType().GetElementType();
      if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);

      RegularArrayLongInfo sourceArrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (transposition != null)
      {
        if (transposition.Length == 0 || transposition.Length > sourceArrayInfo.Rank)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "transposition");
        if (transposition.Count(d => d < 0 || d >= sourceArrayInfo.Rank) > 0)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreArrayElementsOutOfRange], "transposition");
        for (int i = 0; i < sourceArrayInfo.Rank; i++)
        {
          int count = transposition.Count(d => d == i);
          if (count > 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "transposition");
          else if (count == 0 && ranges != null && ranges.Length > 0 && ranges[i].Count != 1L)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "ranges");
        }
      }
      if (targetIndices != null && targetIndices.Length != (transposition != null ? transposition.Length : sourceArrayInfo.Rank))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "targetIndices");
      if (sourceIndices != null && sourceIndices.Length != sourceArrayInfo.Rank)
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "sourceIndices");

      RegularArrayLongInfo targetArrayInfo = new RegularArrayLongInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray());
			Array target = targetArrayInfo.CreateArray<TResult>();
			if (array.LongLength != 0)
			{
        long[] indices = new long[sourceArrayInfo.Rank];
        for (ArrayLongIndex targetArrayIndex = new ArrayLongIndex(targetArrayInfo) { ZeroBased = zeroBased, AsRanges = false };
          targetArrayIndex.Carry == 0; targetArrayIndex++)
        {
          targetArrayIndex.DimIndices.Apply((t, i) =>
          {
            indices[transposition != null ? transposition[i] : i] = targetArrayIndex.GetLongDimIndex(i);
          });
          TSource source = sourceArrayInfo.GetValue<TSource>(array, true, true, indices);
          long sourceIndex = sourceArrayInfo.CalcFlatIndex(true, indices);
          if (sourceIndices != null)
            sourceArrayInfo.CalcDimIndices(sourceIndex, zeroBased, sourceIndices);
          if (targetIndices != null)
            targetArrayIndex.GetDimIndices(targetIndices);
          targetArrayIndex.SetValue<TResult>(target, converter(source, sourceIndex, sourceIndices, targetArrayIndex.FlatIndex, targetIndices));
        }
      }
			return target;
		}

    #endregion
    #region Range regular array

    public static Array RangeAsRegular(this Array array, int[] transposition = null, int[] lowerBounds = null, bool zeroBased = false, Range? range = null, params Range[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");
      if (range.HasValue && (range.Value.Index < 0 || range.Value.Count < 0))
        throw new ArgumentOutOfRangeException("Parameter is out of range", "range");

      RegularArrayInfo sourceArrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions(zeroBased, ranges));
      if (transposition != null)
      {
        if (transposition.Length == 0 || transposition.Length > sourceArrayInfo.Rank)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "transposition");
        if (transposition.Count(d => d < 0 || d >= sourceArrayInfo.Rank) > 0)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreArrayElementsOutOfRange], "transposition");
        for (int i = 0; i < sourceArrayInfo.Rank; i++)
        {
          int count = transposition.Count(d => d == i);
          if (count > 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "transposition");
          else if (count == 0 && ranges != null && ranges.Length > 0 && ranges[i].Count != 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "ranges");
        }
      }
      if (lowerBounds != null && lowerBounds.Length != (transposition != null ? transposition.Length : ranges.Length))
        throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "lowerBounds");

      RegularArrayInfo targetArrayInfo = lowerBounds != null ?
        new RegularArrayInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray(), lowerBounds) :
        new RegularArrayInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray());
      if (range.HasValue && (range.Value.Index > targetArrayInfo.Length || range.Value.Count > targetArrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      Array target = range.HasValue ? Array.CreateInstance(array.GetRegularArrayElementType(), range.Value.Count) : targetArrayInfo.CreateArray(array.GetRegularArrayElementType());
      if (array.Length != 0)
      {
        int[] sourceIndices = new int[sourceArrayInfo.Rank];
        if (transposition != null)
          sourceArrayInfo.GetLowerBounds(sourceIndices);
        int[] targetIndices = new int[targetArrayInfo.Rank];
        for (ArrayIndex targetArrayIndex = new ArrayIndex(targetArrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = true, AsRanges = false };
          targetArrayIndex.Carry == 0 && (!range.HasValue || targetArrayIndex.FlatIndex < range.Value.Index + range.Value.Count); targetArrayIndex++)
        {
          targetArrayIndex.GetDimIndices(targetIndices);
          targetIndices.Apply((t, i) =>
          {
            int j = transposition != null ? transposition[i] : i;
            sourceIndices[j] = sourceArrayInfo.GetLowerBound(j) + t;
          });
          if (range.HasValue)
            target.SetValue(array.GetValue(sourceIndices), targetArrayIndex.FlatIndex - range.Value.Index);
          else
            target.SetValue(array.GetValue(sourceIndices), targetIndices);
        }
      }
      return target;
    }

    public static Array RangeAsLongRegular(this Array array, int[] transposition = null, LongRange? range = null, params LongRange[] ranges)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (ranges == null)
        throw new ArgumentNullException("ranges");
      if (range.HasValue && (range.Value.Index < 0L || range.Value.Count < 0L))
        throw new ArgumentOutOfRangeException("Parameter is out of range.", "range");

      RegularArrayLongInfo sourceArrayInfo = new RegularArrayLongInfo(array.GetRegularArrayLongDimensions(ranges));
      if (transposition != null)
      {
        if (transposition.Length == 0 || transposition.Length > sourceArrayInfo.Rank)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "transposition");
        if (transposition.Count(d => d < 0 || d >= sourceArrayInfo.Rank) > 0)
          throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreArrayElementsOutOfRange], "transposition");
        for (int i = 0; i < sourceArrayInfo.Rank; i++)
        {
          int count = transposition.Count(d => d == i);
          if (count > 1)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "transposition");
          else if (count == 0 && ranges != null && ranges.Length > 0 && ranges[i].Count != 1L)
            throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.OneOrMoreInvalidArrayElements], "ranges");
        }
      }

      RegularArrayLongInfo targetArrayInfo = new RegularArrayLongInfo((transposition != null ? transposition.AsEnumerable() : Enumerable.Range(0, sourceArrayInfo.Rank)).Select(d => sourceArrayInfo.GetLength(d)).ToArray());
      if (range.HasValue && (range.Value.Index > targetArrayInfo.Length || range.Value.Count > targetArrayInfo.Length - range.Value.Index))
        throw new ArgumentException("Invalid parameter.", "range");

      Array target = range.HasValue ? Array.CreateInstance(array.GetRegularArrayElementType(), range.Value.Count) : targetArrayInfo.CreateArray(array.GetRegularArrayElementType());
      if (array.Length != 0)
      {
        long[] sourceIndices = new long[sourceArrayInfo.Rank];
        if (transposition != null)
          sourceArrayInfo.GetLowerBounds(sourceIndices);
        long[] targetIndices = new long[targetArrayInfo.Rank];
        for (ArrayLongIndex targetArrayIndex = new ArrayLongIndex(targetArrayInfo) { FlatIndex = range.HasValue ? range.Value.Index : 0, ZeroBased = false, AsRanges = false };
          targetArrayIndex.Carry == 0 && (!range.HasValue || targetArrayIndex.FlatIndex < range.Value.Index + range.Value.Count); targetArrayIndex++)
        {
          targetArrayIndex.GetDimIndices(targetIndices);
          targetIndices.Apply((t, i) =>
          {
            int j = transposition != null ? transposition[i] : i;
            sourceIndices[j] = sourceArrayInfo.GetLowerBound(j) + t;
          });
          if (range.HasValue)
            target.SetValue(array.GetValue(sourceIndices), targetArrayIndex.FlatIndex - range.Value.Index);
          else
            target.SetValue(array.GetValue(sourceIndices), targetIndices);
        }
      }
      return target;
    }

    #endregion
    #endregion
    #region Jagged array extensions
    #region Miscellaneous jagged extensions

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arrayType"></param>
    /// <returns></returns>
    public static bool IsJaggedArray(this Type arrayType)
		{
			if (arrayType == null)
				throw new ArgumentNullException("arrayType");
			else if (!arrayType.IsArray)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.TypeIsNotArray]);
			//
			return arrayType.GetElementType().IsArray;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static bool IsJaggedArray(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      //
      return array.GetType().IsJaggedArray();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="ranks"></param>
		/// <returns></returns>
		public static Type MakeJaggedArrayType(this Type elementType, int[] ranks)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			if (ranks == null)
				throw new ArgumentNullException("ranks");
			int rank = 0;
			for (int i = 0; i < ranks.Length; rank += ranks[i++])
				if (ranks[i] <= 0)
					throw new ArgumentRegularArrayElementException("ranks", "Value is out of range", i);
				else if (ranks[i] > int.MaxValue - rank)
					throw new ArgumentRegularArrayElementException("ranks", "Ranks sum is out of range", i);
			//
			for (int i = 0; i < ranks.Length; i++)
				elementType = ranks[ranks.Length - - 1] == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(ranks[ranks.Length - - 1]);
			return elementType;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="elementType"></param>
		/// <param name="ranks"></param>
		/// <returns></returns>
		public static Type[] MakeJaggedArrayTypes(this Type elementType, int[] ranks)
		{
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			if (ranks == null)
				throw new ArgumentNullException("ranks");
			int rank = 0;
			for (int i = 0; i < ranks.Length; rank += ranks[i++])
				if (ranks[i] <= 0)
					throw new ArgumentRegularArrayElementException("ranks", "Value is out of range", i);
				else if (ranks[i] > int.MaxValue - rank)
					throw new ArgumentRegularArrayElementException("ranks", "Ranks sum is out of range", i);
			//
			Type[] types = new Type[ranks.Length];
			for (int i = 0; i < ranks.Length; i++)
				types[ranks.Length - - 1] = elementType = ranks[ranks.Length - - 1] == 1 ? elementType.MakeArrayType() : elementType.MakeArrayType(ranks[ranks.Length - - 1]);
			return types;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Type GetJaggedArrayElementType(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      //
      Type type = array.GetType();
			while (type.IsArray)
				type = type.GetElementType();
			return type;
		}

		public static int[] GetJaggedArrayRanks(this Array array)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      //
      PwrList<int> ranks = new PwrList<int>();
			Type type = array.GetType();
			while (type.IsArray)
			{
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			return ranks.ToArray();
		}

		public static T[][] ToRankedArray<T>(this T[] array, int[] ranks)
		{
			if (array == null)
				throw new ArgumentNullException("flatArray");
			if (ranks == null)
				throw new ArgumentNullException("ranks");
			if (ranks.Any(r => r < 0))
				throw new ArgumentException("Array contains out of range value", "ranks");
			if (array.Length != ranks.Aggregate((left, right) => left + right))
				throw new ArgumentException("Invalitotarank", "ranks");

			int i = 0;
			return ranks.Select(r => { int s = i; i += r; return array.Skip(s).Take(r).ToArray(); }).ToArray();
		}

		public static T[] ToFlatArray<T>(this T[][] array)
		{
			return array.SelectMany(a => a).ToArray();
		}

		#endregion
		#region Enumerate jagged array

		/// <summary>
		/// Enumerate all elements of <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">Typof enumerateelements i<paramref name="array"/>.</typeparam>
		/// <param name="array">Jagged array tenumerate.</param>
		/// <param name="ranges"></param>
		/// <returns>Returns IEnumerable&lt;<typeparamref name="T"/>&gt;</returns>
		public static IEnumerable<T> EnumerateAsJagged<T>(this Array array, params Range[][] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      //
      PwrList<int> ranks = new PwrList<int>();
			Type t = array.GetType();
			while (t.IsArray)
			{
				ranks.Add(t.GetArrayRank());
				t = t.GetElementType();
			}
			if (typeof(T) != t && !typeof(T).IsSubclassOf(t))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			bool asRanges = false;
			if (ranges != null && ranges.Length != 0)
			{
				if (ranges.Length != ranks.Count)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "ranges");
				if (ranges.Where((r, i) => r == null || r.Length != ranks[i]).Any())
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement], "ranges");
				asRanges = true;
			}
			//
			int depth = 0;
			PwrStack<ArrayContext> arrayContexts = new PwrStack<ArrayContext>();
			ArrayIndex arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { AsRanges = asRanges };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.LongLength > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.LongLength == 0)
					break;
				arrayContexts.Push(new ArrayContext(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { AsRanges = asRanges };
				//depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					for (; arrayIndex.Carry == 0; arrayIndex++)
						yield return arrayIndex.GetValue<T>(array);
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts.Pop();
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		/// <summary>
		/// Enumerate all elements of <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">Typof enumerateelements i<paramref name="array"/>.</typeparam>
		/// <param name="array">Jagged array tenumerate.</param>
		/// <param name="ranges"></param>
		/// <returns>Returns IEnumerable&lt;<typeparamref name="T"/>&gt;</returns>
		public static IEnumerable<T> EnumerateAsJagged<T>(this Array array, params LongRange[][] ranges)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      //
      PwrList<int> ranks = new PwrList<int>();
			Type t = array.GetType();
			while (t.IsArray)
			{
				ranks.Add(t.GetArrayRank());
				t = t.GetElementType();
			}
			if (typeof(T) != t && !typeof(T).IsSubclassOf(t))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			bool asRanges = false;
			if (ranges != null && ranges.Length != 0)
			{
				if (ranges.Length != ranks.Count)
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayLength], "ranges");
				if (ranges.Where((r, i) => r == null || r.Length != ranks[i]).Any())
					throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElement], "ranges");
				asRanges = true;
			}
			//
			int depth = 0;
			PwrStack<ArrayContextLong> arrayContexts = new PwrStack<ArrayContextLong>();
			ArrayLongIndex arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions(ranges[depth]))) { AsRanges = asRanges };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.LongLength > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.LongLength == 0)
					break;
				arrayContexts.Push(new ArrayContextLong(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions(ranges[++depth]))) { AsRanges = asRanges };
				//depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					for (; arrayIndex.Carry == 0; arrayIndex++)
						yield return arrayIndex.GetValue<T>(array);
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts.Pop();
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		#endregion
		#region Clone jagged array

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Array CloneAsJagged<T>(this Array array)
		{
			return ConvertAsJagged<T, T>(array, v => v);
		}

		#endregion
		#region Format jagged array

		public static string FormatAsJagged<T>(this Array array, Func<T, string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int, string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int, int[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, dimIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int, int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int[], int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.None,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, int, int[], int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.None, itemFormatter, nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, dimIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int[], int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices, rankedIndices) : (Func<T, int, int[], int[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[], int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
			return FormatAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, itemFormatter, nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		/// <summary>
		/// Formaarray.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">Formatting array.</param>
		/// <param name="options">Jagged array options.</param>
		/// <param name="itemFormatter">Function that returns string representation of array item value. Parameters contaiarray item valuanjagged array item indices.</param>
		/// <param name="nullFormatter">Function that returns null array string representation. Parameter is jagged array item indices.</param>
		/// <param name="itemDelimiter">Function that returns item delimiter. Parameter is currenjagged array depth anregular array rank.</param>
		/// <param name="openBracket">Function that returns open bracket string. Parameter is currenjagged array depth anregular array rank.</param>
		/// <param name="closeBracket">Function that returns open bracket string. Parameter is currenjagged array depth anregular array rank.</param>
		/// <returns>Formattestring representation of array.</returns>
		private static string FormatAsJagged<T>(this Array array, JaggedArrayOptions options, Func<T, int, int[], int[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, int, string> itemDelimiter, Func<int, int, int, string> openBracket, Func<int, int, int, string> closeBracket)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (itemFormatter == null)
				throw new ArgumentNullException("itemFormatter");
			if (nullFormatter == null)
				throw new ArgumentNullException("nullFormatter");
			if (itemDelimiter == null)
				throw new ArgumentNullException("itemDelimiter");
			if (openBracket == null)
				throw new ArgumentNullException("openBracket");
			if (closeBracket == null)
				throw new ArgumentNullException("closeBracket");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			int brackets = 0;
			StringBuilder sb = new StringBuilder();
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			RegularArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
			ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (arrayInfo.Length == 0)
				{
					for (int i = 0; i < arrayInfo.Rank; i++)
						sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
					for (int i = array.Rank - 1; i >= 0; i--)
						sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					break;
				}
				//
				for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
				//
				if (arrayIndex.FlatIndex > 0)
					sb.Append(itemDelimiter(depth, arrayInfo.Rank - 1 - brackets, arrayInfo.GetLength(arrayInfo.Rank - 1 - brackets)));
				//
				for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
					sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
				//
				Array a = (arrayIndex.FlatIndex < arrayInfo.Length) ? arrayIndex.GetValue<Array>(array) : null;
				if (a == null)
				{
					//
					sb.Append(nullFormatter(depth));
					//
					for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
					for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
						sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					//
					if (arrayIndex.Inc())
						break;
					else
						continue;
				}
				//
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				array = a;
				arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
				arrayIndex = new ArrayIndex(arrayInfo) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				depth++;
				//
				if (depth == ranks.Count - 1)
				{
					if (arrayInfo.Length == 0)
					{
						for (int i = 0; i < arrayInfo.Rank; i++)
							sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
						for (int i = arrayInfo.Rank - 1; i >= 0; i--)
							sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					}
					else
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int i = 0; i < depth; i++)
								for (int j = 0; j < ranks[i]; j++)
									bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							for (int i = 0; i < depth; i++)
								arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
						//
						do
						{
							//
							if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
								for (int j = 0; j < ranks[depth]; j++)
									bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
							//
							if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
								arrayIndex.GetDimIndices(rankedIndices[depth]);
							//
							for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
							//
							if (arrayIndex.FlatIndex > 0)
								sb.Append(itemDelimiter(depth, arrayInfo.Rank - 1 - brackets, arrayInfo.GetLength(arrayInfo.Rank - 1 - brackets)));
							//
							for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
								sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
							//
							sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), flatIndex++, bandedIndices, rankedIndices));
							//
							for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
							for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
								sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
						}
						while (!arrayIndex.Inc());
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				arrayInfo = (RegularArrayInfo)arrayIndex.ArrayInfo;
				depth--;
				//
				for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
				for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
					sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
				//
				if (arrayIndex.FlatIndex < arrayIndex.ArrayInfo.Length - 1)
				{
					arrayIndex++;
					goto descent;
				}
				else
					goto ascent;
			}
			return sb.ToString();
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long, string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long, long[], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, dimIndices) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, rankedIndices) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long, long[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, flatIndex, rankedIndices) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long[], long[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.None,
				itemFormatter != null ? (item, flatIndex, dimIndices, rankedIndices) => itemFormatter(item, dimIndices, rankedIndices) : (Func<T, long, long[], long[][], string>)null,
				nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		public static string FormatAsJagged<T>(this Array array, Func<T, long, long[], long[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
			return FormatAsJagged<T>(array, JaggedArrayOptions.None, itemFormatter, nullFormatter, itemDelimiter, openBracket, closeBracket);
		}

		/// <summary>
		/// Formaarray.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array">Formatting array.</param>
		/// <param name="options">Jagged array options.</param>
		/// <param name="itemFormatter">Function that returns string representation of array item value. Parameters contaiarray item valuanjagged array item indices.</param>
		/// <param name="nullFormatter">Function that returns null array string representation. Parameter is jagged array item indices.</param>
		/// <param name="itemDelimiter">Function that returns item delimiter. Parameter is currenjagged array depth anregular array rank.</param>
		/// <param name="openBracket">Function that returns open bracket string. Parameter is currenjagged array depth anregular array rank.</param>
		/// <param name="closeBracket">Function that returns open bracket string. Parameter is currenjagged array depth anregular array rank.</param>
		/// <returns>Formattestring representation of array.</returns>
		private static string FormatAsJagged<T>(this Array array, JaggedArrayOptions options, Func<T, long, long[], long[][], string> itemFormatter,
			Func<int, string> nullFormatter, Func<int, int, long, string> itemDelimiter, Func<int, int, long, string> openBracket, Func<int, int, long, string> closeBracket)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (itemFormatter == null)
				throw new ArgumentNullException("itemFormatter");
			if (nullFormatter == null)
				throw new ArgumentNullException("nullFormatter");
			if (itemDelimiter == null)
				throw new ArgumentNullException("itemDelimiter");
			if (openBracket == null)
				throw new ArgumentNullException("openBracket");
			if (closeBracket == null)
				throw new ArgumentNullException("closeBracket");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			int brackets = 0;
			StringBuilder sb = new StringBuilder();
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			RegularArrayLongInfo arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
			ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo);
		descent:
			while (depth < ranks.Count - 1)
			{
				if (arrayInfo.Length == 0)
				{
					for (int i = 0; i < arrayInfo.Rank; i++)
						sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
					for (int i = array.Rank - 1; i >= 0; i--)
						sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					break;
				}
				//
				for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
				//
				if (arrayIndex.FlatIndex > 0)
					sb.Append(itemDelimiter(depth, arrayInfo.Rank - 1 - brackets, arrayInfo.GetLength(arrayInfo.Rank - 1 - brackets)));
				//
				for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
					sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
				//
				Array a = (arrayIndex.FlatIndex < arrayInfo.Length) ? arrayIndex.GetValue<Array>(array) : null;
				if (a == null)
				{
					//
					sb.Append(nullFormatter(depth));
					//
					for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
					for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
						sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					//
					if (arrayIndex.Inc())
						break;
					else
						continue;
				}
				//
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				array = a;
				arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
				arrayIndex = new ArrayLongIndex(arrayInfo);
				depth++;
				//
				if (depth == ranks.Count - 1)
				{
					if (arrayInfo.Length == 0)
					{
						for (int i = 0; i < array.Rank; i++)
							sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
						for (int i = arrayInfo.Rank - 1; i >= 0; i--)
							sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
					}
					else
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int i = 0; i < depth; i++)
								for (int j = 0; j < ranks[i]; j++)
									bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							for (int i = 0; i < depth; i++)
								arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
						//
						do
						{
							//
							if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
								for (int j = 0; j < ranks[depth]; j++)
									bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
							//
							if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
								arrayIndex.GetDimIndices(rankedIndices[depth]);
							//
							for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.LowerBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
							//
							if (arrayIndex.FlatIndex > 0)
								sb.Append(itemDelimiter(depth, arrayInfo.Rank - 1 - brackets, arrayInfo.GetLength(arrayInfo.Rank - 1 - brackets)));
							//
							for (int i = arrayInfo.Rank - brackets; brackets > 0; i++, brackets--)
								sb.Append(openBracket(depth, i, arrayInfo.GetLength(i)));
							//
							sb.Append(itemFormatter(arrayIndex.GetValue<T>(array), flatIndex++, bandedIndices, rankedIndices));
							//
							for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
							for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
								sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
						}
						while (!arrayIndex.Inc());
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				arrayInfo = (RegularArrayLongInfo )arrayIndex.ArrayInfo;
				depth--;
				//
				for (brackets = 0; brackets < arrayInfo.Rank && arrayIndex.DimIndices[arrayInfo.Rank - 1 - brackets] == arrayInfo.UpperBounds[arrayInfo.Rank - 1 - brackets]; brackets++) ;
				for (int i = arrayInfo.Rank - 1; brackets > 0; i--, brackets--)
					sb.Append(closeBracket(depth, i, arrayInfo.GetLength(i)));
				//
				
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
			return sb.ToString();
		}

		#endregion
		#region Select jagged array

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int, TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int[], int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.None,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.None, selector);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[], int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[], int[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, selector);
		}

		private static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, JaggedArrayOptions options, Func<TSource, int, int[], int[][], TResult> selector)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (selector == null)
				throw new ArgumentNullException("selector");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			ArrayIndex arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						yield return selector(arrayIndex.GetValue<TSource>(array), flatIndex++, bandedIndices, rankedIndices);
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long, TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, dimIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, flatIndex, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long[], long[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.None,
				selector != null ? (item, flatIndex, dimIndices, rankedIndices) => selector(item, dimIndices, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[], long[][], TResult> selector)
		{
			return SelectAsJagged<TSource, TResult>(array, JaggedArrayOptions.None, selector);
		}

		private static IEnumerable<TResult> SelectAsJagged<TSource, TResult>(this Array array, JaggedArrayOptions options, Func<TSource, long, long[], long[][], TResult> selector)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (selector == null)
				throw new ArgumentNullException("selector");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			ArrayLongIndex arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						yield return selector(arrayIndex.GetValue<TSource>(array), flatIndex++, bandedIndices, rankedIndices);
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		#endregion
		#region Where jagged array

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int, bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int, int[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, dimIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int, int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int[], int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.None,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, int, int[], int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.None, predicate);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, dimIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int[], int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices, rankedIndices) : (Func<T, int, int[], int[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, bool zeroBased, Func<T, int, int[], int[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, predicate);
		}

		private static IEnumerable<T> WhereAsJagged<T>(this Array array, JaggedArrayOptions options, Func<T, int, int[], int[][], bool> predicate)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (predicate == null)
				throw new ArgumentNullException("predicate");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> offsets = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				offsets.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			ArrayIndex arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[offsets[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[offsets[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						T value = arrayIndex.GetValue<T>(array);
						//
						if (predicate(value, flatIndex++, bandedIndices, rankedIndices))
							yield return value ;
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long, bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long, long[], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, dimIndices) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, rankedIndices) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long, long[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, flatIndex, rankedIndices) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long[], long[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.None,
				predicate != null ? (item, flatIndex, dimIndices, rankedIndices) => predicate(item, dimIndices, rankedIndices) : (Func<T, long, long[], long[][], bool>)null);
		}

		public static IEnumerable<T> WhereAsJagged<T>(this Array array, Func<T, long, long[], long[][], bool> predicate)
		{
			return WhereAsJagged<T>(array, JaggedArrayOptions.None, predicate);
		}

		private static IEnumerable<T> WhereAsJagged<T>(this Array array, JaggedArrayOptions options, Func<T, long, long[], long[][], bool> predicate)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (predicate == null)
				throw new ArgumentNullException("predicate");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			ArrayLongIndex arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						T value = arrayIndex.GetValue<T>(array);
						//
						if (predicate(value, flatIndex++, bandedIndices, rankedIndices))
							yield return value ;
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		#endregion
		#region Fill jagged array

		public static void FillAsJagged<T>(this Array array, T value)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				(long flatIndex, long[] dimIndices, long[][] rankedIndices) => value);
		}

		public static void FillAsJagged<T>(this Array array, Func<T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator() : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int, T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int, int[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, dimIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int, int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int[], int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.None,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices, rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<int, int[], int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.None, valuator);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int, int[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, dimIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int, int[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int[], int[][], T> valuator)
		{
			FillAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices, rankedIndices) : (Func<int, int[], int[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, bool zeroBased, Func<int, int[], int[][], T> valuator)
		{
			FillAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, valuator);
		}

		private static void FillAsJagged<T>(this Array array, JaggedArrayOptions options, Func<int, int[], int[][], T> valuator)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			ArrayIndex arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						arrayIndex.SetValue<T>(array, valuator(flatIndex++, bandedIndices, rankedIndices));
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		public static void FillAsJagged<T>(this Array array, Func<long, T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long, long[], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, dimIndices) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(rankedIndices) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long, long[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(flatIndex, rankedIndices) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long[], long[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.None,
				valuator != null ? (flatIndex, dimIndices, rankedIndices) => valuator(dimIndices, rankedIndices) : (Func<long, long[], long[][], T>)null);
		}

		public static void FillAsJagged<T>(this Array array, Func<long, long[], long[][], T> valuator)
		{
			FillAsJagged<T>(array, JaggedArrayOptions.None, valuator);
		}

		private static void FillAsJagged<T>(this Array array, JaggedArrayOptions options, Func<long, long[], long[][], T> valuator)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			ArrayLongIndex arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						arrayIndex.SetValue<T>(array, valuator(flatIndex++, bandedIndices, rankedIndices));
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		#endregion
		#region Apply jagged array

		public static void ApplyAsJagged<T>(this Array array, Action<T> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int, int[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, dimIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int, int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int[], int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.None,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, int, int[], int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.None, action);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int, int[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, dimIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int, int[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int[], int[][]> action)
		{
			ApplyAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices, rankedIndices) : (Action<T, int, int[], int[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, bool zeroBased, Action<T, int, int[], int[][]> action)
		{
			ApplyAsJagged<T>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, action);
		}

		private static void ApplyAsJagged<T>(this Array array, JaggedArrayOptions options, Action<T, int, int[], int[][]> action)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			ArrayIndex arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayIndex(new RegularArrayInfo(array.GetRegularArrayDimensions())) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						action(arrayIndex.GetValue<T>(array), flatIndex++, bandedIndices, rankedIndices);
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long, long[]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressRankedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, dimIndices) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, rankedIndices) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long, long[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.SuppressBandedIndices,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, flatIndex, rankedIndices) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long[], long[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.None,
				action != null ? (item, flatIndex, dimIndices, rankedIndices) => action(item, dimIndices, rankedIndices) : (Action<T, long, long[], long[][]>)null);
		}

		public static void ApplyAsJagged<T>(this Array array, Action<T, long, long[], long[][]> action)
		{
			ApplyAsJagged<T>(array, JaggedArrayOptions.None, action);
		}

		private static void ApplyAsJagged<T>(this Array array, JaggedArrayOptions options, Action<T, long, long[], long[][]> action)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(T) != type && !typeof(T).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			ArrayLongIndex arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				array = arrayIndex.GetValue<Array>(array);
				arrayIndex = new ArrayLongIndex(new RegularArrayLongInfo (array.GetRegularArrayLongDimensions()));
				depth++;
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						action(arrayIndex.GetValue<T>(array), flatIndex++, bandedIndices, rankedIndices);
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
		}

		#endregion
		#region Convert jagged array

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int, TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int[], int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.None,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, int, int[], int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.None, convertor);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, dimIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | (zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None),
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int[], int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices, rankedIndices) : (Func<TSource, int, int[], int[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, bool zeroBased, Func<TSource, int, int[], int[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, zeroBased ? JaggedArrayOptions.ZeroBasedIndices : JaggedArrayOptions.None, convertor);
		}

		private static Array ConvertAsJagged<TSource, TResult>(this Array array, JaggedArrayOptions options, Func<TSource, int, int[], int[][], TResult> converter)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (converter == null)
				throw new ArgumentNullException("converter");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			Type[] targetArrayTypes = new Type[ranks.Count];
			targetArrayTypes[ranks.Count - 1] = typeof(TResult);
			for (int i = 1; i < ranks.Count; i++)
				targetArrayTypes[ranks.Count - - 1] = ranks[ranks.Count - i] == 1 ? targetArrayTypes[ranks.Count - i].MakeArrayType() : targetArrayTypes[ranks.Count - i].MakeArrayType(ranks[ranks.Count - i]);
			//
			int flatIndex = 0;
			int[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new int[rank] : null;
			int[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new int[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new int[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContext> arrayContexts = new PwrList<ArrayContext>();
			PwrStack<Array> targetArrayContexts = new PwrStack<Array>();
			ArrayInfo arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
			ArrayIndex arrayIndex = new ArrayIndex(arrayInfo) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
			Array targetArray = arrayInfo.CreateArray(targetArrayTypes[depth]);
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContext(array, arrayIndex));
				targetArrayContexts.Push(targetArray);
				array = arrayIndex.GetValue<Array>(array);
				arrayInfo = new RegularArrayInfo(array.GetRegularArrayDimensions());
				Array t = arrayInfo.CreateArray(targetArrayTypes[++depth]);
				arrayIndex.SetValue<Array>(targetArray, t);
				targetArray = t;
				arrayIndex = new ArrayIndex(arrayInfo) { ZeroBased = (options & JaggedArrayOptions.ZeroBasedIndices) != 0 };
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						arrayIndex.SetValue<TResult>(targetArray, converter(arrayIndex.GetValue<TSource>(array), flatIndex++, bandedIndices, rankedIndices));
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContext arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				targetArray = targetArrayContexts.Pop();
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
			return targetArray;
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long, TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices | JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressRankedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, dimIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.SuppressBandedIndices,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, flatIndex, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long[], long[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.None,
				convertor != null ? (item, flatIndex, dimIndices, rankedIndices) => convertor(item, dimIndices, rankedIndices) : (Func<TSource, long, long[], long[][], TResult>)null);
		}

		public static Array ConvertAsJagged<TSource, TResult>(this Array array, Func<TSource, long, long[], long[][], TResult> convertor)
		{
			return ConvertAsJagged<TSource, TResult>(array, JaggedArrayOptions.None, convertor);
		}

		private static Array ConvertAsJagged<TSource, TResult>(this Array array, JaggedArrayOptions options, Func<TSource, long, long[], long[][], TResult> converter)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (converter == null)
				throw new ArgumentNullException("converter");
			//
			PwrList<int> ranks = new PwrList<int>();
			PwrList<int> biases = new PwrList<int>();
			int rank = 0;
			Type type = array.GetType();
			for (int i = 0; type.IsArray; i++)
			{
				biases.Add(rank);
				rank += type.GetArrayRank();
				ranks.Add(type.GetArrayRank());
				type = type.GetElementType();
			}
			//
			if (typeof(TSource) != type && !typeof(TSource).IsSubclassOf(type))
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.InvalidArrayElementType]);
			//
			Type[] targetArrayTypes = new Type[ranks.Count];
			targetArrayTypes[ranks.Count - 1] = typeof(TResult);
			for (int i = 1; i < ranks.Count; i++)
				targetArrayTypes[ranks.Count - - 1] = ranks[ranks.Count - i] == 1 ? targetArrayTypes[ranks.Count - i].MakeArrayType() : targetArrayTypes[ranks.Count - i].MakeArrayType(ranks[ranks.Count - i]);
			//
			long flatIndex = 0;
			long[] bandedIndices = (options & JaggedArrayOptions.SuppressBandedIndices) == 0 ? new long[rank] : null;
			long[][] rankedIndices = (options & JaggedArrayOptions.SuppressRankedIndices) == 0 ? new long[ranks.Count][] : null;
			if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
				for (int i = 0; i < ranks.Count; i++)
					rankedIndices[i] = new long[ranks[i]];
			//
			int depth = 0;
			PwrList<ArrayContextLong> arrayContexts = new PwrList<ArrayContextLong>();
			PwrStack<Array> targetArrayContexts = new PwrStack<Array>();
			ArrayLongInfo arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
			ArrayLongIndex arrayIndex = new ArrayLongIndex(arrayInfo);
			Array targetArray = arrayInfo.CreateArray(targetArrayTypes[depth]);
		descent:
			while (depth < ranks.Count - 1)
			{
				if (array.Length > 0)
					for (; arrayIndex.Carry == 0 && arrayIndex.GetValue<Array>(array) == null; arrayIndex++) ;
				if (arrayIndex.Carry != 0 || array.Length == 0)
					break;
				arrayContexts.Add(new ArrayContextLong(array, arrayIndex));
				targetArrayContexts.Push(targetArray);
				array = arrayIndex.GetValue<Array>(array);
				arrayInfo = new RegularArrayLongInfo (array.GetRegularArrayLongDimensions());
				Array t = arrayInfo.CreateArray(targetArrayTypes[++depth]);
				arrayIndex.SetValue<Array>(targetArray, t);
				targetArray = t;
				arrayIndex = new ArrayLongIndex(arrayInfo);
				if (depth == ranks.Count - 1 && array.Length != 0)
				{
					//
					if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
						for (int i = 0; i < depth; i++)
							for (int j = 0; j < ranks[i]; j++)
								bandedIndices[biases[i] + j] = arrayContexts[i].Index.DimIndices[j];
					//
					if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
						for (int i = 0; i < depth; i++)
							arrayContexts[i].Index.GetDimIndices(rankedIndices[i]);
					//
					for (; arrayIndex.Carry == 0; arrayIndex++)
					{
						//
						if ((options & JaggedArrayOptions.SuppressBandedIndices) == 0)
							for (int j = 0; j < ranks[depth]; j++)
								bandedIndices[biases[depth] + j] = arrayIndex.DimIndices[j];
						//
						if ((options & JaggedArrayOptions.SuppressRankedIndices) == 0)
							arrayIndex.GetDimIndices(rankedIndices[depth]);
						//
						arrayIndex.SetValue<TResult>(targetArray, converter(arrayIndex.GetValue<TSource>(array), flatIndex++, bandedIndices, rankedIndices));
					}
				}
			}
		ascent:
			if (depth != 0)
			{
				ArrayContextLong arrayContext = arrayContexts[arrayContexts.Count - 1];
				arrayContexts.RemoveAt(arrayContexts.Count - 1);
				array = arrayContext.Array;
				arrayIndex = arrayContext.Index;
				targetArray = targetArrayContexts.Pop();
				depth--;
				if (arrayIndex.IsMax)
					goto ascent;
				else
				{
					arrayIndex++;
					goto descent;
				}
			}
			return targetArray;
		}

		#endregion
		#region Embedded types

		/// <summary>
		/// 
		/// </summary>
		[Flags]
		private enum JaggedArrayOptions
		{
			None = 0,
			ZeroBasedIndices = 1,
			SuppressBandedIndices = 2,
			SuppressRankedIndices = 4
		}

		/// <summary>
		/// Array context
		/// </summary>
		private class ArrayContext
		{
			public ArrayContext(Array array, ArrayIndex index)
			{
				Array = array;
				Index = index;
			}

			public Array Array { get; private set; }

			public ArrayIndex Index { get; private set; }
		}

		/// <summary>
		/// Array contexwith addressing by long
		/// </summary>
		private class ArrayContextLong
		{
			public ArrayContextLong(Array array, ArrayLongIndex index)
			{
				Array = array;
				Index = index;
			}

			public Array Array { get; private set; }

			public ArrayLongIndex Index { get; private set; }
		}

		#endregion
		#endregion
/*
		#region General array extensions
		#region Enumerate general array

		/// <summary>
		/// Enumerate all elements of <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">Typof enumerateelements i<paramref name="array"/>.</typeparam>
		/// <param name="array">Jagged array tenumerate.</param>
		/// <param name="ranges"></param>
		/// <returns>Returns IEnumerable&lt;<typeparamref name="T"/>&gt;</returns>
		public static IEnumerable<T> Enumerate<T>(this Array array, params Range[] ranges)
		{
			return array.IsJaggedArray() ? 
				array.EnumerateAsJagged<T>(ranges != null && ranges.Length > 0 ? ranges.ToRankedArray(array.GetJaggedArrayRanks()) : null) : array.EnumerateAsRegular<T>(ranges);
		}

		/// <summary>
		/// Enumerate all elements of <paramref name="array"/>.
		/// </summary>
		/// <typeparam name="T">Typof enumerateelements i<paramref name="array"/>.</typeparam>
		/// <param name="array">Jagged array tenumerate.</param>
		/// <param name="ranges"></param>
		/// <returns>Returns IEnumerable&lt;<typeparamref name="T"/>&gt;</returns>
		public static IEnumerable<T> EnumerateLong<T>(this Array array, params LongRange[] ranges)
		{
			return array.IsJaggedArray() ? array.EnumerateAsJagged<T>(ranges != null && ranges.Length > 0 ? ranges.ToRankedArray(array.GetJaggedArrayRanks()) : null) : array.EnumerateAsLongRegular<T>(ranges);
		}

		#endregion
		#region Clone general array

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Array Clone<T>(this Array array)
		{
			return array.IsJaggedArray() ? array.CloneAsJagged<T>() : array.CloneAsRegular<T>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static Array CloneLong<T>(this Array array)
		{
			return array.IsJaggedArray() ? array.CloneAsJagged<T>() : array.CloneAsLongRegular<T>();
		}

		#endregion
		#region Select general array

		public static IEnumerable<TResult> Select<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> selector, int[] indices, bool zeroBased)
		{
			return array.IsJaggedArray() ?
				SelectAsJagged<TSource, TResult>(array, zeroBased, selector) :
				SelectAsRegular<TSource, TResult>(array, zeroBased: zeroBased, dimIndices: indices, selector: selector);
		}

		#endregion
		#region Where general array

		public static IEnumerable<T> Where<T>(this Array array, bool zeroBased, int[] indices, Func<T, int, int[], bool> predicate)
    {
			return array.IsJaggedArray() ?
				WhereAsJagged<T>(array, zeroBased, predicate) :
				WhereAsRegular<T>(array, zeroBased: zeroBased, indices: indices, predicate: predicate);
		}

		#endregion
		#region Fill general array

		public static void Fill<T>(this Array array, Func<int, int[], T> valuator, int[] indices, bool zeroBased)
		{
			if (array.IsJaggedArray())
				FillAsJagged<T>(array, zeroBased, valuator);
			else
				FillAsRegular<T>(array, zeroBased: zeroBased, indices: indices, valuator: valuator);
		}

		#endregion
		#region Apply general array

		public static void Apply<T>(this Array array, Action<T, int, int[]> action, int[] indices, bool zeroBased)
		{
			if (array.IsJaggedArray())
				ApplyAsJagged<T>(array, zeroBased, action);
			else
				ApplyAsRegular<T>(array, zeroBased: zeroBased, indices: indices, action: action);
		}

		#endregion
		#region Convert general array

		//public static Array Convert<TSource, TResult>(this Array array, Func<TSource, int, int[], TResult> converter, bool zeroBased)
		//{
		//	return array.IsJaggedArray() ?
		//		ConvertAsJagged<TSource, TResult>(array, zeroBased, converter) :
		//		ConvertAsRegular<TSource, TResult>(array, converter, );
		//}

		#endregion
		#endregion
*/
		#region Flat array extensions
		#region Validation methods

		public static void ValidateRange<T>(this T[] array, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");
		}

		public static void ValidateRange<T>(this T[] array, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("Index is out of range", "range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("Count is out of range", "range");
		}

		#endregion
		#region Apply methods

		public static void Apply<T>(this T[] array, Action<T> action)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");

			for (int i = 0; i < array.Length; i++)
				action(array[i]);
		}

		public static void Apply<T>(this T[] array, Action<T, int> action)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");

			for (int i = 0; i < array.Length; i++)
				action(array[i], i);
		}

		public static void Apply<T>(this T[] array, Action<T> action, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");

			for (; count > 0; count--)
				action(array[index++]);
		}

		public static void Apply<T>(this T[] array, Action<T, int> action, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");

			for (; count > 0; index++, count--)
				action(array[index], index);
		}

		public static void Apply<T>(this T[] array, Action<T> action, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("range", "Index is out of range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("range", "Count is out of range");

			for (int i = range.Index, c = range.Count; c > 0; i++, c--)
				action(array[i]);
		}

		public static void Apply<T>(this T[] array, Action<T, int> action, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (action == null)
				throw new ArgumentNullException("action");
			if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("range", "Index is out of range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("range", "Count is out of range");

			for (int i = range.Index, c = range.Count; c > 0; i++, c--)
				action(array[i], i);
		}

    #endregion
    #region Fill methods

    public static void Fill<T>(this T[] array, T value)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      for (int i = 0; i < array.Length; i++)
        array[i] = value ;
    }

    public static void Fill<T>(this T[] array, Func<T> valuator)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");

			for (int i = 0; i < array.Length; i++)
				array[i] = valuator();
		}

		public static void Fill<T>(this T[] array, Func<int, T> valuator)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");

			for (int i = 0; i < array.Length; i++)
				array[i] = valuator(i);
		}

    public static void Fill<T>(this T[] array, T value, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count");

      for (; count > 0; count--)
        array[index++] = value ;
    }

    public static void Fill<T>(this T[] array, Func<T> valuator, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");

			for (; count > 0; count--)
				array[index++] = valuator();
		}

		public static void Fill<T>(this T[] array, Func<int, T> valuator, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException("index");
			if (count < 0 || count > array.Length - index)
				throw new ArgumentOutOfRangeException("count");

			for (; count > 0; index++, count--)
				array[index] = valuator(index);
		}

    public static void Fill<T>(this T[] array, T value, Range range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.Index < 0 || range.Index > array.Length)
        throw new ArgumentOutOfRangeException("range", "Index is out of range");
      if (range.Count < 0 || range.Count > array.Length - range.Index)
        throw new ArgumentOutOfRangeException("range", "Count is out of range");

      for (int i = range.Index, c = range.Count; c > 0; i++, c--)
        array[i] = value ;
    }

    public static void Fill<T>(this T[] array, Func<T> valuator, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("range", "Index is out of range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("range", "Count is out of range");

			for (int i = range.Index, c = range.Count; c > 0; i++, c--)
				array[i] = valuator();
		}

		public static void Fill<T>(this T[] array, Func<int, T> valuator, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (valuator == null)
				throw new ArgumentNullException("valuator");
			if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("range", "Index is out of range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("range", "Count is out of range");

			for (int i = range.Index, c = range.Count; c > 0; i++, c--)
				array[i] = valuator(i);
		}

    #endregion
    #region Clear methods

    public static void Clear<T>(this T[] array)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      Array.Clear(array, array.GetLowerBound(0), array.Length);
    }

    public static void Clear<T>(this T[] array, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      Array.Clear(array, index, count);
    }

    public static void Clear<T>(this T[] array, Range range)
    {
      if (array == null)
        throw new ArgumentNullException("array");

      Array.Clear(array, range.Index, range.Index);
    }

    #endregion
    #region Reverse methods

    public static void Reverse<T>(this T[] array)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      Array.Reverse(array);
		}

    public static void Reverse<T>(this T[] array, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index", "Index is out of range");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count", "Count is out of range");

      Array.Reverse(array, index, count);
    }

    public static void Reverse<T>(this T[] array, Range range)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (range.Index < 0 || range.Index > array.Length)
				throw new ArgumentOutOfRangeException("range", "Index is out of range");
			if (range.Count < 0 || range.Count > array.Length - range.Index)
				throw new ArgumentOutOfRangeException("range", "Count is out of range");

      Array.Reverse(array, range.Index, range.Count);
    }

    #endregion
    #region Sort methods

    public static void Sort<T>(this T[] array, Comparison<T> comparison)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparison == null)
        throw new ArgumentNullException("comparison");

      Array.Sort(array, comparison);
    }

    public static void Sort<T>(this T[] array, Comparison<T> comparison, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparison == null)
        throw new ArgumentNullException("comparison");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index", "Index is out of range");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count", "Count is out of range");

      Array.Sort(array, index, count, new CustomComparer<T>(comparison));
    }

    public static void Sort<T>(this T[] array, Comparison<T> comparison, Range range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparison == null)
        throw new ArgumentNullException("comparison");
      if (range.Index < 0 || range.Index > array.Length)
        throw new ArgumentOutOfRangeException("range", "Index is out of range");
      if (range.Count < 0 || range.Count > array.Length - range.Index)
        throw new ArgumentOutOfRangeException("range", "Count is out of range");

      Array.Sort(array, range.Index, range.Count, new CustomComparer<T>(comparison));
    }

    public static void Sort<T>(this T[] array, IComparer<T> comparer)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparer == null)
        throw new ArgumentNullException("comparer");

      Array.Sort(array, comparer);
    }

    public static void Sort<T>(this T[] array, IComparer<T> comparer, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index", "Index is out of range");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count", "Count is out of range");

      Array.Sort(array, index, count, comparer);
    }

    public static void Sort<T>(this T[] array, IComparer<T> comparer, Range range)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (comparer == null)
        throw new ArgumentNullException("comparer");
      if (range.Index < 0 || range.Index > array.Length)
        throw new ArgumentOutOfRangeException("range", "Index is out of range");
      if (range.Count < 0 || range.Count > array.Length - range.Index)
        throw new ArgumentOutOfRangeException("range", "Count is out of range");

      Array.Sort(array, range.Index, range.Count, comparer);
    }

    #endregion
    #region Parse methods

    public static T[] Parse<T>(string s, Func<string, T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars)
    {
      return Parse(s, (v, i) => itemParser(v), itemPattern, delimitChars, spaceChars, escapeChars);
    }

    public static T[] Parse<T>(string s, Func<string, int, T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      if (itemParser == null)
        throw new ArgumentNullException("itemParser");
      if (delimitChars == null)
        throw new ArgumentNullException("delimitChars");

      s = s.Trim(spaceChars);
      if (s == string.Empty)
        return new T[0];
      string splitter = string.Format(arrayDelimiterFormat,
        Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
        spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty);
      Regex split = new Regex(splitter, RegexOptions.Multiline);
      return split.Split(s).Select((t, i) => itemParser(t, i)).ToArray();
    }

    public static T[] Parse<T>(string s, Func<string, T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars, char[] openingChars, char[] closingChars)
    {
      return Parse(s, (v, i) => itemParser(v), itemPattern, delimitChars, spaceChars, escapeChars, openingChars, closingChars);
    }

    public static T[] Parse<T>(string s, Func<string, int, T> itemParser, string itemPattern, char[] delimitChars, char[] spaceChars, char[] escapeChars, char[] openingChars, char[] closingChars)
    {
      if (s == null)
        throw new ArgumentNullException("s");
      if (itemParser == null)
        throw new ArgumentNullException("itemParser");
      if (delimitChars == null)
        throw new ArgumentNullException("delimitChars");
      if (openingChars == null)
        throw new ArgumentNullException("openingChars");
      if (closingChars == null)
        throw new ArgumentNullException("closingChars");

      string pattern = string.Format(arrayPatternFormat,
        !string.IsNullOrEmpty(itemPattern) ? itemPattern : string.Format(arrayItemsFormat,
          Regex.Escape(new string(delimitChars)),
          spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty,
          Regex.Escape(new string(openingChars)).Escape('\\', false, ']', '}'), Regex.Escape(new string(closingChars)).Escape('\\', false, ']', '}')),
        Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
        spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty,
        Regex.Escape(new string(openingChars)).Escape('\\', false, ']', '}'), Regex.Escape(new string(closingChars)).Escape('\\', false, ']', '}'));
      Match match = Regex.Match(s, pattern, RegexOptions.Multiline);
      if (!match.Success)
        return null;
      if (match.Groups["Items"].Captures[0].Value == string.Empty)
        return new T[0];
      string splitter = string.Format(arrayDelimiterFormat,
        Regex.Escape(new string(delimitChars)).Escape('\\', false, ']', '}'),
        spaceChars != null ? Regex.Escape(new string(spaceChars)).Escape('\\', false, ']', '}') : string.Empty, escapeChars != null ? Regex.Escape(new string(escapeChars)).Escape('\\', false, ']', '}') : string.Empty);
      Regex split = new Regex(splitter, RegexOptions.Multiline);
      return split.Split(match.Groups["Items"].Captures[0].Value).Select((t, i) => itemParser(t, i)).ToArray();
    }

    #endregion
    #region Format methods

    public static string Format<T>(this T[] array, Func<T, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket)
    {
      return Format(array, itemFormatter, itemDelimiter, openBracket, closeBracket, 0, array.Length);
    }

    public static string Format<T>(this T[] array, Func<T, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket, Range range)
    {
      return Format(array, itemFormatter, itemDelimiter, openBracket, closeBracket, range.Index, range.Count);
    }

    public static string Format<T>(this T[] array, Func<T, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (itemFormatter == null)
        throw new ArgumentNullException("itemFormatter");

      return Format<T>(array, (t, i) => itemFormatter(t), itemDelimiter, openBracket, closeBracket, index, count);
    }

    public static string Format<T>(this T[] array, Func<T, int, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket)
    {
      return Format(array, itemFormatter, itemDelimiter, openBracket, closeBracket, 0, array.Length);
    }

    public static string Format<T>(this T[] array, Func<T, int, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket, Range range)
    {
      return Format(array, itemFormatter, itemDelimiter, openBracket, closeBracket, range.Index, range.Count);
    }

    public static string Format<T>(this T[] array, Func<T, int, string> itemFormatter, string itemDelimiter, string openBracket, string closeBracket, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count");
      if (itemFormatter == null)
        throw new ArgumentNullException("itemFormatter");

      StringBuilder sb = new StringBuilder();
      if (openBracket != null)
        sb.Append(openBracket);
      for (int i = 0; i < count; i++)
      {
        if (i > 0 && itemDelimiter != null)
          sb.Append(itemDelimiter);
        sb.Append(itemFormatter(array[index + i], index + i));
      }
      if (closeBracket != null)
        sb.Append(closeBracket);
      return sb.ToString();
    }

    #endregion
    #region Manipulation methods

    public static T[] Insert<T>(this T[] array, int index, T[] value)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (value == null)
        throw new ArgumentNullException("value ");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (value.Length > int.MaxValue - array.Length)
        throw new ArgumentException("Tobig resularray.");

      T[] result = new T[array.Length + value.Length];
      if (index > 0)
        Array.Copy(array, 0, result, 0, index);
      Array.Copy(value, 0, result, index, value.Length);
      if (index < array.Length)
        Array.Copy(array, index, result, index + value.Length, array.Length - index);
      return result;
    }

    public static T[] Remove<T>(this T[] array, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count");

      T[] result = new T[array.Length - count];
      if (index > 0)
        Array.Copy(array, 0, result, 0, index);
      if (index < array.Length - count)
        Array.Copy(array, index + count, result, index, array.Length - index - count);
      return result;
    }

    #endregion
    #region Enumerate methods

    public static IEnumerable<T> Enumerate<T>(this T[] array, int index, int count)
    {
      if (array == null)
        throw new ArgumentNullException("array");
      if (index < 0 || index > array.Length)
        throw new ArgumentOutOfRangeException("index");
      if (count < 0 || count > array.Length - index)
        throw new ArgumentOutOfRangeException("count");

      for (; count > 0; index++, count--)
        yield return array[index];
    }

    public static IEnumerable<T> Enumerate<T>(this T[] array, Range range)
    {
      return array.Range(range.Index, range.Count);
    }

    #endregion
    #region Range methods

    public static T[] Range<T>(this T[] array, int index, int count)
		{
      if (array == null)
        throw new ArgumentNullException("array");

      T[] result = new T[count];
			Array.Copy(array, index, result, 0, count);
			return result;
		}

		public static T[] Range<T>(this T[] array, Range range)
		{
			return array.Range(range.Index, range.Count);
		}

		#endregion
		#region Resize methods

		public static T[] Resize<T>(this T[] array, int newSize)
		{
      if (array == null)
        throw new ArgumentNullException("array");
      if (newSize < 0)
				throw new ArgumentOutOfRangeException("newSize");

			if (newSize == array.Length)
				return array;
			T[] newArray = new T[newSize];
			Array.Copy(array, newArray, newSize < array.Length ? newSize : array.Length);
			return newArray;
		}

		#endregion
		#endregion
		#region Index formatting

		public static string FormatAsRegularIndices(int[] indices)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.ArrayIsEmpty], "indices");

			StringBuilder sb = new StringBuilder();
			string itemFormat = string.Format("{{0{0}}}", IndexItemFormat);
			sb.Append(IndexOpenBracket);
			sb.AppendFormat(itemFormat, indices[0]);
			itemFormat = IndexItemDelimiter + itemFormat;
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
					sb.Append(IndexItemDelimiter);
				sb.AppendFormat(itemFormat, indices[i]);
			}
			sb.Append(IndexCloseBracket);
			return sb.ToString();
		}

		public static string FormatAsJaggedIndices(int[][] indices)
		{
			if (indices == null)
				throw new ArgumentNullException("indices");
			if (indices.Length == 0)
				throw new ArgumentException(ArrayResources.Default.Strings[ArrayMessage.ArrayIsEmpty], "indices");

			StringBuilder sb = new StringBuilder();
			string itemFormat = string.Format("{{0{0}}}", IndexItemFormat);
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
					sb.Append(IndexLevelDelimiter);
				sb.Append(IndexOpenBracket);
				for (int j = 0; j < indices[i].Length; j++)
				{
					if (j > 0)
						sb.Append(IndexItemDelimiter);
					sb.AppendFormat(itemFormat, indices[i][j]);
				}
				sb.Append(IndexCloseBracket);
			}
			return sb.ToString();
		}

		public static string FormatAsLongRegularIndices(long[] indices)
		{
			StringBuilder sb = new StringBuilder();
			string itemFormat = string.Format("{{0{0}}}", IndexItemFormat);
			sb.Append(IndexOpenBracket);
			sb.AppendFormat(itemFormat, indices[0]);
			itemFormat = IndexItemDelimiter + itemFormat;
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
					sb.Append(IndexItemDelimiter);
				sb.AppendFormat(itemFormat, indices[i]);
			}
			sb.Append(IndexCloseBracket);
			return sb.ToString();
		}

		public static string FormatAsLongJaggedIndices(long[][] indices)
		{
			StringBuilder sb = new StringBuilder();
			string itemFormat = string.Format("{{0{0}}}", IndexItemFormat);
			for (int i = 0; i < indices.Length; i++)
			{
				if (i > 0)
					sb.Append(IndexLevelDelimiter);
				sb.Append(IndexOpenBracket);
				for (int j = 0; j < indices[i].Length; j++)
				{
					if (j > 0)
						sb.Append(IndexItemDelimiter);
					sb.AppendFormat(itemFormat, indices[i][j]);
				}
				sb.Append(IndexCloseBracket);
			}
			return sb.ToString();
		}

		#endregion
	}
}
