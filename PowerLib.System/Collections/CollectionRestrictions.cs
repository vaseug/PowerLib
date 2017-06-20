using System;


namespace PowerLib.System.Collections
{
	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum CollectionRestrictions
	{
		/// <summary>
		/// ����������� �����������
		/// </summary>
		None = 0x0,
		/// <summary>
		/// ����������� �� ���������� ������ ���������
		/// </summary>
		FixedSize = 0x1,
		/// <summary>
		/// ����������� �� ��������� ��������� ������������ ��������� ���������
		/// </summary>
		FixedLayout = 0x2,
		/// <summary>
		/// ����������� �� ��������� ��������� ��������� ������
		/// </summary>
		Fixed = 0x3,
		/// <summary>
		/// ����������� �� ��������� �������� �������� ���������
		/// </summary>
		ReadOnlyValue = 0x4,
		/// <summary>
		/// ����������� �� ����� ��������� ���������
		/// </summary>
		ReadOnly = 0x7,
	}
}
