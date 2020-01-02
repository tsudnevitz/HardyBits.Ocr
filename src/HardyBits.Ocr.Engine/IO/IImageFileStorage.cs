using System;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Engine.IO
{
  public interface IImageFileStorage
  {
    ValueTask<IStoredImageFile> StoreAsync(IFileConfiguration fileConfiguration);
    ValueTask<IStoredImageFile> StoreAsync(ReadOnlyMemory<byte> memory);
    IStoredImageFile Wrap(string path);
  }
}