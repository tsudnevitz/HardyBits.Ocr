using PdfSharp.Pdf;

namespace HardyBits.Ocr.Engine.Pdf
{
  internal interface IPdfDocumentFactory
  {
    PdfDocument Open(string path);
  }
}