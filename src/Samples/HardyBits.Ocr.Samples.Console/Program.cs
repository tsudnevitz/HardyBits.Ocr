using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Tesseract.Enums;

namespace HardyBits.Ocr.Samples.Console
{
  public static class Program
  {
    private class RecognitionConfiguration : IRecognitionConfiguration
    {
      private class ImageData : IImageData
      {
        public string Name { get; } = "sample_scanned";
        public string Extension { get; } = ".tif";
        public string MimeType { get; } = "image/tiff";
        public ReadOnlyMemory<byte> Data { get; } = File.ReadAllBytes(@"Samples\sample_scanned.tif");
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
        public string Type { get; } = "Any";
        public string Method { get; } = "Any";
        public IParameterCollection Parameters { get; } = new ParameterCollection();
      }

      public RecognitionConfiguration()
      {
        Engine = new EngineConfiguration();
        Image = new ImageData();
        Preprocessors = Enumerable.Repeat(new PreprocessorConfiguration(), 1).ToArray();
      }

      public IImageData Image { get; }
      public IEngineConfiguration Engine { get; }
      public IReadOnlyCollection<IPreprocessorConfiguration> Preprocessors { get; }
    }

    public static async Task Main()
    {
      using var engine = new ImageRecognitionEngine();
      var config = new RecognitionConfiguration();
      var result = await engine.RecognizeAsync(config, isAsync:false);

      foreach (var page in result)
      {
        System.Console.WriteLine(page.Text);
      }

      System.Console.ReadLine();
    }
  }
}
