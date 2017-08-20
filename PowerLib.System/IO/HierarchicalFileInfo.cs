using System;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.IO
{
	/// <summary>
	/// HierarchicalFileInfo
	/// </summary>
	public sealed class HierarchicalFileInfo : HierarchicalFileSystemInfo
	{
		#region Constructors

		internal HierarchicalFileInfo(FileInfo fi, HierarchicalDirectoryInfo directory, int depth, string relativeName)
      : base(fi, directory, depth, relativeName)
    {
		}

		#endregion
		#region Properties

		/// <summary>
		/// FileInfo
		/// </summary>
		public new FileInfo Info
		{
			get
			{
				return (FileInfo)base.Info;
			}
		}

		#endregion
		#region Operators

		public static explicit operator FileInfo(HierarchicalFileInfo hfi)
		{
			return hfi.Info;
		}

		#endregion
	}
}
