using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLib.System.Collections
{
	public interface IElementContext<T, C>
	{
		T Element
		{
			get;
		}

		C Context
		{
			get;
		}
	}
}
