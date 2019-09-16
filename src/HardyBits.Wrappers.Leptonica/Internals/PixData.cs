using System;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class PixData : IPixData
  {
    public PixData(Pix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      Data = Leptonica5Pix.pixGetData(pix.HandleRef);
      WordsPerLine = Leptonica5Pix.pixGetWpl(pix.HandleRef);
    }

    public IntPtr Data { get; }
    public int WordsPerLine { get; }
  }
}