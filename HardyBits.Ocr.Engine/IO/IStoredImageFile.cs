using System;

namespace HardyBits.Ocr.Engine.IO
{
  public interface IStoredImageFile : IDisposable
  {
    string OriginalName { get; }
    string OriginalExtension { get; }
    string Path { get; }
  }
}