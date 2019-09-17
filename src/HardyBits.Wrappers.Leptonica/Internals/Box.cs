using System;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class Box : LeptonicaObjectBase, IBox
  {
    public Box(IntPtr pointer) : base(pointer) { }
    protected override void DestroyObject(ref IntPtr pointer) => Leptonica5Pix.boxDestroy(ref pointer);
  }
}