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
    private class ImageData : IImageData
    {
      public string Name { get; }
      public string Extension { get; }
      public ReadOnlyMemory<byte> Data { get; }

      public ImageData(string filePath)
      {
        if (filePath == null)
          throw new ArgumentNullException(nameof(filePath));

        Data = File.ReadAllBytes(filePath);
        Name = Path.GetFileNameWithoutExtension(filePath);
        Extension = Path.GetExtension(filePath);
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
      Image = new ImageData(filePath);
      Preprocessors = Enumerable.Repeat(new PreprocessorConfiguration(), 1).ToArray();
    }

    public IImageData Image { get; }
    public IEngineConfiguration Engine { get; }
    public IReadOnlyCollection<IPreprocessorConfiguration> Preprocessors { get; }
  }
}
