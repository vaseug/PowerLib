using System;
using System.IO;
using PowerLib.System.Collections;


namespace PowerLib.System.IO
{
	/// <summary>
	/// HierarchicalDirectoryInfo
	/// </summary>
	public sealed class HierarchicalDirectoryInfo : HierarchicalFileSystemInfo
	{
		#region Constructors

		internal HierarchicalDirectoryInfo(DirectoryInfo di, HierarchicalDirectoryInfo parent, int depth, string relativeName)
      : base(di, parent, depth, relativeName)
    {
		}

		#endregion
		#region Properties

		/// <summary>
		/// DirectoryInfo
		/// </summary>
		public new DirectoryInfo Info
		{
			get
			{
				return (DirectoryInfo)base.Info;
			}
		}

		#endregion
		#region Operators

		public static explicit operator DirectoryInfo(HierarchicalDirectoryInfo hdi)
		{
			return hdi.Info;
		}

		#endregion
	}
}
