using System;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal class Pix : LeptonicaObjectBase, IPix
  {
    internal Pix(int imageWidth, int imageHeight, int imageDepth)
      : base(() => Leptonica5Pix.pixCreate(imageWidth, imageHeight, imageDepth))
    {
      Width = imageWidth;
      Height = imageHeight;
      Depth = imageDepth;
    }

    internal Pix(string imageFilePath)
      : base(() => Leptonica5Pix.pixRead(imageFilePath))
    {
      Width = Leptonica5Pix.pixGetWidth(HandleRef);
      Height = Leptonica5Pix.pixGetHeight(HandleRef);
      Depth = Leptonica5Pix.pixGetDepth(HandleRef);
    }

    internal Pix(IntPtr pointer)
      : base(pointer)
    {
      Width = Leptonica5Pix.pixGetWidth(HandleRef);
      Height = Leptonica5Pix.pixGetHeight(HandleRef);
      Depth = Leptonica5Pix.pixGetDepth(HandleRef);
    }

    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    public int XRes 
    {
      get => Leptonica5Pix.pixGetXRes(HandleRef);
      private set => Leptonica5Pix.pixSetXRes(HandleRef, value);
    }

    public int YRes
    {
      get => Leptonica5Pix.pixGetYRes(HandleRef);
      private set => Leptonica5Pix.pixSetYRes(HandleRef, value);
    }

    public IPix Clone()
    {
      var pointer = Leptonica5Pix.pixClone(HandleRef).GetPointerOrThrow();
      return new Pix(pointer);
    }

    protected override void DestroyObject(ref IntPtr pointer) => Leptonica5Pix.pixDestroy(ref pointer);
  }
}