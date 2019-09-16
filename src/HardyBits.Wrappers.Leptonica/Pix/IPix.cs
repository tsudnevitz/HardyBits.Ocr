using System;
using System.Runtime.InteropServices;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IPix : IDisposable
  {
    int Width { get; }
    int Height { get; }
    int Depth { get; }
    int XRes { get; }
    int YRes { get; }
    HandleRef Handle { get; }
    IPix Clone();
  }
}