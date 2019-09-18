using System;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public unsafe class PixHelper : IPixHelper
  {
    public ImageFileFormat GetFileFormat(ReadOnlySpan<byte> span)
    {
      fixed (byte* p = span)
      {
        if (Leptonica5Pix.findFileFormatBuffer(p, out var format) != 0)
          return ImageFileFormat.Unknown;

        return format;
      }
    }
  }
}