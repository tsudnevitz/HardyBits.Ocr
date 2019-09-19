using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Wrappers.Leptonica.Enums;

namespace HardyBits.Ocr.Samples.Console
{
  public static class Program
  {
    public static async Task Main()
    { 
      var config1 = new RecognitionConfigurationBuilder()
        .ConfigureEngine(x => x.TessData, $@"{Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location)}\tessdata")
        .ConfigureEngine(x => x.EngineMode, 3)
        .ConfigureEngine(x => x.Language, "eng")
        .ConfigureFilePath(@"Samples\sample_photo_1_side.jpg")
        .AddPreprocessor<HeavyLiftPreprocessor>(x => x.SetParameters(new
          {
            ReductionFactor = DeskewReductionFactor.Default,
            MinUpConfidence = 4f,
            MinRatio = 2.5f,
            MinDistance = 50,
            EraseDistance = 70
          }))
        .Build();

      var config2 = new RecognitionConfigurationBuilder()
        .ConfigureFrom(config1)
        .ConfigureFilePath(@"Samples\sample_scanned.tif")
        .Build();

      using var engine = new ImageRecognitionEngine();
      var task1 = engine.RecognizeAsync(config1, runParallel: true);
      var task2 = engine.RecognizeAsync(config2, runParallel: true);

      var results = await Task.WhenAll(task1, task2);
      foreach (var page in results.SelectMany(x => x))
        System.Console.WriteLine(page.Text);

      System.Console.WriteLine("Finished!");
      System.Console.ReadLine();
    }
  }
}
