using System;
using System.Runtime.InteropServices;
using HardyBits.Wrappers.Leptonica.Extensions;

namespace HardyBits.Wrappers.Leptonica.Internals
{
  internal abstract class LeptonicaObjectBase : ILeptonicaObject
  {
    protected LeptonicaObjectBase(Func<IntPtr> creator)
    {
      if (creator == null)
        throw new ArgumentNullException(nameof(creator));

      HandleRef = creator().GetHandleOrThrow(this);
    }

    protected LeptonicaObjectBase(IntPtr pointer)
    {
      if (pointer == null)
        throw new ArgumentNullException(nameof(pointer));

      HandleRef = new HandleRef(this, pointer);
    }

    public HandleRef HandleRef { get; private set; }

    protected abstract void DestroyObject(ref IntPtr pointer);

    private void ReleaseUnmanagedResources()
    {
      if (HandleRef.Handle == IntPtr.Zero)
        return;

      var pointer = HandleRef.Handle;
      HandleRef = new HandleRef();
      DestroyObject(ref pointer);
    }

    public virtual void Dispose()
    {
      ReleaseUnmanagedResources();
      GC.SuppressFinalize(this);
    }

    ~LeptonicaObjectBase()
    {
      ReleaseUnmanagedResources();
    }
  }
}