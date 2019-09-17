using System;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public unsafe class PixHelper : IPixHelper
  {
    public ImageFileFormat GetFileFormat(ReadOnlyMemory<byte> memory)
    {
      using var pointer = memory.Pin();
      if (Leptonica5Pix.findFileFormatBuffer(pointer.Pointer, out var format) != 0)
        return ImageFileFormat.Unknown;

      return format;
    }
  }
}