using System;
using System.Collections.Generic;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Leptonica.Filters;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  internal class PreprocessorFactory : IPreprocessorFactory
  {
    private readonly ILeptonicaInterop _leptonicaInterop;
    private readonly Dictionary<string, IPreprocessor> _preprocessors;

    public PreprocessorFactory() : this(new LeptonicaInterop())
    {
      _preprocessors = new Dictionary<string, IPreprocessor>();
      var hvPrep = new HeavyLiftPreprocessor(_leptonicaInterop);
      _preprocessors.Add(hvPrep.Type, hvPrep);
    }

    internal PreprocessorFactory(ILeptonicaInterop leptonicaInterop)
    {
      _leptonicaInterop = leptonicaInterop ?? throw new ArgumentNullException(nameof(leptonicaInterop));
    }

    public IPreprocessor Create(IPreprocessorConfiguration config)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      if (_preprocessors.TryGetValue(config.Type, out var preprocessor))
        return preprocessor;

      throw new ApplicationException($"Preprocessor {config.Type} not found.");
    }
  }
}