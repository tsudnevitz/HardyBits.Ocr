using System;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPixColormap : IDisposable
  {
    bool AddColor(IPixColor color);
  }
}