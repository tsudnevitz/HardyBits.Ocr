using System;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPixColormap : ILeptonicaObject
  {
    bool AddColor(IPixColor color);
  }
}