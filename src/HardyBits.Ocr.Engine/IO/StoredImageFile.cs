using System;
using System.IO;

namespace HardyBits.Ocr.Engine.IO
{
  public class StoredImageFile : IStoredImageFile
  {
    public StoredImageFile(string path, string originalName, string originalExtension)
    {
      Path = path ?? throw new ArgumentNullException(nameof(path));
      OriginalName = originalName ?? throw new ArgumentNullException(nameof(originalName));
      OriginalExtension = originalExtension ?? throw new ArgumentNullException(nameof(originalExtension));
    }

    public StoredImageFile(string path)
    {
      Path = path ?? throw new ArgumentNullException(nameof(path));
      OriginalName = System.IO.Path.GetFileNameWithoutExtension(path);
      OriginalExtension = System.IO.Path.GetExtension(path);
    }

    public string OriginalName { get; }
    public string OriginalExtension { get; }
    public string Path { get; }

    private void TryDeleteFile()
    {
      try
      {
        if (Path != null && File.Exists(Path))
          File.Delete(Path);
      }
      catch
      {
      }
    }

    public void Dispose()
    {
      TryDeleteFile();
    }

    ~StoredImageFile()
    {
      Dispose();
    }
  }
}