﻿using System;
using System.Collections.Generic;

namespace PowerLib.System.Linq
{
  public static class ElementExtension
  {
    public static TSource With<TSource>(this TSource element, Action<TSource> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      if (element != null)
        action(element);
      return element;
    }

    public static TSource? With<TSource>(this TSource? element, Action<TSource> action)
      where TSource : struct
    {
      if (action == null)
        throw new ArgumentNullException("action");

      if (element.HasValue)
        action(element.Value);
      return element;
    }

    public static TResult With<TSource, TResult>(this TSource element, Func<TSource, TResult> selector)
    {
      if (selector == null)
        throw new ArgumentNullException("selector");

      return element != null ? selector(element) : default(TResult);
    }

    public static TResult With<TSource, TResult>(this TSource? element, Func<TSource, TResult> selector)
      where TSource : struct
    {
      if (selector == null)
        throw new ArgumentNullException("selector");

      return element.HasValue? selector(element.Value) : default(TResult);
    }

    public static TResult WithDispose<TSource, TResult>(this TSource element, Func<TSource, TResult> selector)
      where TSource : IDisposable
    {
      if (selector == null)
        throw new ArgumentNullException("selector");

      if (element == null)
        return default(TResult);

      using (element)
        return selector(element);
    }

    public static IEnumerable<TResult> Yield<TSource, TResult>(this TSource element, Func<TSource, TResult> producer, Func<TSource, TResult, TSource> mutator, Func<TSource, bool> predicate)
    {
      if (producer == null)
        throw new ArgumentNullException("producer");
      if (predicate == null)
        throw new ArgumentNullException("predicate");

      while (predicate(element))
      {
        var portion = producer(element);
        yield return portion;
        if (mutator != null)
          element = mutator(element, portion);
      }
    }

    public static IEnumerable<TResult> Yield<TSource, TResult>(this TSource element, Func<TSource, int, TResult> producer, Func<TSource, int, TResult, TSource> mutator, Func<TSource, int, bool> predicate)
    {
      if (producer == null)
        throw new ArgumentNullException("producer");
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      int i = 0;
      while (predicate(element, i))
      {
        var portion = producer(element, i);
        yield return portion;
        if (mutator != null)
          element = mutator(element, i, portion);
        i++;
      }
    }

    public static TSource? AsNullable<TSource>(this TSource element)
      where TSource : struct
    {
      return (TSource?)element;
    }

    public static TSource? NullIf<TSource>(this TSource element, Func<TSource, bool> predicate)
      where TSource : struct
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");

      return predicate(element) ? default(TSource) : element;
    }

    public static TSource DefaultIf<TSource>(this TSource element, Func<TSource, bool> predicate)
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");

      return predicate(element) ? default(TSource) : element;
    }

    public static TSource WithIf<TSource>(this TSource element, Func<TSource, bool> predicate, Action<TSource> action)
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      if (action == null)
        throw new ArgumentNullException("action");

      if (predicate(element))
        action(element);
      return element;
    }

    public static TSource WithCase<TSource>(this TSource element, Func<TSource, bool> predicate, Action<TSource> trueAction, Action<TSource> falseAction)
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      if (trueAction == null)
        throw new ArgumentNullException("trueAction");
      if (falseAction == null)
        throw new ArgumentNullException("falseAction");

      if (predicate(element))
        trueAction(element);
      else
        falseAction(element);
      return element;
    }

    public static TResult WithCase<TSource, TResult>(this TSource element, Func<TSource, bool> predicate, Func<TSource, TResult> trueFunc, Func<TSource, TResult> falseFunc)
    {
      if (predicate == null)
        throw new ArgumentNullException("predicate");
      if (trueFunc == null)
        throw new ArgumentNullException("trueFunc");
      if (falseFunc == null)
        throw new ArgumentNullException("falseFunc");

      return predicate(element) ? trueFunc(element) : falseFunc(element);
    }

    public static TSource WithCase<TKey, TSource>(this TSource element, TKey key, IEnumerable<KeyValuePair<TKey, Action<TSource>>> actions)
    {
      IDictionary<TKey, Action<TSource>> dic = actions as IDictionary<TKey, Action<TSource>>;
      if (actions == null)
      {
        dic = new Dictionary<TKey, Action<TSource>>();
        actions.Apply(a => dic.Add(a.Key, a.Value));
      }
      Action<TSource> action;
      if (dic.TryGetValue(key, out action))
        action(element);
      return element;
    }

    public static TResult WithCase<TKey, TSource, TResult>(this TSource element, TKey key, Func<TSource, TResult> defaultFunc, IEnumerable<KeyValuePair<TKey, Func<TSource, TResult>>> functions)
    {
      IDictionary<TKey, Func<TSource, TResult>> dic = functions as IDictionary<TKey, Func<TSource, TResult>>;
      if (functions == null)
      {
        dic = new Dictionary<TKey, Func<TSource, TResult>>();
        functions.Apply(a => dic.Add(a.Key, a.Value));
      }
      Func<TSource, TResult> function;
      return dic.TryGetValue(key, out function) ? function(element) : defaultFunc(element);
    }

    public static bool IsDefault<TSource>(this TSource element)
    {
      bool isValueType = element.GetType().IsValueType;
      return isValueType && element.Equals(default(TSource)) || element == null;
    }
  }
}
