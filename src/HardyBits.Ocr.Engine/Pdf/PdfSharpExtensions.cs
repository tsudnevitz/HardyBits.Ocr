using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Wrappers.Tesseract.Results;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Pdf.Content;
using PdfSharpCore.Pdf.Content.Objects;

namespace HardyBits.Ocr.Engine.Pdf
{
  public static class PdfSharpExtensions
  {
    public static bool ContainsText(this PdfDocument document)
    {
      return document.Pages.Any<PdfPage>(x => x.ContainsText());
    }

    public static async Task<IEnumerable<IStoredImageFile>> ExtractImagesAsync(this PdfDocument document, IImageFileStorage storage)
    {
      if (storage == null)
        throw new ArgumentNullException(nameof(storage));

      var results = new List<IStoredImageFile>();
      foreach (var page in document.Pages)
      {
        var resources = page.Elements.GetDictionary("/Resources");
        var xObjects = resources?.Elements.GetDictionary("/XObject");
        if (xObjects == null) 
          continue;

        var items = xObjects.Elements.Values;
        foreach (var item in items)
        {
          var reference = item as PdfReference;
          if (!(reference?.Value is PdfDictionary xObject) ||
              xObject.Elements.GetString("/Subtype") != "/Image") 
            continue;

          var path = await TryExportImageAsync(xObject);
          if (path == null)
            continue;

          var file = storage.Wrap(path);
          results.Add(file);
        }
      }
      return results;
    }

    private static async ValueTask<string> TryExportImageAsync(PdfDictionary image)
    {
      var filter = image.Elements.GetName2("/Filter");

      if (filter != "/DCTDecode")
        return null;

      var path = Path.GetTempPath();
      await using var fileStream = new FileStream(path, FileMode.Create);
      await fileStream.WriteAsync(image.Stream.Value.AsMemory());
      return path;
    }

    public static string GetName2(this PdfDictionary.DictionaryElements dict, string key)
    {
      object obj = dict[key];

      if (obj == null)
        return string.Empty;

      if (obj is PdfReference pdfReference)
        obj = pdfReference.Value;

      if (obj is PdfName pdfName)
        return pdfName.Value;

      if (obj is PdfNameObject pdfNameObject)
        return pdfNameObject.Value;

      throw new InvalidCastException("GetName: Object is not a name.");
    }

    public static string GetName3(this PdfDictionary.DictionaryElements dict, string key)
    {
      var value = dict[key];
      switch (value)
      {
        case PdfReference pdfReference:
          switch (pdfReference.Value)
          {
            case PdfNameObject pdfNameObject:
              return pdfNameObject.Value;
            default:
              return string.Empty;
          }
        case PdfName pdfName:
          return pdfName.Value;
        default:
          return string.Empty;
      }
    }

    public static IRecognitionResults ExtractText(this PdfDocument document)
    {
      var results = new RecognitionResults();
      foreach (var page in document.Pages)
      {
        var pageTexts = page.ExtractText();
        var result = new RecognitionResult
        {
          Confidence = 100,
          Text = string.Join(string.Empty, pageTexts)
        };
        results.Add(result);
      }
      return results;
    }  

    public static bool ContainsText(this PdfPage page)
    {       
      var content = ContentReader.ReadContent(page);      
      return content.ExtractText().Any();
    }  

    public static IEnumerable<string> ExtractText(this PdfPage page)
    {       
      var content = ContentReader.ReadContent(page);      
      var text = content.ExtractText();
      return text;
    }   

    public static IEnumerable<string> ExtractText(this CObject cObject)
    {
      switch (cObject)
      {
        case COperator cOperator:
        {
          if (cOperator.OpCode.Name == OpCodeName.Tj.ToString() ||
              cOperator.OpCode.Name == OpCodeName.TJ.ToString())
          {
            foreach (var cOperand in cOperator.Operands)
              foreach (var txt in ExtractText(cOperand))
                yield return txt;   
          }

          break;
        }
        case CSequence cSequence:
        {
          foreach (var element in cSequence)
            foreach (var txt in ExtractText(element))
              yield return txt;
          break;
        }
        case CString cString:
        {
          yield return cString.Value;
          break;
        }
      }
    }
  }
}