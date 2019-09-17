using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Tesseract.Enums;

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

        Data = File.ReadAllBytes(@"Samples\sample_scanned.pdf");
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
        {"mode", ParameterValue.Create("Default")},
        {"tessdata", ParameterValue.Create($@"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\libs\tessdata")}
      };

      public string TessData => Parameters.GetValue<string>("tessdata");
      public EngineMode EngineMode => Parameters.GetValue<EngineMode>("mode");
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
