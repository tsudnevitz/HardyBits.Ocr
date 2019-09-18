using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

    public async Task<ImageFileTypes> GetFileTypeAsync(Stream stream)
    {
      stream.Position = 0;
      var buffer = new byte[1024];
      var bytesRead = await stream.ReadAsync(buffer, 0, 1024);

      return CheckPdfType(buffer.AsSpan(0, bytesRead)) 
        ? ImageFileTypes.Pdf 
        : GetImageType(buffer.AsSpan(0, 12));
    }

    private ImageFileTypes GetImageType(ReadOnlySpan<byte> imageData)
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
    private static unsafe bool CheckPdfType(ReadOnlySpan<byte> imageData)
    {
      fixed (byte* firstByte = imageData)
      {
        var ansiHeader = Encoding.ASCII.GetString(firstByte, imageData.Length);
        if (ansiHeader.Contains("%PDF"))
          return true;

        var utfHeader = Encoding.UTF8.GetString(firstByte, imageData.Length);
        if (utfHeader.Contains("%PDF"))
          return true;
      }

      return false;
    }
  }
}