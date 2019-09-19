using System;
using System.Collections.Generic;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.Extensions;
using HardyBits.Wrappers.Leptonica.Interop;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  internal class PreprocessorFactory : IPreprocessorFactory
  {
    private readonly ILeptonicaInterop _leptonicaInterop;
    private readonly Dictionary<string, Type> _preprocessors;

    public PreprocessorFactory() : this(new LeptonicaInterop())
    {
      _preprocessors = new Dictionary<string, Type>();
      AddPreprocessor(typeof(HeavyLiftPreprocessor));
    }

    private PreprocessorFactory(ILeptonicaInterop leptonicaInterop)
    {
      _leptonicaInterop = leptonicaInterop ?? throw new ArgumentNullException(nameof(leptonicaInterop));
    }

    private void AddPreprocessor(Type preprocessorType)
    {
      if (!preprocessorType.IsAssignableToGenericType(typeof(PreprocessorBase<>)))
        throw new ArgumentException($"Supplied type {preprocessorType.Name} does not derive from {typeof(PreprocessorBase<>).Name}.");

      var manifest = GetManifest(preprocessorType);
      _preprocessors.Add(manifest.Name, preprocessorType);
    }

    public IPreprocessor Create(IPreprocessorConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      if (_preprocessors.TryGetValue(config.Type, out var type))
        return (IPreprocessor) Activator.CreateInstance(type, _leptonicaInterop, config.Parameters);

      throw new ApplicationException($"Preprocessor {config.Type} not found.");
    }

    public static IManifest GetManifest(Type preprocessorType)
    {
      var baseType = GetPreprocessorBaseType(preprocessorType.BaseType);
      var manifestType = baseType.GetGenericArguments()[0];
      return (IManifest) Activator.CreateInstance(manifestType);
    }

    private static Type GetPreprocessorBaseType(Type baseType)
    {
      if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(PreprocessorBase<>))
        return baseType;

      if (baseType == null)
        throw new InvalidOperationException($"Supplied type does not inherit {typeof(PreprocessorBase<>).Name} type.");

      return GetPreprocessorBaseType(baseType.BaseType);
    }
  }
}
