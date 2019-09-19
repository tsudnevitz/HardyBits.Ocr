using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace HardyBits.Ocr.Engine.Configuration
{
  [DebuggerStepThrough]
  public class PreprocessorConfigurationBuilder : Dictionary<string, IParameterValue>, IParameterCollection, IPreprocessorConfigurationBuilder
  {
    public PreprocessorConfigurationBuilder() 
      : base(StringComparer.InvariantCultureIgnoreCase)
    {
    }

    public PreprocessorConfigurationBuilder(IParameterCollection parameters)
      : base(StringComparer.InvariantCultureIgnoreCase)
    {
      if (parameters == null)
        throw new ArgumentNullException(nameof(parameters));

      // ToDo: do not copy by references?
      foreach (var parameter in parameters)
        Add(parameter.Key, parameter.Value);
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

    public IPreprocessorConfigurationBuilder SetParameter<T>(string paramName, T paramValue)
    {
      if (paramName == null)
        throw new ArgumentNullException(nameof(paramName));

      if (ContainsKey(paramName))
        this[paramName] = ParameterValue.Create(paramValue);
      else
        Add(paramName, ParameterValue.Create(paramValue));

      return this;
    }

    public IPreprocessorConfigurationBuilder SetParameters(object anonymousObject)
    {
      if (anonymousObject == null)
        throw new ArgumentNullException(nameof(anonymousObject));

      foreach (var prop in anonymousObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        SetParameter(prop.Name, prop.GetValue(anonymousObject));

      return this;
    }

    public IPreprocessorConfiguration Build(string preprocessorType)
    {
      return new PreprocessorConfiguration(preprocessorType, this);
    }
  }
}