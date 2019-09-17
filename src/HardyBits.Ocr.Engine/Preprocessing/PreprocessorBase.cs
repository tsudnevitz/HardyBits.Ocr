using System;
using System.Runtime.CompilerServices;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Leptonica.Interop;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public abstract class PreprocessorBase<TManifest> : IPreprocessor
    where TManifest : class, IManifest, new()
  {
    protected readonly ILeptonicaInterop leptonicaInterop;
    protected readonly IParameterCollection parameters;

    protected PreprocessorBase(ILeptonicaInterop leptonicaInterop, IParameterCollection parameters)
    {
      this.leptonicaInterop = leptonicaInterop ?? throw new ArgumentNullException(nameof(leptonicaInterop));
      this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    public abstract IPix Run(IPix image);

    protected T GetParameterOrDefault<T>(T defaultValue = default, [CallerMemberName] string key = "")
    {
      if (string.IsNullOrEmpty(key))
        throw new ArgumentException("Key not specified.", nameof(key));

      return parameters.TryGetValue<T>(key, out var value) ? value : defaultValue;
    }

    protected T GetParameterOrThrow<T>([CallerMemberName] string key = "")
    {
      if (string.IsNullOrEmpty(key))
        throw new ArgumentException("Key not specified.", nameof(key));

      return parameters.GetValue<T>(key);
    }
  }
}