using System;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class Pixa : LeptonicaObjectBase, IPixa
  {
    public Pixa(int initialPointersCount = 0)
      : base(() => Leptonica5Pix.pixaCreate(initialPointersCount))
    {
    }
    
    public void AddPix(IPix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      if (Leptonica5Pix.pixaAddPix(HandleRef.Handle, pix.HandleRef.Handle, 1) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    public void AddPix(IntPtr pixPointer)
    {
      if (Leptonica5Pix.pixaAddPix(HandleRef.Handle, pixPointer, 1) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    protected override void DestroyObject(ref IntPtr pointer) => Leptonica5Pix.pixaDestroy(ref pointer);
  }
}
