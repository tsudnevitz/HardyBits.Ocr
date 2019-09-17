using System;

namespace HardyBits.Wrappers.Tesseract.Results
{
  public interface ITesseractResult : IDisposable
  {
    string Text { get; }
    string HocrText { get; }
    float? Confidence { get; }
  }
}