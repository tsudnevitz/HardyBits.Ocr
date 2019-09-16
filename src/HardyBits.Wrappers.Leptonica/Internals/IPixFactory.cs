using System;
using System.Collections.Generic;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPixFactory
  {
    IEnumerable<IPix> Create(ReadOnlyMemory<byte> imageData);
    IEnumerable<IPix> Create(int imageWidth, int imageHeight, int imageDepth);
    IEnumerable<IPix> Create(string imageFilePath);
  }
}