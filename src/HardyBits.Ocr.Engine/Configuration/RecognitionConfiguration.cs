using System.Collections.Generic;
using System.IO;

namespace HardyBits.Ocr.Engine.Configuration
{
  internal class RecognitionConfiguration : IRecognitionConfiguration, IFileConfiguration, IEngineConfiguration
  {
    // File
    public string Name { get; set; }
    public string Extension { get; set; }
    public Stream DataStream { get; set; }
    public bool DisposeStream { get; set; } = true;

    // Engine
    public string TessData { get; set; }
    public int EngineMode { get; set; }
    public string Language { get; set; }

    public RecognitionConfiguration()
    {
    }

    public IFileConfiguration File => this;
    public IEngineConfiguration Engine => this;
    public IReadOnlyCollection<IPreprocessorConfiguration> Preprocessors { get; set; }

    public void Dispose()
    {
      if (DisposeStream)
        DataStream?.Dispose();
    }
  }
}
