using System;
using HardyBits.Wrappers.Leptonica.Enums;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IPixHelper
  {
    ImageFileFormat GetFileFormat(ReadOnlyMemory<byte> memory);
  }
}
