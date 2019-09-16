using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public class Box : IBox
  {
    public Box(IntPtr pointer)
    {
      if (pointer == null)
        throw new ArgumentNullException(nameof(pointer));

      HandleRef = new HandleRef(this, pointer);
    }

    public HandleRef HandleRef { get; private set; }

    private void ReleaseUnmanagedResources()
    {
      if (HandleRef.Handle == IntPtr.Zero)
        return;

      var pointer = HandleRef.Handle;
      HandleRef = new HandleRef();
      Leptonica5Pix.boxDestroy(ref pointer);
    }

    public void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~Box()
    {
      ReleaseUnmanagedResources();
    }
  }
}