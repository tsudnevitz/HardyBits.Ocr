using System;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPixa : ILeptonicaObject
  {
    void AddPix(IPix pix);
    void AddPix(IntPtr pixPointer);
  }
}