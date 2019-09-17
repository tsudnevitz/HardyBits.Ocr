using System;
using System.Collections.Generic;
using System.Linq;

namespace HardyBits.Ocr.Engine.Extensions
{
  public static class TypeExtensions
  {
    public static T GetAttribute<T>(this object instance)
    {
      return instance.GetAttributes<T>().SingleOrDefault();
    }

    public static IEnumerable<T> GetAttributes<T>(this object instance)
    {
      if (instance == null)
        throw new ArgumentNullException(nameof(instance));

      return instance.GetType().GetCustomAttributes(typeof(T), true).Cast<T>();
    }

    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
      var interfaceTypes = givenType.GetInterfaces();

      foreach (var it in interfaceTypes)
        if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
          return true;

      if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        return true;

      var baseType = givenType.BaseType;
      if (baseType == null)
        return false;

      return IsAssignableToGenericType(baseType, genericType);
    }
  }
}