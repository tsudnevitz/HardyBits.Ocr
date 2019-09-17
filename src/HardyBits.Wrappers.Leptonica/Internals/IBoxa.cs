using System;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal interface IBoxa : ILeptonicaObject
  {
    void AddBox(IntPtr boxPointer);
  }
}