using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Constants;

namespace HardyBits.Wrappers.Leptonica.Imports
{
  internal static unsafe class Leptonica5Filters
  {
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindSkewAndDeskew))]
    public static extern IntPtr pixFindSkewAndDeskew(HandleRef pixs, int redsearch, out float pangle, out float pconf);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixOrientCorrect))]
    public static extern IntPtr pixOrientCorrect(HandleRef pixs, float minupconf, float minratio, out float pupconf, out float pleftconf, out int protation, int debug);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindLargestRectangle))]
    public static extern int pixFindLargestRectangle(HandleRef pixs, int polarity, out IntPtr pbox, IntPtr ppixdb);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixPrepare1bpp))]
    public static extern IntPtr pixPrepare1bpp(HandleRef pixs, void* box, float cropfract, int outres);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixWriteJpeg))]
    public static extern int pixWriteJpeg (string filename, HandleRef pix, int quality, int progressive);

    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(boxGetGeometry))]
    public static extern int boxGetGeometry(HandleRef box, out int px, out int py, out int pw, out int ph);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixClipRectangle))]
    public static extern IntPtr pixClipRectangle (HandleRef pixs, HandleRef box, IntPtr pboxc);
    
    [DllImport(LibraryNames.Leptonica5, CallingConvention = CallingConvention.Cdecl, EntryPoint = nameof(pixFindPageForeground))]
    public static extern IntPtr pixFindPageForeground(HandleRef pixs, int threshold, int mindist, int erasedist, int showmorph, IntPtr pixac);
  }
}