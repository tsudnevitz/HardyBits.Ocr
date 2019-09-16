using System;
using System.Runtime.InteropServices;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IBox : IDisposable
  {
    HandleRef HandleRef { get; }
  }
}