using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace HardyBits.Ocr.Engine.Configuration
{
  [DebuggerStepThrough]
  public class ParameterCollection : Dictionary<string, IParameterValue>, IParameterCollection
  {
    public ParameterCollection() 
      : base(StringComparer.InvariantCultureIgnoreCase)
    {
    }

    public bool TryGetValue<T>(string key, out T value)
    {
      if (key == null)
        throw new ArgumentNullException(nameof(key));

      value = default;
      if (!TryGetValue(key, out var val))
        return false;

      if (typeof(T).IsEnum)
      {
        if (!(val.BoxedValue is string stringVal))
          return false;

        if (!Enum.GetNames(typeof(T)).Contains(stringVal))
          return false;

        value = (T) Enum.Parse(typeof(T), stringVal);
        return true;
      }

      if (!(val.BoxedValue is T))
        return false;

      value = (T) val.BoxedValue;
      return true;
    }

    public T GetValue<T>(string key)
    {
      if (!TryGetValue(key, out T value))
        throw new KeyNotFoundException($"Key '{key}' not found or is of invalid type.");

      return value;
    }
  }
}