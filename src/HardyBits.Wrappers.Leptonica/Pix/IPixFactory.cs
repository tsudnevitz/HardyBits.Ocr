using System;
using System.Collections.Generic;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IPixFactory
  {
    IEnumerable<IPix> Create(ReadOnlyMemory<byte> imageData);
    IEnumerable<IPix> Create(int imageWidth, int imageHeight, int imageDepth);
    IEnumerable<IPix> Create(string imageFilePath);
  }
}