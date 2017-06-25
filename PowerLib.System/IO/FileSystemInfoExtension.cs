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
		#region Enumerate FileSystemInfo (Comparer version)

		private static IEnumerable<FileSystemInfo> EnumerateFileSystemInfosCore(DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			//
			foreach (FileSystemInfo fsi in comparer != null ?
				predicate != null ?
					diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(predicate).OrderBy(o => o, comparer) :
					diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).OrderBy(o => o, comparer) :
				predicate != null ?
					diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(predicate) :
					diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
			{
				DirectoryInfo diInner = fsi as DirectoryInfo;
				if (diInner == null)
					yield return fsi;
				else
				{
					if (diStart.Exists)
					{
						using (IEnumerator<FileSystemInfo> e = EnumerateFileSystemInfosCore(diInner, searchPattern, maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, predicate, comparer).GetEnumerator())
						{
							if (e.MoveNext())
							{
								yield return fsi;
								if (maxDepth == 1)
									continue;
								else
									yield return e.Current;
							}
							else if (!excludeEmpty)
								yield return fsi;
							while (e.MoveNext())
								yield return e.Current;
						}
					}
				}
			}
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			//
			return EnumerateFileSystemInfosCore(diStart, DefaultSearchPattern, 0, false,
				predicate != null ? predicate : null, comparer);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			//
			return EnumerateFileSystemInfosCore(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				predicate, comparer);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			if (maxDepth < 0)
				throw new ArgumentOutOfRangeException("maxDepth");
			//
			return EnumerateFileSystemInfosCore(diStart, searchPattern, maxDepth, excludeEmpty,
				predicate, comparer);
		}

		#endregion
		#region Enumerate FileSystemInfo with IHierarchicalContext<T>

		private static IEnumerable<FileSystemInfo> EnumerateFileSystemInfosCore(DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty, FileSystemHierarchicalContext context,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			//
			context.PushAncestor(diStart);
			//
			try
			{
				//
				foreach (FileSystemInfo fsi in comparer != null ?
					predicate != null ?
						diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(o => predicate(new ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>(o, context))).OrderBy(o => o, comparer) :
						diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).OrderBy(o => o, comparer) :
					predicate != null ?
						diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(o => predicate(new ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>(o, context))) :
						diStart.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
				{
					DirectoryInfo diInner = fsi as DirectoryInfo;
					if (diInner == null)
						yield return fsi;
					else
					{
						if (diStart.Exists)
						{
							using (IEnumerator<FileSystemInfo> e = EnumerateFileSystemInfosCore(diInner, searchPattern, maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, context, predicate, comparer).GetEnumerator())
							{
								if (e.MoveNext())
								{
									yield return fsi;
									if (maxDepth == 1)
										continue;
									else
										yield return e.Current;
								}
								else if (!excludeEmpty)
									yield return fsi;
								while (e.MoveNext())
									yield return e.Current;
							}
						}
					}
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
			return EnumerateFileSystemInfos(diStart, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			//
			return EnumerateFileSystemInfosCore(diStart, DefaultSearchPattern, 0, false, new FileSystemHierarchicalContext(), predicate, comparer);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			//
			return EnumerateFileSystemInfosCore(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false, new FileSystemHierarchicalContext(), predicate, comparer);
		}

		public static IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			if (maxDepth < 0)
				throw new ArgumentOutOfRangeException("maxDepth");
			//
			return EnumerateFileSystemInfosCore(diStart, searchPattern, maxDepth, excludeEmpty, new FileSystemHierarchicalContext(), predicate , comparer);
		}

		#endregion
		#region Enumerate as HierarchicalFileSystemInfo

		private static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfosCore(HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			//
			foreach (FileSystemInfo fsi in comparer != null ?
				predicate != null ?
					diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(predicate).OrderBy(v => v, comparer) :
					diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).OrderBy(v => v, comparer) :
				predicate != null ?
					diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(predicate) :
					diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
			{
				DirectoryInfo diInner = fsi as DirectoryInfo;
				if (diInner == null)
					yield return new HierarchicalFileInfo(diStart, fsi as FileInfo, diStart.Depth + 1);
				else
				{
					HierarchicalDirectoryInfo diParent = new HierarchicalDirectoryInfo(diStart, diInner, diStart.Depth + 1);
					if (diStart.Info.Exists)
					{
						using (IEnumerator<HierarchicalFileSystemInfo> e = EnumerateHierarchicalFileSystemInfosCore(diParent, searchPattern, maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, predicate, comparer).GetEnumerator())
						{
							if (e.MoveNext())
							{
								yield return
									fsi is FileInfo ? (HierarchicalFileSystemInfo)new HierarchicalFileInfo(diParent, fsi as FileInfo, diParent.Depth + 1) : fsi is DirectoryInfo ? (HierarchicalFileSystemInfo)new HierarchicalDirectoryInfo(diParent, fsi as DirectoryInfo, diParent.Depth + 1) :
									null;
								if (maxDepth == 1)
									continue;
								else
									yield return e.Current;
							}
							else if (!excludeEmpty)
								yield return
									fsi is FileInfo ? (HierarchicalFileSystemInfo)new HierarchicalFileInfo(diParent, fsi as FileInfo, diParent.Depth + 1) : fsi is DirectoryInfo ? (HierarchicalFileSystemInfo)new HierarchicalDirectoryInfo(diParent, fsi as DirectoryInfo, diParent.Depth + 1) : null;
							while (e.MoveNext())
								yield return e.Current;
						}
					}
				}
			}
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), DefaultSearchPattern, 0, false, predicate, comparer);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false, predicate, comparer);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			if (maxDepth < 0)
				throw new ArgumentOutOfRangeException("maxDepth");
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), searchPattern, maxDepth, excludeEmpty, predicate, comparer);
		}

		#endregion
		#region Enumerate as HierarchicalFileSystemInfo with IHierarchicalContext<T>

		private static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfosCore(HierarchicalDirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			FileSystemHierarchicalContext context, Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			//
			context.PushAncestor(diStart.Info);
			//
			try
			{
				//
				foreach (FileSystemInfo fsi in comparer != null ?
					predicate != null ?
						diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(o => predicate(new ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>(o, context))).OrderBy(o => o, comparer) :
						diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).OrderBy(o => o, comparer) :
					predicate != null ?
						diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly).Where(o => predicate(new ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>(o, context))) :
						diStart.Info.EnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly))
				{
					DirectoryInfo diInner = fsi as DirectoryInfo;
					if (diInner == null)
						yield return new HierarchicalFileInfo(diStart, fsi as FileInfo, diStart.Depth + 1);
					else
					{
						HierarchicalDirectoryInfo diParent = new HierarchicalDirectoryInfo(diStart, diInner, diStart.Depth + 1);
						if (diStart.Info.Exists)
						{
							using (IEnumerator<HierarchicalFileSystemInfo> e = EnumerateHierarchicalFileSystemInfosCore(diParent, searchPattern, maxDepth > 0 ? maxDepth - 1 : 0, excludeEmpty, context, predicate, comparer).GetEnumerator())
							{
								if (e.MoveNext())
								{
									yield return
										fsi is FileInfo ? (HierarchicalFileSystemInfo)new HierarchicalFileInfo(diParent, fsi as FileInfo, diParent.Depth + 1) : fsi is DirectoryInfo ? (HierarchicalFileSystemInfo)new HierarchicalDirectoryInfo(diParent, fsi as DirectoryInfo, diParent.Depth + 1) :
										null;
									if (maxDepth == 1)
										continue;
									else
										yield return e.Current;
								}
								else if (!excludeEmpty)
									yield return
										fsi is FileInfo ? (HierarchicalFileSystemInfo)new HierarchicalFileInfo(diParent, fsi as FileInfo, diParent.Depth + 1) : fsi is DirectoryInfo ? (HierarchicalFileSystemInfo)new HierarchicalDirectoryInfo(diParent, fsi as DirectoryInfo, diParent.Depth + 1) : null;
								while (e.MoveNext())
									yield return e.Current;
							}
						}
					}
				}
			}
			finally
			{
				context.PopAncestor();
			}
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison != null ? new CustomComparer<FileSystemInfo>(comparison) : null);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), DefaultSearchPattern, 0, false, new FileSystemHierarchicalContext(), predicate, comparer);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false, new FileSystemHierarchicalContext(), predicate, comparer);
		}

		public static IEnumerable<HierarchicalFileSystemInfo> EnumerateHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			if (diStart == null)
				throw new NullReferenceException();
			if (searchPattern == null)
				throw new ArgumentNullException("searchPattern");
			if (maxDepth < 0)
				throw new ArgumentOutOfRangeException("maxDepth");
			//
			return EnumerateHierarchicalFileSystemInfosCore(new HierarchicalDirectoryInfo(null, diStart, 0), searchPattern, maxDepth, excludeEmpty, new FileSystemHierarchicalContext(), predicate, comparer);
		}

		#endregion
		#region Enumerate Directories

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(fsi => fsi as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(fsi => fsi as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(fsi => fsi as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, DefaultSearchPattern, 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate Directories with IHierarchicalContext<T>

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
					ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
					ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) =>
					ec.Element is DirectoryInfo && (predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as DirectoryInfo);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, DefaultSearchPattern, 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<DirectoryInfo> EnumerateDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate as HierarchicalDirectoryInfo

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty,
				fsi => fsi is DirectoryInfo && (predicate == null || predicate(fsi as DirectoryInfo)),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, DefaultSearchPattern, 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate as HierarchicalDirectoryInfo with IHierarchicalContext<T>

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) => ec.Element is DirectoryInfo &&
					(predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) => ec.Element is DirectoryInfo &&
					(predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty,
				(ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>> ec) => ec.Element is DirectoryInfo &&
					(predicate == null || predicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))),
				comparison != null ? (x, y) => comparison(x as DirectoryInfo, y as DirectoryInfo) : (Comparison<FileSystemInfo>)null)
				.Select(v => v as HierarchicalDirectoryInfo);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, DefaultSearchPattern, 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, false,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalDirectoryInfo> EnumerateHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty,
				predicate, comparer != null ? comparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate Files

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, true,
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
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, true,
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
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, true,
				fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is FileInfo).Select(fsi => fsi as FileInfo);
		}

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, direction,
				filePredicate != null ? filePredicate : (Func<FileInfo, bool>)null, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate != null ? dirPredicate : (Func<DirectoryInfo, bool>)null, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption, direction,
				filePredicate != null ? filePredicate : (Func<FileInfo, bool>)null, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate != null ? dirPredicate : (Func<DirectoryInfo, bool>)null, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction,
				filePredicate != null ? filePredicate : (Func<FileInfo, bool>)null, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate != null ? dirPredicate : (Func<DirectoryInfo, bool>)null, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate Files with IHierarchicalContext<T>

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, true,
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
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, true,
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
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, true,
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
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<FileInfo> EnumerateFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate as HierarchicalFileInfo

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, true,
				fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is HierarchicalFileInfo).Select(fsi => fsi as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, true,
				fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is HierarchicalFileInfo).Select(fsi => fsi as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, true,
				fsi => fsi is DirectoryInfo && (dirPredicate == null || dirPredicate(fsi as DirectoryInfo)) || fsi is FileInfo && (filePredicate == null || filePredicate(fsi as FileInfo)),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is HierarchicalFileInfo).Select(fsi => fsi as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, DefaultSearchPattern, 0, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Enumerate as HierarchicalFileInfo with IHierarchicalContext<T>

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, true,
				ec =>
					ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
					ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is HierarchicalFileInfo)
				.Select(fsi => fsi as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, true,
				ec =>
					ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
					ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(fsi => fsi is HierarchicalFileInfo)
				.Select(fsi => fsi as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, true,
				ec =>
					ec.Element is DirectoryInfo && (dirPredicate == null || dirPredicate(new ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as DirectoryInfo, ec.Context))) ||
					ec.Element is FileInfo && (filePredicate == null || filePredicate(new ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>(ec.Element as FileInfo, ec.Context))),
				(x, y) => x is DirectoryInfo ?
					y is DirectoryInfo ? dirComparison != null ? dirComparison(x as DirectoryInfo, y as DirectoryInfo) : 0 : direction ? 1 : -1 :
					y is DirectoryInfo ? direction ? -1 : 1 : fileComparison != null ? fileComparison(x as FileInfo, y as FileInfo) : 0)
				.Where(v => v is HierarchicalFileInfo)
				.Select(v => v as HierarchicalFileInfo);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, DefaultSearchPattern, 0, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		public static IEnumerable<HierarchicalFileInfo> EnumerateHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction,
				filePredicate, fileComparer != null ? fileComparer.Compare : (Comparison<FileInfo>)null,
				dirPredicate, dirComparer != null ? dirComparer.Compare : (Comparison<DirectoryInfo>)null);
		}

		#endregion
		#region Get FileSystemInfo

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get FileSystemInfo with IHierarchicalContext<T>

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, comparer).ToArray();
		}

		public static FileSystemInfo[] GetFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalFileSystemInfo

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalFileSystemInfo with IHierarchicalContext<T>

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<FileSystemInfo> comparison)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, DefaultSearchPattern, 0, false, predicate, comparer).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static HierarchicalFileSystemInfo[] GetHierarchicalFileSystemInfos(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<FileSystemInfo> comparer)
		{
			return EnumerateHierarchicalFileSystemInfos(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get Directories

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, predicate, comparer).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get Directories with IHierarchicalContext<T>

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, predicate, comparer).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static DirectoryInfo[] GetDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalDirectoryInfo

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, predicate, comparer).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<DirectoryInfo, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalDirectoryInfo with IHierarchicalContext<T>

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Comparison<DirectoryInfo> comparison)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparison).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, predicate, comparer).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, searchOption, excludeEmpty, predicate, comparer).ToArray();
		}

		public static HierarchicalDirectoryInfo[] GetHierarchicalDirectories(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, IComparer<DirectoryInfo> comparer)
		{
			return EnumerateHierarchicalDirectories(diStart, searchPattern, maxDepth, excludeEmpty, predicate, comparer).ToArray();
		}

		#endregion
		#region Get Files

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		#endregion
		#region Get Files with IHierarchicalContext<T>

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static FileInfo[] GetFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalFileInfo

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<DirectoryInfo, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<FileInfo, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<DirectoryInfo, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		#endregion
		#region Get as HierarchicalFileInfo with IHierarchicalContext<T>

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, Comparison<FileInfo> fileComparison,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, Comparison<DirectoryInfo> dirComparison)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparison, dirPredicate, dirComparison).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, searchOption, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		public static HierarchicalFileInfo[] GetHierarchicalFiles(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool direction,
			Func<ElementContext<FileInfo, IHierarchicalContext<DirectoryInfo>>, bool> filePredicate, IComparer<FileInfo> fileComparer,
			Func<ElementContext<DirectoryInfo, IHierarchicalContext<DirectoryInfo>>, bool> dirPredicate, IComparer<DirectoryInfo> dirComparer)
		{
			return EnumerateHierarchicalFiles(diStart, searchPattern, maxDepth, direction, filePredicate, fileComparer, dirPredicate, dirComparer).ToArray();
		}

		#endregion
		#region Move

		public static int MoveTo(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			return MoveTo(diStart, DefaultSearchPattern, 0, false, predicate, replacing, renameOnly);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			return MoveTo(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, replacing, renameOnly);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			if (replacing == null)
				throw new ArgumentNullException("replcaing");
			//
			FileSystemInfo[] fsis = diStart.GetFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Comparison());
			int count = 0;
			for (int i = 0; i < fsis.Length; i++)
			{
				FileInfo fi = fsis[fsis.Length - 1 - i] as FileInfo;
				if (fi != null && fi.Exists)
				{
					string fullName = renameOnly ? Path.Combine(fi.DirectoryName, replacing(fi.Name)) : replacing(fi.FullName);
					if (fullName != fi.FullName)
					{
						fi.MoveTo(fullName);
						count++;
					}
				}
				else
				{
					DirectoryInfo di = fsis[fsis.Length - 1 - i] as DirectoryInfo;
					if (di != null && di.Exists)
					{
						string fullName = renameOnly ? Path.Combine(di.Parent != null ? di.Parent.FullName : di.Root.FullName, replacing(di.Name)) : replacing(di.FullName);
						if (fullName != di.FullName)
						{
							di.MoveTo(fullName);
							count++;
						}
					}
				}
			}
			return count;
		}

		#endregion
		#region Move with IHierarchicalContext<T>

		public static int MoveTo(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			return MoveTo(diStart, DefaultSearchPattern, 0, false, predicate, replacing, renameOnly);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			return MoveTo(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, replacing, renameOnly);
		}

		public static int MoveTo(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, Func<string, string> replacing, bool renameOnly)
		{
			if (replacing == null)
				throw new ArgumentNullException("replcaing");

			FileSystemInfo[] fsis = diStart.GetFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Comparison());
			int count = 0;
			for (int i = 0; i < fsis.Length; i++)
			{
				FileInfo fi = fsis[fsis.Length - 1 - i] as FileInfo;
				if (fi != null && fi.Exists)
				{
					string fullName = renameOnly ? Path.Combine(fi.DirectoryName, replacing(fi.Name)) : replacing(fi.FullName);
					if (fullName != fi.FullName)
					{
						fi.MoveTo(fullName);
						count++;
					}
				}
				else
				{
					DirectoryInfo di = fsis[fsis.Length - 1 - i] as DirectoryInfo;
					if (di != null && di.Exists)
					{
						string fullName = renameOnly ? Path.Combine(di.Parent != null ? di.Parent.FullName : di.Root.FullName, replacing(di.Name)) : replacing(di.FullName);
						if (fullName != di.FullName)
						{
							di.MoveTo(fullName);
							count++;
						}
					}
				}
			}
			return count;
		}

		#endregion
		#region Delete

		public static int Delete(this DirectoryInfo diStart,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			return Delete(diStart, DefaultSearchPattern, 0, false, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			return Delete(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<FileSystemInfo, bool> predicate, bool recursive)
		{
			int count = 0;
			FileSystemInfo[] fsis = diStart.GetFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Comparison());
			for (int i = 0; i < fsis.Length; i++)
			{
				FileInfo fi = fsis[recursive ? i : fsis.Length - 1 - i] as FileInfo;
				if (fi != null && fi.Exists)
				{
					fi.Delete();
					count++;
				}
				else
				{
					DirectoryInfo di = fsis[recursive ? i : fsis.Length - 1 - i] as DirectoryInfo;
					if (di != null && di.Exists)
					{
						di.Delete(recursive);
						count++;
					}
				}
			}
			return count++;
		}

		#endregion
		#region Delete with IHierarchicalContext<T>

		public static int Delete(this DirectoryInfo diStart,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			return Delete(diStart, DefaultSearchPattern, 0, false, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, SearchOption searchOption, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			return Delete(diStart, searchPattern, searchOption == SearchOption.TopDirectoryOnly ? 1 : 0, excludeEmpty, predicate, recursive);
		}

		public static int Delete(this DirectoryInfo diStart, string searchPattern, int maxDepth, bool excludeEmpty,
			Func<ElementContext<FileSystemInfo, IHierarchicalContext<DirectoryInfo>>, bool> predicate, bool recursive)
		{
			int count = 0;
			FileSystemInfo[] fsis = diStart.GetFileSystemInfos(searchPattern, maxDepth, excludeEmpty, predicate, new SelectComparer<FileSystemInfo, bool>(t => t is FileInfo, ComparableComparer<bool>.Default).Comparison());
			for (int i = 0; i < fsis.Length; i++)
			{
				FileInfo fi = fsis[recursive ? i : fsis.Length - 1 - i] as FileInfo;
				if (fi != null && fi.Exists)
				{
					fi.Delete();
					count++;
				}
				else
				{
					DirectoryInfo di = fsis[recursive ? i : fsis.Length - 1 - i] as DirectoryInfo;
					if (di != null && di.Exists)
					{
						di.Delete(recursive);
						count++;
					}
				}
			}
			return count++;
		}

		#endregion
	}
}
