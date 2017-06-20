using System;
using System.IO;
using System.Resources;
using System.Globalization;


namespace PowerLib.System.Resources
{
	public class ResourceAccessor<K>
	{
		private ResourceManager _rm;
		private CultureInfo _ci;
		private Func<K, string> _selector;
		private Accessor<K, string> _strings;
		private Accessor<K, Stream> _streams;
		private Accessor<K, object> _objects;

		#region Constructors

		public ResourceAccessor(Func<K, string> selector, ResourceManager rm)
			: this(selector, rm, null)
		{
		}

		public ResourceAccessor(Func<K, string> selector, ResourceManager rm, CultureInfo ci)
		{
			if (selector == null)
				throw new ArgumentNullException("selector");
			if (rm == null)
				throw new ArgumentNullException("rm");

			_rm = rm;
			_ci = ci;
			_selector = selector;
			_strings = new Accessor<K, string>(t => _ci != null ? _rm.GetString(selector(t), _ci) : _rm.GetString(selector(t)));
			_streams = new Accessor<K, Stream>(t => _ci != null ? _rm.GetStream(selector(t), _ci) : _rm.GetStream(selector(t)));
			_objects = new Accessor<K, object>(t => _ci != null ? _rm.GetObject(selector(t), _ci) : _rm.GetObject(selector(t)));
		}

		#endregion
		#region Properties

		public ResourceManager ResourceManager
		{
			get { return _rm; }
		}

		public CultureInfo DefaultCulture
		{
			get { return _ci; }
		}

    public string FormatMessage(K key, params object[] args)
    {
      string format = Strings[key];
      return format == null ? null : string.Format(format, args);
    }

		public IAccessor<K, string> Strings
		{
			get { return _strings; }
		}

		public IAccessor<K, Stream> Streams
		{
			get { return _streams; }
		}

		public IAccessor<K, object> Objects
		{
			get { return _objects; }
		}

		#endregion
	}
}
