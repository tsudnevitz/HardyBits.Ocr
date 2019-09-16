using System;
using System.Runtime.CompilerServices;
using System.Text;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Internals;

namespace HardyBits.Ocr.Engine.IO
{
  internal class ImageFileTypeRecognizer : IImageFileTypeRecognizer
  {
    private readonly IPixHelper _pixHelper;

    public ImageFileTypeRecognizer(IPixHelper pixHelper)
    {
      _pixHelper = pixHelper ?? throw new ArgumentNullException(nameof(pixHelper));
    }

    public ImageFileTypes GetFileType(ReadOnlyMemory<byte> imageData)
    {
      return CheckPdfType(imageData) ? ImageFileTypes.Pdf : GetImageType(imageData);
    }

    private ImageFileTypes GetImageType(in ReadOnlyMemory<byte> imageData)
    {
      var format = _pixHelper.GetFileFormat(imageData);

      switch (format)
      {
        case ImageFileFormat.Tiff:
        case ImageFileFormat.TiffPackbits:
        case ImageFileFormat.TiffRle:
        case ImageFileFormat.TiffG3:
        case ImageFileFormat.TiffG4:
        case ImageFileFormat.TiffLzw:
        case ImageFileFormat.TiffZip:
        case ImageFileFormat.Pnm:
        case ImageFileFormat.Ps:
        case ImageFileFormat.Gif:
        case ImageFileFormat.Jp2:
        case ImageFileFormat.Webp:
        case ImageFileFormat.Lpdf:
        case ImageFileFormat.Default:
        case ImageFileFormat.Spix:
        case ImageFileFormat.Bmp:
        case ImageFileFormat.JfifJpeg:
        case ImageFileFormat.Png:
          return ImageFileTypes.Bitmap;
        default:
          return ImageFileTypes.Unrecognized;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe bool CheckPdfType(in ReadOnlyMemory<byte> imageData)
    {
      var bytesLength = imageData.Length < 1024 ? imageData.Length : 1024;
      var headerBytes = imageData.Slice(0, bytesLength).Span;

      fixed (byte* firstByte = &headerBytes.GetPinnableReference())
      {
        var ansiHeader = Encoding.ASCII.GetString(firstByte, bytesLength);
        if (ansiHeader.Contains("%PDF"))
          return true;

        var utfHeader = Encoding.UTF8.GetString(firstByte, bytesLength);
        if (utfHeader.Contains("%PDF"))
          return true;
      }

      return false;
    }
  }
}