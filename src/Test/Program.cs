using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine;

namespace HardyBits.Ocr.Samples.Console
{
  public static class Program
  {
    public static async Task Main()
    {
      var config1 = new RecognitionConfiguration(@"C:\Users\Hubert\source\repos\HardyBits.Ocr\src\Samples\HardyBits.Ocr.Samples.Console\Samples\sample_photo_1_side.jpg");
      var config2 = new RecognitionConfiguration(@"C:\Users\Hubert\source\repos\HardyBits.Ocr\src\Samples\HardyBits.Ocr.Samples.Console\Samples\sample_photo_2_side.jpg");

      using var engine = new ImageRecognitionEngine();
      var task1 = engine.RecognizeAsync(config1, runParallel: true);
      var task2 = engine.RecognizeAsync(config2, runParallel: true);

      var results = await Task.WhenAll(task1, task2);
      var result = results.SelectMany(x => x);

      foreach (var page in result)
        System.Console.WriteLine(page.Text);

      System.Console.WriteLine("Finished!");
      System.Console.ReadLine();
    }
  }
}
