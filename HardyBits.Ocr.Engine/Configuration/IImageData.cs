﻿using System;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IImageData
  {
    string Name { get; }
    string Extension { get; }
    string MimeType { get; }
    ReadOnlyMemory<byte> Data { get; }
  }
}