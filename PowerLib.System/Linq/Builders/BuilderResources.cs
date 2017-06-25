using System;
using System.Globalization;
using PowerLib.System.Resources;


namespace PowerLib.System.Linq.Builders
{
  internal sealed class BuilderResources : EnumTypeResourceAccessor<BuilderMessage, BuilderResources>
  {
    private static Lazy<BuilderResources> _default = new Lazy<BuilderResources>(() => new BuilderResources());

    private BuilderResources()
      : base(t => t.ToString())
    {
    }

    internal BuilderResources(CultureInfo ci)
      : base(t => t.ToString(), ci)
    {
    }

    public static BuilderResources Default
    {
      get { return _default.Value; }
    }
  }

	internal enum BuilderMessage
	{
    FieldOrPropertyNotFound = 1,
    FieldNotFound = 2,
    PropertyNotFound = 3,
    MethodNotFound = 4,
    ConstructorNotFound = 5,
	}
}
