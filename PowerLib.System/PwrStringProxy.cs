using System;
using PowerLib.System.Collections;

namespace PowerLib.System
{
  /// <summary>
  /// 
  /// </summary>
  public class PwrStringProxy : PwrListProxy<char>
  {
    #region Constructors

    public PwrStringProxy(string str)
      : base(() => str.Length, i => str[i])
    {
      if (str == null)
        throw new ArgumentNullException("str");
    }

    #endregion
  }
}
