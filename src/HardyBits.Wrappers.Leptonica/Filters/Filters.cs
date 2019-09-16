using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
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
      var newPix = TransformOrThrow(() => Leptonica5Filters.pixFindSkewAndDeskew(pix.HandleRef, searchReductionFactor, out angle, out confidence));
      return new DeskewFilterResult(newPix, angle, confidence);
    }

    public FixOrientationResult FixOrientation(IPix pix, float minimumConfidence = 8f, float minimumRatio = 2.5f)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      float pageUpConfidence = 0;
      float pageLeftConfidence = 0;
      int pageRotation = 0;
      var newPix = TransformOrThrow(() => Leptonica5Filters.pixOrientCorrect(pix.HandleRef, minimumConfidence, minimumRatio, out pageUpConfidence, out pageLeftConfidence, out pageRotation, 0));
      return new FixOrientationResult(newPix, pageUpConfidence, pageLeftConfidence, pageRotation);
    }

    public static int page;
    public unsafe PixFilterResult Test(IPix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      using var pixa = new Pixa();
      pixa.AddPix(pix);

      using var deskewed = new Pix.Pix(Leptonica5Filters.pixDeskewBoth(pix.HandleRef.Handle, 0));
      pixa.AddPix(deskewed);
      using var pix1bpp = new Pix.Pix(Leptonica5Filters.pixPrepare1bpp(deskewed.HandleRef, null, 0, 0));
      pixa.AddPix(pix1bpp);
      using var oriented = new Pix.Pix(Leptonica5Filters.pixOrientCorrect(pix1bpp.HandleRef, 4f, 2.5f, out _, out _, out var rotation, 0));
      pixa.AddPix(oriented);
      //var radians = rotation * (float) Math.PI / 180f;
      //using var rotated = new Pix.Pix(Leptonica5Filters.pixRotate(deskewed.HandleRef.HandleRef, radians, 3, 1, oriented.Width, oriented.Height));
      //pixa.AddPix(rotated);
      var boxPtr = Leptonica5Filters.pixFindPageForeground(oriented.HandleRef, 128, 50, 70, 0, IntPtr.Zero);
      using var boxa = new Boxa(1);
      boxa.AddBox(boxPtr);
      //Leptonica5Filters.pixFindLargestRectangle(oriented.HandleRef, 0, out IntPtr boxPtr, out IntPtr drawnPtr);
      //Leptonica5Filters.pixFindLargeRectangles(oriented.HandleRef.HandleRef, 0, 100, out _, out IntPtr drawnPtrs);
      var color = (255 << 24) + (10 << 16) + (10 << 8) + (100);
      var clipped = new Pix.Pix(Leptonica5Filters.pixClipRectangle(oriented.HandleRef, new HandleRef(this, boxPtr), IntPtr.Zero));
      using var painted = new Pix.Pix(Leptonica5Filters.pixPaintBoxa(oriented.HandleRef.Handle, boxa.HandleRef.Handle, color));
      //using var drawnPix = new Pix.Pix(drawnPtr);
      //using var drawnPixs = new Pix.Pix(drawnPtrs);
      //pixa.AddPix(drawnPix);
      //pixa.AddPix(drawnPixs);
      pixa.AddPix(painted);
      pixa.AddPix(clipped);

      var cur = Interlocked.Increment(ref page);
      Leptonica5Pix.pixaWriteMultipageTiff($"changed{cur}.tif", pixa.HandleRef.Handle);
      Start($"changed{cur}.tif");
      
      return new PixFilterResult(clipped);
    }

    private static void Start(string path)
    {
      var process = new Process();
      process.StartInfo.FileName = "explorer";
      process.StartInfo.Arguments = "\"" + path + "\"";
      process.Start();
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