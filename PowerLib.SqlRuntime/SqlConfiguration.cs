using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Threading;

namespace PowerLib.SqlServer
{
  public static class SqlConfiguration
  {
    private static readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();
    private static readonly Dictionary<string, object> map = new Dictionary<string, object>();

    public static bool Init<T>(string name, T data)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (data == null)
        throw new ArgumentNullException("data");
      //
      locker.EnterUpgradeableReadLock();
      try
      {
        if (map.ContainsKey(name))
          return false;
        locker.EnterWriteLock();
        try
        {
          map.Add(name, data);
        }
        finally
        {
          locker.ExitWriteLock();
        }
        return true;
      }
      finally
      {
        locker.ExitUpgradeableReadLock();
      }
    }

    public static void Set<T>(string name, T data)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      if (data == null)
        throw new ArgumentNullException("data");
      //
      locker.EnterWriteLock();
      try
      {
        if (!map.ContainsKey(name))
          map.Add(name, data);
        else
          map[name] = data;
      }
      finally
      {
        locker.ExitWriteLock();
      }
    }

    public static T Get<T>(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      //
      locker.EnterReadLock();
      try
      {
        object result;
        return map.TryGetValue(name, out result) ? (T)result : default(T);
      }
      finally
      {
        locker.ExitReadLock();
      }
    }

    public static void Clear()
    {
      locker.EnterWriteLock();
      try
      {
        map.Clear();
      }
      finally
      {
        locker.ExitWriteLock();
      }
    }

    public static bool Remove(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      //
      locker.EnterWriteLock();
      try
      {
        return map.Remove(name);
      }
      finally
      {
        locker.ExitWriteLock();
      }
    }

    public static bool Contains(string name)
    {
      if (name == null)
        throw new ArgumentNullException("name");
      //
      locker.EnterReadLock();
      try
      {
        return map.ContainsKey(name);
      }
      finally
      {
        locker.ExitReadLock();
      }
    }
  }
}
