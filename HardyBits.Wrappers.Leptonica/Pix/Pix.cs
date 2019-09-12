using System;
using System.Linq;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public class Pix : IPix
  {
    private static readonly int[] AllowedDepths = { 1, 2, 4, 8, 16, 32 };

    internal Pix(int imageWidth, int imageHeight, int imageDepth)
    {
      if (!AllowedDepths.Contains(imageDepth))
        throw new ArgumentException($"Depth must be {string.Join(", ", AllowedDepths)} bits.", nameof(imageDepth));

      if (imageWidth <= 0) 
        throw new ArgumentException("Width must be greater than zero", nameof(imageWidth));

      if (imageHeight <= 0) 
        throw new ArgumentException("Height must be greater than zero", nameof(imageHeight));

      Handle = Leptonica5Pix.pixCreate(imageWidth, imageHeight, imageDepth).GetHandleOrThrow(this);
      Width = imageWidth;
      Height = imageHeight;
      Depth = imageDepth;
    }

    internal Pix(string imageFilePath)
    {
      if (imageFilePath == null)
        throw new ArgumentNullException(nameof(imageFilePath));

      Handle = Leptonica5Pix.pixRead(imageFilePath).GetHandleOrThrow(this);
      Width = Leptonica5Pix.pixGetWidth(Handle);
      Height = Leptonica5Pix.pixGetHeight(Handle);
      Depth = Leptonica5Pix.pixGetDepth(Handle);
    }

    internal Pix(IntPtr pointer)
    {
      Handle = pointer.GetHandleOrThrow(this);
      Width = Leptonica5Pix.pixGetWidth(Handle);
      Height = Leptonica5Pix.pixGetHeight(Handle);
      Depth = Leptonica5Pix.pixGetDepth(Handle);
    }

    public int Width { get; }
    public int Height { get; }
    public int Depth { get; }

    public int XRes 
    {
      get => Leptonica5Pix.pixGetXRes(Handle);
      private set => Leptonica5Pix.pixSetXRes(Handle, value);
    }

    public int YRes
    {
      get => Leptonica5Pix.pixGetYRes(Handle);
      private set => Leptonica5Pix.pixSetYRes(Handle, value);
    }

    public HandleRef Handle { get; private set; }

    public IPix Clone()
    {
      var pointer = Leptonica5Pix.pixClone(Handle).GetPointerOrThrow();
      return new Pix(pointer);
    }

    private void ReleaseUnmanagedResources()
    {
      var tmpHandle = Handle.Handle;
      Leptonica5Pix.pixDestroy(ref tmpHandle);
      Handle = new HandleRef(this, IntPtr.Zero);
    }

    public void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~Pix()
    {
      ReleaseUnmanagedResources();
    }
  }
}