using System;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;
using HardyBits.Wrappers.Leptonica.Internals;

namespace HardyBits.Wrappers.Leptonica.Interop
{
  public class LeptonicaInterop : ILeptonicaInterop
  {
    public IPix DeskewBoth(IPix pix, DeskewReductionFactor searchReduction)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pointer = Leptonica5Filters.pixDeskewBoth(pix.HandleRef.Handle, (int) searchReduction).GetPointerOrThrow();
      return new Pix(pointer);
    }

    public IPix PrepareOneBitPerPixel(IPix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pointer = Leptonica5Filters.pixPrepare1bpp(pix.HandleRef.Handle, IntPtr.Zero, 0, 0).GetPointerOrThrow();
      return new Pix(pointer);
    }

    public IPix OrientationCorrect(IPix pix, float minUpConfidence = 4f, float minRatio = 2.5f)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pointer = Leptonica5Filters.pixOrientCorrect(pix.HandleRef.Handle, minUpConfidence, minRatio, out _, out _, out _, 0).GetPointerOrThrow();
      return new Pix(pointer);
    }

    public IBox FindPageForeground(IPix pix, int threshold = 128, int minDistance = 50, int eraseDistance = 70)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pointer = Leptonica5Filters.pixFindPageForeground(pix.HandleRef.Handle, threshold, minDistance, eraseDistance, 0, IntPtr.Zero).GetPointerOrThrow();
      return new Box(pointer);
    }

    public IPix ClipRectangle(IPix pix, IBox box)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pointer = Leptonica5Filters.pixClipRectangle(pix.HandleRef.Handle, box.HandleRef.Handle, IntPtr.Zero).GetPointerOrThrow();
      return new Pix(pointer);
    }
  }
}