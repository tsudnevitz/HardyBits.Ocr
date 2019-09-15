using System;
using System.IO;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Engine.IO
{
  public class ImageFileStorage : IImageFileStorage
  {
    public async Task<IStoredImageFile> StoreAsync(ReadOnlyMemory<byte> data)
    {
      var tmpFilePath = Path.GetTempFileName();
      await using var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable, 4096, FileOptions.Asynchronous);
      await stream.WriteAsync(data);

      return new StoredImageFile(tmpFilePath);
    }

    public async Task<IStoredImageFile> StoreAsync(IImageData imageData)
    {
      var tmpFilePath = Path.GetTempFileName();
      await using var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable, 4096, FileOptions.Asynchronous);
      await stream.WriteAsync(imageData.Data);

      return new StoredImageFile(tmpFilePath, imageData.Name, imageData.Extension);
    }

    public IStoredImageFile Wrap(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
      
      return new StoredImageFile(path);
    }
  }
}
