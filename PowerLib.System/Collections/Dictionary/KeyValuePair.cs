using System;
using System.Collections.Generic;

namespace PowerLib.System.Collections
{
  public static class KeyValuePair
  {
    public static KeyValuePair<K, T> Create<K, T>(K key, T value)
    {
      return new KeyValuePair<K, T>(key, value);
    }
  }
}
