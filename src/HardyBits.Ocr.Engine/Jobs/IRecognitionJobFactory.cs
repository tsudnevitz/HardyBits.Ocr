using System.Collections.Generic;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Wrappers.Tesseract.Factories;

namespace HardyBits.Ocr.Engine.Jobs
{
  public interface IRecognitionJobFactory
  {
    IRecognitionJob Create(ImageFileTypes fileType, IStoredImageFile file, IConfiguredTesseractEngineFactory engineFactory, IEnumerable<IPreprocessor> preprocessors);
  }
}