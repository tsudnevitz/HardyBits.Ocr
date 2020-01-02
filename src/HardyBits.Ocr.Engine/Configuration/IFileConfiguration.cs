using System;
using System.IO;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IFileConfiguration : IDisposable
  {
    string Name { get; }
    string Extension { get; }
    Stream DataStream { get; }
    bool DisposeStream { get; }
  }
}