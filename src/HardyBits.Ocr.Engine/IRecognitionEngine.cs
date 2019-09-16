using System;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine
{
  public interface IRecognitionEngine : IDisposable
  {
    Task<IRecognitionResults> RecognizeAsync(IRecognitionConfiguration config, bool runParallel = true);
    IRecognitionResults Recognize(IRecognitionConfiguration config, bool runParallel = true);
  }
}
