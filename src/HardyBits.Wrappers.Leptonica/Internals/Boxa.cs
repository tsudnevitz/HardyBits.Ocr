using System;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class Boxa : LeptonicaObjectBase
  {
    public Boxa(int initialPointersCount = 0)
     : base(() => Leptonica5Pix.boxaCreate(initialPointersCount))
    {
    }

    public void AddBox(IntPtr boxPointer)
    {
      if (Leptonica5Pix.boxaAddBox(HandleRef.Handle, boxPointer, 0) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    protected override void DestroyObject(ref IntPtr pointer) => Leptonica5Pix.boxaDestroy(ref pointer);
  }
}
