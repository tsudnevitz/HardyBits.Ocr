using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using HardyBits.Ocr.Engine.Preprocessing;

namespace HardyBits.Ocr.Engine.Configuration
{
  public class RecognitionConfigurationBuilder : IRecognitionConfigurationBuilder
  {
    private RecognitionConfiguration _configuration = new RecognitionConfiguration();
    private Dictionary<string, PreprocessorConfigurationBuilder> _preprocessors = new Dictionary<string, PreprocessorConfigurationBuilder>();

    public IRecognitionConfigurationBuilder ConfigureEngine<T>(Expression<Func<IEngineConfiguration, T>> property, T value)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      if (value == null)
        throw new ArgumentNullException(nameof(value));

      var interfacePropertyInfo = GetPropertyInfo(property);
      var instancePropertyInfo = _configuration.GetType().GetProperty(interfacePropertyInfo.Name, interfacePropertyInfo.GetMethod.ReturnType);

      if (instancePropertyInfo == null)
        throw new InvalidOperationException($"Property {instancePropertyInfo.Name} not found.");

      instancePropertyInfo.SetValue(_configuration, value);
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
      if (stream == null)
        throw new ArgumentNullException(nameof(stream));

      if (!stream.CanRead)
        throw new ArgumentException("Invalid stream state. Unable to read.");

      _configuration.DataStream = stream;
      _configuration.Name = fileName;
      _configuration.Extension = fileExtension;

      return this;
    }

    public IRecognitionConfigurationBuilder ConfigureFilePath(string filePath, string fileName = default, string fileExtension = default)
    {
      if (filePath == null)
        throw new ArgumentNullException(nameof(filePath));

      var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
      return ConfigureFileStream(stream, fileName ?? Path.GetFileNameWithoutExtension(filePath), fileExtension ?? Path.GetExtension(filePath));
    }

    public IRecognitionConfigurationBuilder ConfigureFileBuffer(ReadOnlyMemory<byte> memory, string fileName = default, string fileExtension = default)
    {
      // ToDo: change when appropriate override is available
      var stream = new MemoryStream(memory.ToArray(), 0, memory.Length, false);
      return ConfigureFileStream(stream, fileName, fileExtension);
    }

    public IRecognitionConfigurationBuilder AddPreprocessor(string type, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null)
    {
      if (_preprocessors.ContainsKey(type))
        throw new ArgumentException($"Preprocessor of type {type} id already registered.");
      
      var preprocessorBuilder = new PreprocessorConfigurationBuilder();
      func?.Invoke(preprocessorBuilder);
      _preprocessors.Add(type, preprocessorBuilder);
      return this;
    }

    public IRecognitionConfigurationBuilder AddPreprocessor<T>(Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null) where T : class, IPreprocessor
    {
      return AddPreprocessor(typeof(T), func);
    }

    public IRecognitionConfigurationBuilder AddPreprocessor(Type preprocessorType, Func<IPreprocessorConfigurationBuilder, IPreprocessorConfigurationBuilder> func = null)
    {
      if (preprocessorType == null)
        throw new ArgumentNullException(nameof(preprocessorType));
      
      // ToDo testability?
      var manifest = PreprocessorFactory.GetManifest(preprocessorType);
      return AddPreprocessor(manifest.Name, func);
    }

    public IRecognitionConfigurationBuilder ConfigureFrom(IRecognitionConfiguration clonedConfiguration)
    {
      var newConfiguration = new RecognitionConfiguration();
      newConfiguration.TessData = clonedConfiguration.Engine.TessData;
      newConfiguration.Language = clonedConfiguration.Engine.Language;
      newConfiguration.EngineMode = clonedConfiguration.Engine.EngineMode;

      newConfiguration.DataStream = clonedConfiguration.File.DataStream;
      newConfiguration.DisposeStream = clonedConfiguration.File.DisposeStream;
      newConfiguration.Name = clonedConfiguration.File.Name;
      newConfiguration.Extension = clonedConfiguration.File.Extension;

      var newPreprocessors = new Dictionary<string, PreprocessorConfigurationBuilder>();
      foreach (var preprocessor in clonedConfiguration.Preprocessors)
        newPreprocessors.Add(preprocessor.Type, new PreprocessorConfigurationBuilder(preprocessor.Parameters));

      _configuration = newConfiguration;
      _preprocessors = newPreprocessors;
      return this;
    }

    public IRecognitionConfiguration Build()
    {
      var builtConfiguration = _configuration;
      var builtPreprocessors = _preprocessors;
      builtConfiguration.Preprocessors = builtPreprocessors
        .Select(x => x.Value.Build(x.Key)).ToArray();
      return builtConfiguration;
    }
  }
}