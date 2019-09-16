using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;

namespace HardyBits.Ocr.Engine.Jobs
{
  public interface IRecognitionJobFactory
  {
    IRecognitionJob Create(ImageFileTypes type, IEngineConfiguration config, IStoredImageFile imageFile);
  }
}