using System;
using System.Linq;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Internals
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

      HandleRef = Leptonica5Pix.pixCreate(imageWidth, imageHeight, imageDepth).GetHandleOrThrow(this);
      Width = imageWidth;
      Height = imageHeight;
      Depth = imageDepth;
    }

    internal Pix(string imageFilePath)
    {
      if (imageFilePath == null)
        throw new ArgumentNullException(nameof(imageFilePath));

      HandleRef = Leptonica5Pix.pixRead(imageFilePath).GetHandleOrThrow(this);
      Width = Leptonica5Pix.pixGetWidth(HandleRef);
      Height = Leptonica5Pix.pixGetHeight(HandleRef);
      Depth = Leptonica5Pix.pixGetDepth(HandleRef);
    }

    internal Pix(IntPtr pointer)
    {
      HandleRef = pointer.GetHandleOrThrow(this);
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

    public HandleRef HandleRef { get; private set; }

    public IPix Clone()
    {
      var pointer = Leptonica5Pix.pixClone(HandleRef).GetPointerOrThrow();
      return new Pix(pointer);
    }

    private void ReleaseUnmanagedResources()
    {
      var tmpHandle = HandleRef.Handle;
      Leptonica5Pix.pixDestroy(ref tmpHandle);
      HandleRef = new HandleRef(this, IntPtr.Zero);
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