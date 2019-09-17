using System;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class PixColormap : LeptonicaObjectBase, IPixColormap
  {
    internal PixColormap(IntPtr pointer)
      : base(pointer)
    {
    }

    public PixColormap(int depth)
      : base(() => Leptonica5Pix.pixcmapCreate(depth))
    {
    }

    public bool AddColor(IPixColor color)
    {
      return Leptonica5Pix.pixcmapAddColor(HandleRef, color.Red, color.Green, color.Blue) == 0;
    }

    protected override void DestroyObject(ref IntPtr pointer)
    {
      Leptonica5Pix.pixcmapDestroy(ref pointer);
    }
  }
}