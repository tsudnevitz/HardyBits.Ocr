using System;
using System.Threading.Tasks;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine
{
  public interface IRecognitionEngine : IDisposable
  {
    Task<IRecognitionResults> RecognizeAsync(IRecognitionConfiguration config, bool isAsync = true);
    IRecognitionResults Recognize(IRecognitionConfiguration config, bool isAsync = true);
  }
}
