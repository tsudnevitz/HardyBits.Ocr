using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;


namespace HardyBits.Wrappers.Leptonica.Pix
{
  public class Boxa : IDisposable
  {
    public Boxa(int initialPointersCount = 0)
    {
      HandleRef = Leptonica5Pix.boxaCreate(initialPointersCount).GetHandleOrThrow(this);
    }

    public HandleRef HandleRef { get; private set; }

    public void AddBox(IntPtr boxPointer)
    {
      if (Leptonica5Pix.boxaAddBox(HandleRef.Handle, boxPointer, 0) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    private void ReleaseUnmanagedResources()
    {
      if (HandleRef.Handle == IntPtr.Zero)
        return;

      var pointer = HandleRef.Handle;
      HandleRef = new HandleRef();
      Leptonica5Pix.boxaDestroy(ref pointer);
    }

    public void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~Boxa()
    {
      ReleaseUnmanagedResources();
    }
  }
}
