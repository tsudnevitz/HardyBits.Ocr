using System;

namespace HardyBits.Ocr.Engine.IO
{
  internal interface IImageFileTypeRecognizer
  {
    ImageFileTypes GetFileType(ReadOnlyMemory<byte> imageData);
  }
}