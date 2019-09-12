using System;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public interface IPixColormap : IDisposable
  {
    bool AddColor(IPixColor color);
  }
}