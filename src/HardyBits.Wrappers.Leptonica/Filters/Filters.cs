using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Imports;
using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public class Filters : IFilters
  {
    public DeskewFilterResult Deskew(IPix pix, int searchReductionFactor)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      float angle = 0, confidence = 0;
      var newPix = TransformOrThrow(() => Leptonica5Filters.pixFindSkewAndDeskew(pix.Handle, searchReductionFactor, out angle, out confidence));
      return new DeskewFilterResult(newPix, angle, confidence);
    }

    public FixOrientationResult FixOrientation(IPix pix, float minimumConfidence = 8f, float minimumRatio = 2.5f)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      float pageUpConfidence = 0;
      float pageLeftConfidence = 0;
      int pageRotation = 0;
      var newPix = TransformOrThrow(() => Leptonica5Filters.pixOrientCorrect(pix.Handle, minimumConfidence, minimumRatio, out pageUpConfidence, out pageLeftConfidence, out pageRotation, 0));
      return new FixOrientationResult(newPix, pageUpConfidence, pageLeftConfidence, pageRotation);
    }

    public unsafe PixFilterResult Test(IPix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      var pixa = new Pixa();
      pixa.AddPix(pix);

      float angle = 0, confidence = 0;
      //var boxPtr = Leptonica5Filters.pixFindPageForeground(pix.Handle, 128, 1, 1, 0, IntPtr.Zero);
      //var clipped = new HandleRef(this, Leptonica5Filters.pixClipRectangle(pix.Handle, new HandleRef(this, boxPtr), IntPtr.Zero));
      //pixa.AddPix(clipped.Handle);
      var pix1bpp = new HandleRef(this, Leptonica5Filters.pixPrepare1bpp(pix.Handle, null, 0, 0));
      pixa.AddPix(pix1bpp.Handle);
      var deskewed = new HandleRef(this, Leptonica5Filters.pixFindSkewAndDeskew(pix1bpp, 4, out angle, out confidence));
      pixa.AddPix(deskewed.Handle);
      var oriented = new HandleRef(this, Leptonica5Filters.pixOrientCorrect(deskewed, 4f, 2.5f, out _, out _, out _, 0));
      pixa.AddPix(oriented.Handle);

      Leptonica5Pix.pixaWriteMultipageTiff("changed.tif", pixa.HandleRef.Handle);
      Start("changed.tif");

      //var deskewed2 = new HandleRef(this, Leptonica5Filters.pixFindSkewAndDeskew(oriented, 0, out var angle, out var confidence));

      //var isRectangleFound = Leptonica5Filters.pixFindLargestRectangle(deskewed2, 1, out var box, IntPtr.Zero);
      //var result = Leptonica5Filters.boxGetGeometry(new HandleRef(this, box), out int px, out int py, out int pw, out int ph);
      
      return new PixFilterResult(new Pix.Pix(oriented.Handle));
    }

    private static void Start(string path)
    {
      Process fileopener = new Process();
      fileopener.StartInfo.FileName = "explorer";
      fileopener.StartInfo.Arguments = "\"" + path + "\"";
      fileopener.Start();
    }

    private static Pix.Pix TransformOrThrow(Func<IntPtr> action)
    {
      var ptr = action();
      if (ptr == IntPtr.Zero)
        throw new InvalidOperationException("Null pointer returned. Operation failed.");

      return new Pix.Pix(ptr);
    }

    public class PixFilterResult : IFilterResult
    {
      public PixFilterResult(IPix pix)
      {
        Pix = pix;
      }

      public IPix Pix { get; }
    }
  }
}