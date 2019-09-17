using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Constants;

namespace HardyBits.Wrappers.Leptonica.Imports
{
  internal static class Leptonica5Filters
  {
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindSkewAndDeskew))]
    public static extern IntPtr pixFindSkewAndDeskew(IntPtr pixs, int redsearch, out float pangle, out float pconf);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixOrientCorrect))]
    public static extern IntPtr pixOrientCorrect(IntPtr pixs, float minupconf, float minratio, out float pupconf, out float pleftconf, out int protation, int debug);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindLargestRectangle))]
    public static extern int pixFindLargestRectangle(IntPtr pixs, int polarity, out IntPtr pbox, out IntPtr ppixdb);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindLargeRectangles))]
    public static extern int pixFindLargeRectangles(IntPtr pixs, int polarity, int nrect, out IntPtr pboxa, out IntPtr ppixdb);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixPrepare1bpp))]
    public static extern IntPtr pixPrepare1bpp(IntPtr pixs, IntPtr box, float cropfract, int outres);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixWriteJpeg))]
    public static extern int pixWriteJpeg (string filename, IntPtr pix, int quality, int progressive);

    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(boxGetGeometry))]
    public static extern int boxGetGeometry(IntPtr box, out int px, out int py, out int pw, out int ph);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixClipRectangle))]
    public static extern IntPtr pixClipRectangle (IntPtr pixs, IntPtr box, IntPtr pboxc);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindPageForeground))]
    public static extern IntPtr pixFindPageForeground(IntPtr pixs, int threshold, int mindist, int erasedist, int showmorph, IntPtr pixac);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixDeskewBoth))]
    public static extern IntPtr pixDeskewBoth(IntPtr pixs, int redsearch);

    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixRotate))]
    public static extern IntPtr pixRotate(IntPtr pixs, float angle, int type, int incolor, int width, int height);

    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixPaintBoxa))]
    public static extern IntPtr pixPaintBoxa(IntPtr pixs, IntPtr boxa, int val);
  }
}