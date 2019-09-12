//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using HardyBits.Wrappers.Tesseract.Results;

//namespace HardyBits.Ocr.Engine.Jobs
//{
//  internal class ImageFileHandler
//  {
//    private IFileTypeRecognizer _recognizer;

//    public IJob<IRecognitionResults> Create(IRecognitionConfiguration configuration)
//    {
//      var type = _recognizer.GetFileType(configuration.Image.Data);
//      switch (type)
//      {
//        case FileTypes.PdfText:
//          break;
//        case FileTypes.PdfImage:
//          break;
//        case FileTypes.Tiff:
//          break;
//        case FileTypes.Image:
//          break;
//        default:
//          throw new ArgumentOutOfRangeException();
//      }
//    }
//  }

//  internal interface IFileTypeRecognizer
//  {
//    FileTypes GetFileType(in ReadOnlyMemory<byte> imageData);
//  }

//  internal enum FileTypes
//  {
//    PdfText, PdfImage, Tiff, Image, Unrecognized
//  }

//  public class Document
//  {
//    private IReadOnlyCollection<Page> Pages { get; }
//  }

//  public class Page
//  {

//  }

//  public interface IJob<T> where T : class
//  {
//    Task<T> ResultAsync { get; }
//  }
//}
