using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;


namespace HardyBits.Wrappers.Leptonica.Pix
{
  public class Pixa : IDisposable
  {
    public Pixa(int initialPointersCount = 0)
    {
      HandleRef = Leptonica5Pix.pixaCreate(initialPointersCount).GetHandleOrThrow(this);
    }

    public HandleRef HandleRef { get; private set; }

    public void AddPix(IPix pix)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      if (Leptonica5Pix.pixaAddPix(HandleRef.Handle, pix.Handle.Handle, 0) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    public void AddPix(IntPtr pixPointer)
    {
      if (Leptonica5Pix.pixaAddPix(HandleRef.Handle, pixPointer, 0) != 0)
        throw new InvalidOperationException("Leptonica failed.");
    }

    private void ReleaseUnmanagedResources()
    {
      if (HandleRef.Handle == IntPtr.Zero)
        return;

      var pointer = HandleRef.Handle;
      HandleRef = new HandleRef();
      Leptonica5Pix.pixaDestroy(ref pointer);
    }

    public void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~Pixa()
    {
      ReleaseUnmanagedResources();
    }
  }
}
