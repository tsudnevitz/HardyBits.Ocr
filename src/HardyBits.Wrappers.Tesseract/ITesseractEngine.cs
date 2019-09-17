using System;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Wrappers.Tesseract
{
  public interface ITesseractEngine : IDisposable
  {
    ITesseractResult Process(IPix image);
  }
}