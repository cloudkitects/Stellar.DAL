using System.Data;
using System.Reflection;

namespace Stellar.DAL;

/// <summary>
/// <see cref="IDataRecord" /> "to object" and "to dynamic" extensions.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Parse a <see cref="IDataRecord" /> using generics, reflection, the type cache,
    /// the type converter and an ignore delegate.
    /// </summary>
    public static T? ToObject<T>(this IDataRecord dataRecord, Func<MemberInfo, bool>? ignore = null)
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

        var typeMetadata = TypeCache.GetOrAdd(type, ignore);

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
            ? (T)obj!
            : (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
    }

    /// <summary>
    /// Maps <see cref="IDataRecord" /> to an internal dynamic dictionary with all conversions intrinsic
    /// to the .NET data provider and a provision for DbNull.
    /// </summary>
    public static DynamicDictionary ToDynamic(this IDataRecord dataRecord)
    {
        var obj = new DynamicDictionary();

        for (var i = 0; i < dataRecord.FieldCount; i++)
        {
            var value = dataRecord.GetValue(i);

            obj[dataRecord.GetName(i)] = Convert.IsDBNull(value) ? null! : value;
        }

        return obj;
    }

    [Serializable]
    public class FieldSetValueException(FieldInfo fieldInfo, object value, Exception innerException) : Exception(GetMessage(fieldInfo, value), innerException)
    {
        private static string GetMessage(FieldInfo fieldInfo, object value)
        {
            return $"Error assigning {value} value to {fieldInfo.ReflectedType}.{fieldInfo.Name}({fieldInfo.FieldType}) field.";
        }
    }

    [Serializable]
    public class PropertySetValueException(PropertyInfo propertyInfo, object value, Exception innerException) : Exception(GetMessage(propertyInfo, value), innerException)
    {
        private static string GetMessage(PropertyInfo propertyInfo, object value)
        {
            return $"Error assigning {value} value to {propertyInfo.ReflectedType}.{propertyInfo.Name}({propertyInfo.PropertyType}) property.";
        }
    }
}
