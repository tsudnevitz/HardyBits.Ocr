using System;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IPixData
  {
    IntPtr Data { get; }
    int WordsPerLine { get; }
  }
}