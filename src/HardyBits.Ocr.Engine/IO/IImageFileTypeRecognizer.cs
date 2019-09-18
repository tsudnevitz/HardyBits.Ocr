using System.IO;
using System.Threading.Tasks;

namespace HardyBits.Ocr.Engine.IO
{
  internal interface IImageFileTypeRecognizer
  {
    Task<ImageFileTypes> GetFileTypeAsync(Stream stream);
  }
}