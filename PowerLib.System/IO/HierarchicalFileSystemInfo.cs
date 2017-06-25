using System;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.IO
{
	/// <summary>
	/// HierarchicalFileSystemInfo
	/// </summary>
	public abstract class HierarchicalFileSystemInfo : IParentRef<HierarchicalDirectoryInfo>
	{
		private HierarchicalDirectoryInfo _parent;
		private FileSystemInfo _fsi;
		private int _depth;

		#region Constructors

		protected HierarchicalFileSystemInfo(HierarchicalDirectoryInfo parent, FileSystemInfo fsi, int depth)
		{
			if (fsi == null)
				throw new ArgumentNullException("info");
			if (depth < 0)
				throw new ArgumentOutOfRangeException("depth");
			//
			_parent = parent;
			_fsi = fsi;
			_depth = depth;
		}

		#endregion
		#region Properties

		/// <summary>
		/// Parent
		/// </summary>
		public HierarchicalDirectoryInfo Parent
		{
			get
			{
				return _parent;
			}
		}

		/// <summary>
		/// FileSystemInfo
		/// </summary>
		public FileSystemInfo Info
		{
			get
			{
				return _fsi;
			}
		}

		/// <summary>
		/// Depth of FileSystemInfo in file system tree
		/// </summary>
		public int Depth
		{
			get
			{
				return _depth;
			}
		}

		#endregion
		#region Operators

		public static explicit operator FileSystemInfo(HierarchicalFileSystemInfo hfsi)
		{
			return hfsi.Info;
		}

		#endregion
	}
}
