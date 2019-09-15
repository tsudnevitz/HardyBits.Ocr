using System;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Engine.IO
{
  public interface IImageFileStorage
  {
    Task<IStoredImageFile> StoreAsync(ReadOnlyMemory<byte> data);
    Task<IStoredImageFile> StoreAsync(IImageData imageData);
    IStoredImageFile Wrap(string path);
  }
}