using System;
using System.Runtime.InteropServices;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface ILeptonicaObject : IDisposable
  {
    HandleRef HandleRef { get; }
  }
}