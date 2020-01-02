using System;
using System.IO;
using System.Linq.Expressions;
using HardyBits.Ocr.Engine.Preprocessing;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IRecognitionConfigurationBuilder
  {
    IRecognitionConfigurationBuilder ConfigureEngine<T>(Expression<Func<IEngineConfiguration, T>> property, T value);
    IRecognitionConfigurationBuilder ConfigureFileStream(Stream stream, string fileName = default, string fileExtension = default);
    IRecognitionConfigurationBuilder ConfigureFilePath(string filePath, string fileName = default, string fileExtension = default);
    IRecognitionConfigurationBuilder ConfigureFileBuffer(ReadOnlyMemory<byte> memory, string fileName = default, string fileExtension = default);
    IRecognitionConfigurationBuilder AddPreprocessor(string type, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null);
    IRecognitionConfigurationBuilder AddPreprocessor<T>(Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null) where T : class, IPreprocessor;
    IRecognitionConfigurationBuilder AddPreprocessor(Type preprocessorType, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null);
    IRecognitionConfigurationBuilder ConfigureFrom(IRecognitionConfiguration config);
    IRecognitionConfiguration Build();
  }
}