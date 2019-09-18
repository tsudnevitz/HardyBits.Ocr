using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HardyBits.Ocr.Engine.Extensions
{
  public static class StreamExtensions
  {
    public static async Task WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
      if (MemoryMarshal.TryGetArray(buffer, out var array))
      {
        await stream.WriteAsync(array.Array, array.Offset, array.Count, cancellationToken);
      }
      else
      {
        var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        buffer.Span.CopyTo(sharedBuffer);
        await FinishWriteAsync(stream.WriteAsync(sharedBuffer, 0, buffer.Length, cancellationToken), sharedBuffer);
      }
    }

    private static async Task FinishWriteAsync(Task writeTask, byte[] localBuffer)
    {
      try
      {
        await writeTask.ConfigureAwait(false);
      }
      finally
      {
        ArrayPool<byte>.Shared.Return(localBuffer);
      }
    }

  }
}