using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Samples.Console
{
  internal class RecognitionConfiguration : IRecognitionConfiguration
  {
    private class FileConfiguration : IFileConfiguration
    {
      public string Name { get; }
      public string Extension { get; }
      public Stream DataStream { get; }
      public bool DisposeStream { get; } = true;

      public FileConfiguration(string filePath)
      {
        if (filePath == null)
          throw new ArgumentNullException(nameof(filePath));

        DataStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        Name = Path.GetFileNameWithoutExtension(filePath);
        Extension = Path.GetExtension(filePath);
      }

      public void Dispose()
      {
        // does nothing
      }
    }

    private class EngineConfiguration : IEngineConfiguration
    {
      public string Type { get; } = "tesseract4";
      public IParameterCollection Parameters { get; } = new ParameterCollection
      {
        {"language", ParameterValue.Create("pol")},
        {"mode", ParameterValue.Create(3)},
        {"tessdata", ParameterValue.Create($@"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\tessdata")}
      };

      public string TessData => Parameters.GetValue<string>("tessdata");
      public int EngineMode => Parameters.GetValue<int>("mode");
      public string Language => Parameters.GetValue<string>("language");
    }

    private class PreprocessorConfiguration : IPreprocessorConfiguration
    {
      public string Type { get; } = "HeavyLift";
      public IParameterCollection Parameters { get; } = new ParameterCollection();
    }

    public RecognitionConfiguration(string filePath)
    {
      Engine = new EngineConfiguration();
      File = new FileConfiguration(filePath);
      Preprocessors = Enumerable.Repeat(new PreprocessorConfiguration(), 1).ToArray();
    }

    public IFileConfiguration File { get; }
    public IEngineConfiguration Engine { get; }
    public IReadOnlyCollection<IPreprocessorConfiguration> Preprocessors { get; }
  }
}
