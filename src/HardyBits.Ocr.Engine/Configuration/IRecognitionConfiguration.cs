using System.Collections.Generic;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IRecognitionConfiguration
  {
    IFileConfiguration File { get; }
    IEngineConfiguration Engine { get; }
    IReadOnlyCollection<IPreprocessorConfiguration> Preprocessors { get; }
  }
}