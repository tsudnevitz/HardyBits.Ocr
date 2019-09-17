using System;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPixData
  {
    IntPtr Data { get; }
    int WordsPerLine { get; }
  }
}