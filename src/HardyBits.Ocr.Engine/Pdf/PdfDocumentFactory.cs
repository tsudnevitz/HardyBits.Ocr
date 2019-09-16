using System;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace HardyBits.Ocr.Engine.Pdf
{
  internal class PdfDocumentFactory : IPdfDocumentFactory
  {
    public PdfDocument Open(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof(path));

      return PdfReader.Open(path);
    }
  }
}