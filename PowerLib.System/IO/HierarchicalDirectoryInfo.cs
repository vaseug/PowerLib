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

		internal HierarchicalDirectoryInfo(HierarchicalDirectoryInfo parent, DirectoryInfo di, int depth)
			: base(parent, di, depth)
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
