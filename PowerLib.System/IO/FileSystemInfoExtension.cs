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

      if (maxDepth == 0 || !hasChildren(diParent))
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
            using (var e = di.EnumerateFileSystemInfos(maxDepth - 1, excludeEmpty, predicate, comparison, getChildren, hasChildren).GetEnumerator())
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
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      using (var e = diStart.EnumerateFileSystemInfos(maxDepth, (traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) != 0, predicate, comparison,
        di => di.EnumerateFileSystemInfos(searchPattern ?? DefaultSearchPattern, SearchOption.TopDirectoryOnly), fsi => fsi is DirectoryInfo).GetEnumerator())
      {
        if (e.MoveNext())
        {
          if ((traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
            yield return diStart;
          do
            yield return e.Current;
          while (e.MoveNext());
        }
        else if ((traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) == 0 && (traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
          yield return diStart;
      }
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0,
				predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

    #endregion
    #region Get FileSystemInfo

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
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

      if (maxDepth == 0 || !hasChildren(diParent))
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
              using (var e = di.EnumerateFileSystemInfos(maxDepth - 1, excludeEmpty, context, predicate, comparison, getChildren, hasChildren).GetEnumerator())
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
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, comparison);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      using (var e = diStart.EnumerateFileSystemInfos(maxDepth, (traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) != 0, new FileSystemHierarchicalContext(), predicate, comparison,
        di => di.EnumerateFileSystemInfos(searchPattern ?? DefaultSearchPattern, SearchOption.TopDirectoryOnly), fsi => fsi is DirectoryInfo).GetEnumerator())
      {
        if (e.MoveNext())
        {
          if ((traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
            yield return diStart;
          do
            yield return e.Current;
          while (e.MoveNext());
        }
        else if ((traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) == 0 && (traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
          yield return diStart;
      }
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
		{
			return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
		}

    #endregion
    #region Get FileSystemInfo with IHierarchicalContext<T>

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, comparer).ToArray();
    }

    public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
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

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(fsi => fsi as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(fsi => fsi as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Directories

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
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

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        (ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        (ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as DirectoryInfo);
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Directories with IHierarchicalContext<T>

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Files

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Files

    public static FileInfo[] GetFiles(this DirectoryInfo diStart,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(filePredicate, dirPredicate, comparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Files with IHierarchicalContext<T>

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(DefaultSearchPattern, int.MaxValue, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, searchOption, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFileSystemInfos(searchPattern, maxDepth, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is FileInfo)
        .Select(fsi => fsi as FileInfo);
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Files with IHierarchicalContext<T>

    public static FileInfo[] GetFiles(this DirectoryInfo diStart,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(filePredicate, dirPredicate, comparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparer).ToArray();
    }

    public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparer).ToArray();
    }

    #endregion
    #region Hierarchical methods

    public static string GetRelativePath(this FileSystemInfo fsi, HierarchicalDirectoryInfo hdiParent)
    {
      return hdiParent == null ? null : fsi.FullName
        .Remove(0, (hdiParent.RelativeName == null ? hdiParent.Info.FullName : hdiParent.Info.FullName.Remove(hdiParent.Info.FullName.Length - hdiParent.RelativeName.Length, hdiParent.RelativeName.Length)).Length)
        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    public static HierarchicalDirectoryInfo CreateHierarchicalDirectoryInfo(this DirectoryInfo di, HierarchicalDirectoryInfo hdiParent)
    {
      return new HierarchicalDirectoryInfo(di, hdiParent, hdiParent == null ? 0 : hdiParent.Depth + 1, di.GetRelativePath(hdiParent));
    }

    public static HierarchicalFileInfo CreateHierarchicalFileInfo(this FileInfo fi, HierarchicalDirectoryInfo hdiParent)
    {
      return new HierarchicalFileInfo(fi, hdiParent, hdiParent == null ? 0 : hdiParent.Depth + 1, fi.GetRelativePath(hdiParent));
    }

    public static HierarchicalFileSystemInfo CreateHierarchicalFileSystemInfo(this FileSystemInfo fsi, HierarchicalDirectoryInfo hdiParent)
    {
      if (fsi is DirectoryInfo)
        return new HierarchicalDirectoryInfo((DirectoryInfo)fsi, hdiParent, hdiParent == null ? 0 : hdiParent.Depth + 1, fsi.GetRelativePath(hdiParent));
      else if (fsi is FileInfo)
        return new HierarchicalFileInfo((FileInfo)fsi, hdiParent, hdiParent == null ? 0 : hdiParent.Depth + 1, fsi.GetRelativePath(hdiParent));
      else
        return new HierarchicalFileSystemInfo(fsi, hdiParent, hdiParent == null ? 0 : hdiParent.Depth + 1, fsi.GetRelativePath(hdiParent));
    }

    #endregion
    #region Enumerate Hierarchical FileSystem items

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos<HDI>(this HDI hdiParent, int maxDepth, bool excludeEmpty,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison,
      Func<HDI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren, Func<FileSystemInfo, HDI, HierarchicalFileSystemInfo> factory)
      where HDI : HierarchicalFileSystemInfo
    {
      if (getChildren == null)
        throw new ArgumentNullException("getChildren");
      if (hasChildren == null)
        throw new ArgumentNullException("hasChildren");

      if (maxDepth == 0 || !hasChildren(hdiParent.Info))
        yield break;
      foreach (FileSystemInfo fsi in comparison != null ?
        predicate != null ?
          getChildren(hdiParent).Where(predicate).Sort(comparison) :
          getChildren(hdiParent).Sort(comparison) :
        predicate != null ?
          getChildren(hdiParent).Where(predicate) :
          getChildren(hdiParent))
      {
        if (!hasChildren(fsi))
          yield return factory(fsi, hdiParent);
        else if (fsi.Exists)
        {
          var hfsi = factory(fsi, hdiParent);
          HDI hdi = hfsi as HDI;
          if (hdi != null)
            using (var e = hdi.EnumerateHierarchicalFileSystemInfos(maxDepth - 1, excludeEmpty, predicate, comparison, getChildren, hasChildren, factory).GetEnumerator())
            {
              if (e.MoveNext())
              {
                yield return hfsi;
                if (maxDepth == 1)
                  continue;
                else
                  yield return e.Current;
                while (e.MoveNext())
                  yield return e.Current;
              }
              else if (!excludeEmpty)
                yield return hfsi;
            }
          else
            yield return hfsi;
        }
        else if (!excludeEmpty)
          yield return factory(fsi, hdiParent);
      }
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison);
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, comparison);
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      if (hdiStart == null)
        throw new ArgumentNullException("hdiStart");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      using (var e = hdiStart.EnumerateHierarchicalFileSystemInfos(maxDepth, (traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) != 0, predicate, comparison,
        di => di.Info.EnumerateFileSystemInfos(searchPattern ?? DefaultSearchPattern, SearchOption.TopDirectoryOnly), fsi => fsi is DirectoryInfo, CreateHierarchicalFileSystemInfo).GetEnumerator())
      {
        if (e.MoveNext())
        {
          if ((traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
            yield return hdiStart;
          do
            yield return e.Current;
          while (e.MoveNext());
        }
        else if ((traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) == 0 && (traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
          yield return hdiStart;
      }
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<FileSystemInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Hierarchical FileSystem items

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparer).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<FileSystemInfo> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Hierarchical FileSystem items with IHierarchicalContext<T>

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos<HDI>(this HDI hdiParent, int maxDepth, bool excludeEmpty, HierarchicalContext<HDI> context,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HDI>>, bool> predicate, Comparison<FileSystemInfo> comparison,
      Func<HDI, IEnumerable<FileSystemInfo>> getChildren, Func<FileSystemInfo, bool> hasChildren, Func<FileSystemInfo, HDI, HierarchicalFileSystemInfo> factory)
      where HDI : HierarchicalFileSystemInfo
    {
      if (getChildren == null)
        throw new ArgumentNullException("getChildren");
      if (hasChildren == null)
        throw new ArgumentNullException("hasChildren");

      if (maxDepth == 0 || !hasChildren(hdiParent.Info))
        yield break;
      foreach (FileSystemInfo fsi in comparison != null ?
          predicate != null ?
            getChildren(hdiParent).Where(t => predicate(ElementContext.Create(t, (IHierarchicalContext<HDI>)context))).Sort(comparison) :
            getChildren(hdiParent).Sort(comparison) :
          predicate != null ?
            getChildren(hdiParent).Where(t => predicate(ElementContext.Create(t, (IHierarchicalContext<HDI>)context))) :
            getChildren(hdiParent))
      {
        if (!hasChildren(fsi))
          yield return factory(fsi, hdiParent);
        else if (fsi.Exists)
        {
          var hfsi = factory(fsi, hdiParent);
          HDI hdi = hfsi as HDI;
          if (hdi != null)
            using (var e = hdi.EnumerateHierarchicalFileSystemInfos(maxDepth - 1, excludeEmpty, context, predicate, comparison, getChildren, hasChildren, factory).GetEnumerator())
            {
              if (e.MoveNext())
              {
                yield return hfsi;
                if (maxDepth == 1)
                  continue;
                else
                  yield return e.Current;
                while (e.MoveNext())
                  yield return e.Current;
              }
              else if (!excludeEmpty)
                yield return hfsi;
            }
          else
            yield return hfsi;
        }
        else if (!excludeEmpty)
          yield return factory(fsi, hdiParent);
      }
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison);
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, comparison);
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      if (hdiStart == null)
        throw new ArgumentNullException("hdiStart");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      using (var e = hdiStart.EnumerateHierarchicalFileSystemInfos(maxDepth, (traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) != 0, new HierarchicalFileSystemHierarchicalContext(), predicate, comparison,
        di => di.Info.EnumerateFileSystemInfos(searchPattern ?? DefaultSearchPattern, SearchOption.TopDirectoryOnly), fsi => fsi is DirectoryInfo, CreateHierarchicalFileSystemInfo).GetEnumerator())
      {
        if (e.MoveNext())
        {
          if ((traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
            yield return hdiStart;
          do
            yield return e.Current;
          while (e.MoveNext());
        }
        else if ((traversalOptions & FileSystemTraversalOptions.ExcludeEmpty) == 0 && (traversalOptions & FileSystemTraversalOptions.ExcludeStart) == 0)
          yield return hdiStart;
      }
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Hierarchical FileSystem items with IHierarchicalContext<T>

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, 0, predicate, comparer).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfo(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<FileSystemInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Hierarchical Directories

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(hfsi => hfsi as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(hfsi => hfsi as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return hdiStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : default(Comparison<FileSystemInfo>))
        .Select(hfsi => hfsi as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalDirectories(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo hdiStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return hdiStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<DirectoryInfo, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Hierarchical Directories

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(predicate, comparer).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<DirectoryInfo> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Hierarchical Directories with IHierarchicalContext<T>

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(
        (ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        (ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, traversalOptions,
        (ElementContext<FileSystemInfo, IHierarchicalContext<HierarchicalDirectoryInfo>> ec) =>
          ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
        comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
        .Select(v => v as HierarchicalDirectoryInfo);
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(DefaultSearchPattern, int.MaxValue, 0,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions,
        predicate != null ? predicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>), comparer != null ? comparer.Compare : default(Comparison<DirectoryInfo>));
    }

    #endregion
    #region Get Hierarchical Directories with IHierarchicalContext<T>

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, searchOption, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparison).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(predicate, comparer).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, searchOption, traversalOptions, predicate, comparer).ToArray();
    }

    public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> predicate, IComparer<DirectoryInfo> comparer)
    {
      return diStart.EnumerateHierarchicalDirectories(searchPattern, maxDepth, traversalOptions, predicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Hierarchical Files

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth,
        filePredicate != null ? filePredicate.Match : default(Func<FileInfo, bool>), dirPredicate != null ? dirPredicate.Match : default(Func<DirectoryInfo, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Hierarchical Files

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<FileInfo, bool> filePredicate, Func<DirectoryInfo, bool> dirPredicate, Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(filePredicate, dirPredicate, comparer).ToArray();
    }

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparer).ToArray();
    }

    public static HierarchicalFileInfo[] GetHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<FileInfo> filePredicate, IPredicate<DirectoryInfo> dirPredicate, IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparer).ToArray();
    }

    #endregion
    #region Enumerate Hierarchical Files with IHierarchicalContext<T>

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(DefaultSearchPattern, int.MaxValue, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, searchOption, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFileSystemInfos(searchPattern, maxDepth, FileSystemTraversalOptions.ExcludeStart | FileSystemTraversalOptions.ExcludeEmpty,
        ec =>
          ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
          ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>(ec.Element as FileInfo, ec.Context))), comparison)
        .Where(fsi => fsi is HierarchicalFileInfo)
        .Select(fsi => fsi as HierarchicalFileInfo);
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth,
        filePredicate != null ? filePredicate.Match : default(Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        dirPredicate != null ? dirPredicate.Match : default(Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool>),
        comparer != null ? comparer.Compare : default(Comparison<FileSystemInfo>));
    }

    #endregion
    #region Get Hierarchical Files with IHierarchicalContext<T>

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      Func<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> filePredicate,
      Func<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>, bool> dirPredicate,
      Comparison<FileSystemInfo> comparison)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparison).ToArray();
    }

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(filePredicate, dirPredicate, comparer).ToArray();
    }

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, SearchOption searchOption,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, searchOption, filePredicate, dirPredicate, comparer).ToArray();
    }

    public static HierarchicalFileInfo[] GetFiles(this HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth,
      IPredicate<ElementContext<FileInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> filePredicate,
      IPredicate<ElementContext<DirectoryInfo, IHierarchicalContext<HierarchicalDirectoryInfo>>> dirPredicate,
      IComparer<FileSystemInfo> comparer)
    {
      return diStart.EnumerateHierarchicalFiles(searchPattern, maxDepth, filePredicate, dirPredicate, comparer).ToArray();
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
      return diStart.MoveTo(DefaultSearchPattern, int.MaxValue, 0, predicate, replacing);
    }

    public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
      Func<FileSystemInfo, bool> predicate, Func<FileSystemInfo, string> replacing)
    {
      return diStart.MoveTo(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, replacing);
    }

    public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
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
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
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
			return diStart.MoveTo(DefaultSearchPattern, int.MaxValue, 0, predicate, replacing);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<FileSystemInfo, string> replacing)
		{
			return diStart.MoveTo(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, replacing);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
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
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
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
			return diStart.Delete(DefaultSearchPattern, int.MaxValue, 0, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			return diStart.Delete(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.Delete(recursive,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
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
			return diStart.Delete(DefaultSearchPattern, int.MaxValue, 0, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, FileSystemTraversalOptions traversalOptions,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			return diStart.Delete(searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : int.MaxValue, traversalOptions, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, FileSystemTraversalOptions traversalOptions,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
      if (diStart == null)
        throw new ArgumentNullException("diStart");
      if (searchPattern == null)
        throw new ArgumentNullException("searchPattern");
      if (maxDepth < 0)
        throw new ArgumentOutOfRangeException("maxDepth");

      return diStart.Delete(recursive,
        di => di.EnumerateFileSystemInfos(searchPattern, maxDepth, traversalOptions, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Compare),
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

  [Flags]
  public enum FileSystemTraversalOptions
  {
    ExcludeStart = 1,
    ExcludeEmpty = 2
  }
}
