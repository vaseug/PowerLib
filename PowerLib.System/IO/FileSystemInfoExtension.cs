using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using PowerLib.System.Collections;
using PowerLib.System.Collections.Context;
using PowerLib.System.Collections.Matching;
using PowerLib.System.Linq;

namespace PowerLib.System.IO
{
	/// <summary>
	///	FileSystemInfoExtension class provides extension methods to work with FileSystemInfo and inherited FileInfo and DirectoryInfo objects.
	/// </summary>
	public static class FileSystemInfoExtension
	{
		private const string DefaultSearchPattern = "*";

		#region Is

		/// <summary>
		/// Gets a value that indicates that this is a FileInfo object.
		/// </summary>
		/// <param name="fsi">File system info object.</param>
		/// <returns>True if FileSystemInfo object represents FileInfo object, otherwise false.</returns>
		public static bool IsFile(this FileSystemInfo fsi)
		{                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
			if (fsi == null)
				throw new NullReferenceException();
			//
			return fsi is FileInfo;
		}

		/// <summary>
		/// Gets a value that indicates that this is a DirectoryInfo object.
		/// </summary>
		/// <param name="fsi">File system info object.</param>
		/// <returns>True if FileSystemInfo object represents DirectoryInfo object, otherwise false.</returns>
		public static bool IsDirectory(this FileSystemInfo fsi)
		{
			if (fsi == null)
				throw new NullReferenceException();
			//
			return fsi is DirectoryInfo;
		}

		/// <summary>
		/// Gets a value that indicates that this is a FileInfo object and exists.
		/// </summary>
		/// <param name="fsi">File system info object.</param>
		/// <returns>True if FileSystemInfo object represents FileInfo object and exists, otherwise false.</returns>
		public static bool IsFileAndExists(this FileSystemInfo fsi)
		{
			if (fsi == null)
				throw new NullReferenceException();
			//
			FileInfo fi = fsi as FileInfo;
			return fi != null && fi.Exists;
		}

		/// <summary>
		/// Gets a value that indicates that this is a DirectoryInfo object and exists.
		/// </summary>
		/// <param name="fsi">File system info object.</param>
		/// <returns>True if FileSystemInfo object represents DirectoryInfo object and exists, otherwise false.</returns>
		public static bool IsDirectoryAndExists(this FileSystemInfo fsi)
		{
			if (fsi == null)
				throw new NullReferenceException();
			//
			DirectoryInfo di = fsi as DirectoryInfo;
			return di != null && di.Exists;
		}

    #endregion
    #region Enumerate FileSystemInfo

    public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos<DI>(this DI diParent, int maxDepth, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison,
      Func<DI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren)
      where DI : FileSystemInfo
    {
      if (getChildren == null)
        throw new ArgumentNullException("getChildren");
      if (hasChildren == null)
        throw new ArgumentNullException("hasChildren");

      if (!hasChildren(diParent))
        yield break;
      foreach (FileSystemInfo fsi in comparison != null ?
        predicate != null ?
          getChildren(diParent).Where(predicate).Sort(comparison) :
          getChildren(diParent).Sort(comparison) :
        predicate != null ?
          getChildren(diParent).Where(predicate) :
          getChildren(diParent))
      {
        if (!hasChildren(fsi))
          yield return fsi;
        else if (fsi.Exists)
        {
          DI di = fsi as DI;
          if (di != null)
            using (var e = di.EnumerateFileSystemInfos(maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, predicate, comparison, getChildren, hasChildren).GetEnumerator())
            {
              if (e.MoveNext())
              {
                yield return fsi;
                if (maxDepth == 1)
                  continue;
                else
                  yield return e.Current;
                while (e.MoveNext())
                  yield return e.Current;
              }
              else if (!excludeEmpty)
                yield return fsi;
            }
          else
            yield return fsi;
        }
        else if (!excludeEmpty)
          yield return fsi;
      }
    }

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.EnumerateFileSystemInfos(maxDepth, excludeEmpty, predicate, comparison,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth == 1 ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories), fsi => fsi is DirectoryInfo);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false,
				predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

    #endregion
    #region Get FileSystemInfo

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate FileSystemInfo with IHierarchicalContext<T>

    public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos<DI>(this DI diParent, int maxDepth, bool excludeEmpty, HierarchicalContext<DI> context,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DI>>, bool> predicate, Comparison<FileSystemInfo> comparison,
      Func<DI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren)
      where DI : FileSystemInfo
    {
      if (getChildren == null)
        throw new ArgumentNullException("getChildren");
      if (hasChildren == null)
        throw new ArgumentNullException("hasChildren");

      if (!hasChildren(diParent))
        yield break;
      context.PushAncestor(diParent);
      try
      {
        foreach (FileSystemInfo fsi in comparison != null ?
          predicate != null ?
            getChildren(diParent).Where(t => predicate(ElementContext.Create(t, (IHierarchicalContext<DI>)context))).Sort(comparison) :
            getChildren(diParent).Sort(comparison) :
          predicate != null ?
            getChildren(diParent).Where(t => predicate(ElementContext.Create(t, (IHierarchicalContext<DI>)context))) :
            getChildren(diParent))
        {
          if (!hasChildren(fsi))
            yield return fsi;
          else if (fsi.Exists)
          {
            DI di = fsi as DI;
            if (di != null)
              using (var e = di.EnumerateFileSystemInfos(maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, context, predicate, comparison, getChildren, hasChildren).GetEnumerator())
              {
                if (e.MoveNext())
                {
                  yield return fsi;
                  if (maxDepth == 1)
                    continue;
                  else
                    yield return e.Current;
                  while (e.MoveNext())
                    yield return e.Current;
                }
                else if (!excludeEmpty)
                  yield return fsi;
              }
            else
              yield return fsi;
          }
          else if (!excludeEmpty)
            yield return fsi;
        }
      }
      finally
      {
        context.PopAncestor();
      }
    }

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.EnumerateFileSystemInfos(maxDepth, excludeEmpty, new FileSystemHierarchicalContext(), predicate, comparison,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth == 1 ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories), fsi => fsi is DirectoryInfo);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

    #endregion
    #region Get FileSystemInfo with IHierarchicalContext<T>

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Directories

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(fsi => fsi as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(fsi => fsi as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(fsi => fsi as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(DefaultSearchPattern, 0, false,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Directories

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Directories with IHierarchicalContext<T>

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(
        (ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
        (ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty,
        (ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(DefaultSearchPattern, 0, false,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Directories with IHierarchicalContext<T>

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Files

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, true,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo).Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, true,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo).Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, true,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo).Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(direction,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, direction,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Files

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      IPredicate<FileInfo> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<DirectoryInfo> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    #endregion
    #region Enumerate Files with IHierarchicalContext<T>

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, 0, true,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, true,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, true,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
        (x, y) => x is DirectoryInfo ?
          y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
          y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(direction,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, direction,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>), fileComparer != null ? fileComparer.Compare : default(Comparison<FileInfo>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), dirComparer != null ? dirComparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Files with IHierarchicalContext<T>

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate, IComparer<FileInfo> fileComparer,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate, IComparer<DirectoryInfo> dirComparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
    }

    #endregion
    #region Move

    public static Func<FileSystemInfo, string> Replacing(Func<string, string> replacing, bool renameOnly)
    {
      return (FileSystemInfo fsi) =>
      {
        FileInfo fi = fsi as FileInfo;
        if (fi != null)
          return renameOnly ? Path.Combine(fi.DirectoryName, replacing(fi.Name)) : replacing(fi.FullName);
        DirectoryInfo di = fsi as DirectoryInfo;
        if (di != null)
          return renameOnly ? Path.Combine(di.Parent != null ? di.Parent.FullName : di.Root.FullName, replacing(di.Name)) : replacing(di.FullName);
        return null;
      };
    }

    public static int MoveTo<DI>(this DI diStart, Func<DI, IEnumerable<FileSystemInfo>> getter, Func<FileSystemInfo, string, bool> mover, Func<FileSystemInfo, string> replacing)
      where DI : FileSystemInfo
    {
      int count = 0;
      foreach (var fsi in getter(diStart).Reverse())
        if (mover(fsi, replacing(fsi)))
          count++;
      return count;
    }

    public static int MoveTo(this DirectoryInfo diStart, Func<FileSystemInfo, bool> predicate,
      Func<FileSystemInfo, string> replacing, bool renameOnly)
    {
      return diStart.MoveTo(DefaultSearchPattern, 0, false, predicate, replacing);
    }

    public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Func<FileSystemInfo, string> replacing)
    {
      return diStart.MoveTo(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, replacing);
    }

    public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Func<FileSystemInfo, string> replacing)
    {
      if (replacing == null)
        throw new ArgumentNullException("replcaing");
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.MoveTo(
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
        (fsi, path) =>
        {
          if (string.IsNullOrEmpty(path))
            return false;
          FileInfo fi = fsi as FileInfo;
          if (fi != null)
          {
            if (!fi.Exists)
              return false;
            fi.MoveTo(path);
            return true;
          }
          DirectoryInfo di = fsi as DirectoryInfo;
          if (di != null)
          {
            if (!di.Exists)
              return false;
            di.MoveTo(path);
            return true;
          }
          return false;
        },
        replacing);
    }

		#endregion
		#region Move with IHierarchicalContext<T>

		public static int MoveTo(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<FileSystemInfo, string> replacing)
		{
			return diStart.MoveTo(DefaultSearchPattern, 0, false, predicate, replacing);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<FileSystemInfo, string> replacing)
		{
			return diStart.MoveTo(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, replacing);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<FileSystemInfo, string> replacing)
		{
      if (replacing == null)
        throw new ArgumentNullException("replcaing");
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.MoveTo(
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
        (fsi, path) =>
        {
          if (string.IsNullOrEmpty(path))
            return false;
          FileInfo fi = fsi as FileInfo;
          if (fi != null)
          {
            if (!fi.Exists)
              return false;
            fi.MoveTo(path);
            return true;
          }
          DirectoryInfo di = fsi as DirectoryInfo;
          if (di != null)
          {
            if (!di.Exists)
              return false;
            di.MoveTo(path);
            return true;
          }
          return false;
        },
        replacing);
    }

    #endregion
    #region Delete

    public static int Delete<DI>(this DI diStart, bool recursive, Func<DI, IEnumerable<FileSystemInfo>> getter, Func<FileSystemInfo, bool, bool> remover)
      where DI : FileSystemInfo
    {
      int count = 0;
      foreach (var fsi in recursive ? getter(diStart) : getter(diStart).Reverse())
        if (remover(fsi, recursive))
          count++;
      return count;
    }

    public static int Delete(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			return diStart.Delete(DefaultSearchPattern, 0, false, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			return diStart.Delete(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.Delete(recursive,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
        (fsi, r) =>
        {
          FileInfo fi = fsi as FileInfo;
          if (fi != null)
          {
            if (!fi.Exists)
              return false;
            fi.Delete();
            return true;
          }
          DirectoryInfo di = fsi as DirectoryInfo;
          if (di != null)
          {
            if (!di.Exists)
              return false;
            di.Delete(recursive);
            return true;
          }
          return false;
        });
		}

		#endregion
		#region Delete with IHierarchicalContext<T>

		public static int Delete(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			return diStart.Delete(DefaultSearchPattern, 0, false, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			return diStart.Delete(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.Delete(recursive,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
        (fsi, r) =>
        {
          FileInfo fi = fsi as FileInfo;
          if (fi != null)
          {
            if (!fi.Exists)
              return false;
            fi.Delete();
            return true;
          }
          DirectoryInfo di = fsi as DirectoryInfo;
          if (di != null)
          {
            if (!di.Exists)
              return false;
            di.Delete(recursive);
            return true;
          }
          return false;
        });
    }

    #endregion
  }
}
