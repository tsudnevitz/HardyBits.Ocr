using System;

namespace HardyBits.Ocr.Engine.Configuration
{
  internal class PreprocessorConfiguration : IPreprocessorConfiguration
  {
    public PreprocessorConfiguration(string preprocessorType, IParameterCollection parameters)
    {
      Type = preprocessorType ?? throw new ArgumentNullException(nameof(preprocessorType));
      Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    public string Type { get; }
    public IParameterCollection Parameters { get; }
  }
}