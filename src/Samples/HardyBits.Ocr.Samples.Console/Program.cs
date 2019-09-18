using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.Preprocessing;

namespace HardyBits.Ocr.Samples.Console
{
  public static class Program
  {
    public static async Task Main()
    {
      var config1 = new RecognitionConfiguration(@"Samples\sample_photo_1_side.jpg");
      var config2 = new RecognitionConfiguration(@"Samples\sample_photo_2_side.jpg");

      var config = new RecognitionConfigurationBuilder()
        .ConfigureEngine(x => x.TessData, "tessdata")
        .ConfigureEngine(x => x.EngineMode, 3)
        .ConfigureEngine(x => x.Language, "eng")
        .ConfigureFileStream(File.OpenRead(@"Samples\sample_photo_1_side.jpg"), "sample_photo_1_side", ".jpg")
        .ConfigureFilePath(@"Samples\sample_photo_1_side.jpg")
        .ConfigureFileBuffer(File.ReadAllBytes(@"Samples\sample_photo_1_side.jpg"), "sample_photo_1_side", ".jpg")
        .AddPreprocessor("type", p => p
          .AddParameter("param1", 123)
          .AddParameter("param2", 321)
          .AddParameters(new { param1 = 123, param2 = 321}))
        .AddPreprocessor("type2", x => x
          .AddParameter("param3", 321))
        .AddPreprocessor<HeavyLiftPreprocessor>(x => x
          .AddParameter("param3", 321))
        .ConfigureFrom(config1)
        .Build();

      using var engine = new ImageRecognitionEngine();
      var result = await engine.RecognizeAsync(config, runParallel: false);

      //var result = await task1;//Task.WhenAll(task1, task2);
      //var result = results.SelectMany(x => x);

      foreach (var page in result)
        System.Console.WriteLine(page.Text);

      System.Console.WriteLine("Finished!");
      System.Console.ReadLine();
    }
  }

  public class RecognitionConfigurationBuilder : IRecognitionConfigurationBuilder, IPreprocessorConfigurationBuilder
  {
    private RecognitionConfiguration _configuration = new RecognitionConfiguration();

    public IRecognitionConfigurationBuilder ConfigureEngine<T>(Expression<Func<IEngineConfiguration, T>> property, T value)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      if (value == null)
        throw new ArgumentNullException(nameof(value));

      var propertyInfo = GetPropertyInfo(property);
      propertyInfo.SetValue(_configuration, value);
      return this;
    }

    private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
    {
      var type = typeof(TSource);

      if (!(propertyLambda.Body is MemberExpression member))
        throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

      var propInfo = member.Member as PropertyInfo;
      if (propInfo == null)
        throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

      if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType ?? throw new InvalidOperationException()))
        throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

      return propInfo;
    }

    public IRecognitionConfigurationBuilder ConfigureFileStream(Stream stream, string fileName = default, string fileExtension = default)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder ConfigureFilePath(string filePath)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder ConfigureFileBuffer(ReadOnlyMemory<byte> memory, string fileName = default,
      string fileExtension = default)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder AddPreprocessor(string type, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder AddPreprocessor<T>(Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func) where T : class, IPreprocessor
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder AddPreprocessor(Type preprocessorType, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfigurationBuilder ConfigureFrom(IRecognitionConfiguration config)
    {
      throw new NotImplementedException();
    }

    public IRecognitionConfiguration Build()
    {
      throw new NotImplementedException();
    }

    public IPreprocessorConfigurationBuilder AddParameter<T>(string paramName, T paramType)
    {
      throw new NotImplementedException();
    }

    public IPreprocessorConfigurationBuilder AddParameters(object anonymousObject)
    {
      throw new NotImplementedException();
    }
  }

  public interface IRecognitionConfigurationBuilder
  {
    IRecognitionConfigurationBuilder ConfigureEngine<T>(Expression<Func<IEngineConfiguration, T>> property, T value);
    IRecognitionConfigurationBuilder ConfigureFileStream(Stream stream, string fileName = default, string fileExtension = default);
    IRecognitionConfigurationBuilder ConfigureFilePath(string filePath);
    IRecognitionConfigurationBuilder ConfigureFileBuffer(ReadOnlyMemory<byte> memory, string fileName = default, string fileExtension = default);
    IRecognitionConfigurationBuilder AddPreprocessor(string type, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func);
    IRecognitionConfigurationBuilder AddPreprocessor<T>(Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func) where T : class, IPreprocessor;
    IRecognitionConfigurationBuilder AddPreprocessor(Type preprocessorType, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func);
    IRecognitionConfigurationBuilder ConfigureFrom(IRecognitionConfiguration config);
    IRecognitionConfiguration Build();
  }

  public interface IPreprocessorConfigurationBuilder
  {
    IPreprocessorConfigurationBuilder AddParameter<T>(string paramName, T paramType);
    IPreprocessorConfigurationBuilder AddParameters(object anonymousObject);
  }
}
