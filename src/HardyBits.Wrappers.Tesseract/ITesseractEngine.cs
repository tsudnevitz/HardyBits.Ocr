using System;
using System.Threading.Tasks;
using HardyBits.Wrappers.Leptonica.Pix;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Wrappers.Tesseract
{
  public interface ITesseractEngine : IDisposable
  {
    IRecognitionResult Process(IPix image);
  }
}