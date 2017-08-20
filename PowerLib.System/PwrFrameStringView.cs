using PowerLib.System.Collections;

namespace PowerLib.System
{
  public class PwrFrameStringView : PwrFrameListView<char>
  {
    #region Constructors

    public PwrFrameStringView(string str)
			: base(new PwrStringProxy(str))
		{
    }

    public PwrFrameStringView(string str, Range frame)
      : base(new PwrStringProxy(str), frame)
    {
    }

    public PwrFrameStringView(string str, int frameOffset, int frameSize)
			: base(new PwrStringProxy(str), frameOffset, frameSize)
		{
    }

    #endregion
  }
}
