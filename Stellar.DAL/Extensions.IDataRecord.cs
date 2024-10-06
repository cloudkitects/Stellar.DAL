using System;
using System.Data;
using System.Reflection;

namespace Stellar.DAL;

/// <summary>
/// <see cref="IDataRecord" /> "to object" and "to dynamic" extensions.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Uses generics, reflection, a type metadata cache and the type converter
    /// to encapsulate runtime parsing of a <see cref="IDataRecord" />.
    /// </summary>
    public static T ToObject<T>(this IDataRecord dataRecord)
    {
        var fieldCount = dataRecord.FieldCount;
        var type = typeof(T);

        // single primitive or string value to speed things a bit
        if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
        {
            return (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        var obj = type.GetDefaultValue() ?? Activator.CreateInstance<T>();
        var mapped = false;

        // get type metadata from the cache (or add it if not there)
        var typeMetadata = TypeCache.Get(type);

        for (var i = 0; i < fieldCount; i++)
        {
            var field = dataRecord.GetName(i).ToLower();

            // TODO: here's where we'd take advantage of name mapping...
            var memberInfo = typeMetadata[field];

            switch (memberInfo)
            {
                case null:
                case PropertyInfo {CanWrite: false}:
                    continue;
                case PropertyInfo propertyInfo:
                {
                    var value = dataRecord.GetValue(i);

                    var convertedValue = TypeConverter.Convert(value, propertyInfo.PropertyType);

                    try
                    {
                        propertyInfo.SetValue(obj, convertedValue, null);

                        mapped = true;
                    }
                    catch (Exception exception)
                    {
                        throw new PropertySetValueException(propertyInfo, convertedValue, exception);
                    }

                    break;
                }
                case FieldInfo fieldInfo:
                {
                    var value = dataRecord.GetValue(i);

                    var convertedValue = TypeConverter.Convert(value, fieldInfo.FieldType);

                    try
                    {
                        fieldInfo.SetValue(obj, convertedValue);

                        mapped = true;
                    }
                    catch (Exception exception)
                    {
                        throw new FieldSetValueException(fieldInfo, value, exception);
                    }

                    break;
                }
            }
        }

        return mapped || fieldCount != 1
            ? (T)obj
            : (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
    }

    /// <summary>
    /// Uses generics, reflection, a type metadata cache, the type converter
    /// and an ignore delegate to encapsulate runtime parsing of a <see cref="IDataRecord" />.
    /// </summary>
    public static T ToObject<T>(this IDataRecord dataRecord, Func<MemberInfo, bool> ignore)
    {
        var fieldCount = dataRecord.FieldCount;
        var type = typeof(T);

        // single primitive or string value to speed things a bit
        if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
        {
            return (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        var obj = type.GetDefaultValue() ?? Activator.CreateInstance<T>();
        var mapped = false;

        // get type metadata from the cache (or add it if not there)
        var typeMetadata = TypeCache.Get(type, ignore);

        for (var i = 0; i < fieldCount; i++)
        {
            var field = dataRecord.GetName(i).ToLower();

            // TODO: here's where we'd take advantage of name mapping...
            var memberInfo = typeMetadata[field];

            switch (memberInfo)
            {
                case null:
                case PropertyInfo {CanWrite: false}:
                    continue;
                case PropertyInfo propertyInfo:
                {
                    var value = dataRecord.GetValue(i);

                    var convertedValue = TypeConverter.Convert(value, propertyInfo.PropertyType);

                    try
                    {
                        propertyInfo.SetValue(obj, convertedValue, null);

                        mapped = true;
                    }
                    catch (Exception exception)
                    {
                        throw new PropertySetValueException(propertyInfo, convertedValue, exception);
                    }

                    break;
                }
                case FieldInfo fieldInfo:
                {
                    var value = dataRecord.GetValue(i);

                    var convertedValue = TypeConverter.Convert(value, fieldInfo.FieldType);

                    try
                    {
                        fieldInfo.SetValue(obj, convertedValue);

                        mapped = true;
                    }
                    catch (Exception exception)
                    {
                        throw new FieldSetValueException(fieldInfo, value, exception);
                    }

                    break;
                }
            }
        }

        return mapped || fieldCount != 1
            ? (T)obj
            : (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
    }

    /// <summary>
    /// Maps <see cref="IDataRecord" /> to an internal dynamic dictionary with no
    /// other conversion than DbNull to null.
    /// </summary>
    public static dynamic ToDynamic(this IDataRecord dataRecord)
    {
        dynamic obj = new DynamicDictionary();

        for (var i = 0; i < dataRecord.FieldCount; i++)
        {
            var value = dataRecord.GetValue(i);

            obj[dataRecord.GetName(i)] = value == DBNull.Value ? null : value;
        }

        return obj;
    }

    /// <summary>Exception thrown when setting a fields value.</summary>
    [Serializable]
    public class FieldSetValueException(FieldInfo fieldInfo, object value, Exception innerException) : Exception(GetMessage(fieldInfo, value), innerException)
    {
        private static string GetMessage(FieldInfo fieldInfo, object value)
        {
            return $"Error assigning {value} value to {fieldInfo.ReflectedType}.{fieldInfo.Name}({fieldInfo.FieldType}) field.";
        }
    }

    /// <summary>Exception thrown when setting a properties value.</summary>
    [Serializable]
    public class PropertySetValueException(PropertyInfo propertyInfo, object value, Exception innerException) : Exception(GetMessage(propertyInfo, value), innerException)
    {
        private static string GetMessage(PropertyInfo propertyInfo, object value)
        {
            return $"Error assigning {value} value to {propertyInfo.ReflectedType}.{propertyInfo.Name}({propertyInfo.PropertyType}) property.";
        }
    }
}
