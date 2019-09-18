using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine;

namespace HardyBits.Ocr.Samples.Console
{
  public static class Program
  {
    public static async Task Main()
    {
      var config1 = new RecognitionConfiguration(@"Samples\sample_photo_1_side.jpg");
      var config2 = new RecognitionConfiguration(@"Samples\sample_photo_2_side.jpg");

      using var engine = new ImageRecognitionEngine();
      var result = await engine.RecognizeAsync(config1, runParallel: false);

      //var result = await task1;//Task.WhenAll(task1, task2);
      //var result = results.SelectMany(x => x);

      foreach (var page in result)
        System.Console.WriteLine(page.Text);

      System.Console.WriteLine("Finished!");
      System.Console.ReadLine();
    }
  }
}
