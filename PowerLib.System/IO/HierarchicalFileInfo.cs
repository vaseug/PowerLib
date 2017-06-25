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

		internal HierarchicalFileInfo(HierarchicalDirectoryInfo directory, FileInfo fi, int depth)
			: base(directory, fi, depth)
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
