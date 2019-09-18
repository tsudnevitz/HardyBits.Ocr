using System;
using System.IO;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.Extensions;

namespace HardyBits.Ocr.Engine.IO
{
  public class ImageFileStorage : IImageFileStorage
  {
    public async ValueTask<IStoredImageFile> StoreAsync(IFileConfiguration fileConfiguration)
    {
      var tmpFilePath = Path.GetTempFileName();
      using var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable, 4096, FileOptions.Asynchronous);

      fileConfiguration.DataStream.Position = 0;
      await fileConfiguration.DataStream.CopyToAsync(stream);

      return new StoredImageFile(tmpFilePath, fileConfiguration.Name, fileConfiguration.Extension);
    }

    public async ValueTask<IStoredImageFile> StoreAsync(ReadOnlyMemory<byte> memory)
    {
      var tmpFilePath = Path.GetTempFileName();
      using var stream = new FileStream(tmpFilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete | FileShare.Inheritable, 4096, FileOptions.Asynchronous);

      await stream.WriteAsync(memory);

      return new StoredImageFile(tmpFilePath);
    }

    public IStoredImageFile Wrap(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));
      
      return new StoredImageFile(path);
    }
  }
}
