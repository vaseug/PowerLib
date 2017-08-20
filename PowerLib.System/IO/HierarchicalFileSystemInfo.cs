using System;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.IO
{
	/// <summary>
	/// HierarchicalFileSystemInfo
	/// </summary>
	public class HierarchicalFileSystemInfo : IParentRef<HierarchicalDirectoryInfo>
	{
		private HierarchicalDirectoryInfo _parent;
		private FileSystemInfo _fsi;
		private int _depth;
    private string _relativeName;

		#region Constructors

		internal HierarchicalFileSystemInfo(FileSystemInfo fsi, HierarchicalDirectoryInfo parent, int depth, string relativeName)
    {
			if (fsi == null)
				throw new ArgumentNullException("info");
			if (depth < 0)
				throw new ArgumentOutOfRangeException("depth");

			_parent = parent;
			_fsi = fsi;
			_depth = depth;
      _relativeName = relativeName;
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

    /// <summary>
    /// Relative name
    /// </summary>
    public string RelativeName
    {
      get
      {
        return _relativeName;
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
