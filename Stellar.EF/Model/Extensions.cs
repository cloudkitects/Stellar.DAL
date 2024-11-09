using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Collections.Specialized;

using Stellar.DAL;
using TypeConverter = Stellar.DAL.TypeConverter;
using static Stellar.DAL.Extensions;
using System.Text;

namespace Stellar.EF.Model;

public static class Extensions
{
    #region Entity
    public static bool IdIsNull(this Entity entity)
    {
        return entity is null;
    }

    public static bool IdIsEmpty(this Entity entity)
    {
        return entity.Id.Equals(Guid.Empty);
    }

    public static bool IdIsNullOrEmpty(this Entity entity)
    {
        return IdIsNull(entity) || IdIsEmpty(entity);
    }
    #endregion

    public static void TransferEntityAttribute(Type type, DynamicDictionary dictionary)
    {
        EntityAttribute attribute;

        if (Attribute.IsDefined(type, typeof(EntityAttribute)) && (attribute = (EntityAttribute)Attribute.GetCustomAttribute(type, typeof(EntityAttribute))!) is not null)
        {
            TypeDescriptor.AddAttributes(dictionary, attribute);
        }
    }

    public static DynamicDictionary ExtendWith(this object obj, string key, object value)
    {
        var dictionary = DAL.Extensions.ToDynamicDictionary(obj);

        dictionary.Add(key, value);

        TransferEntityAttribute(obj.GetType(), dictionary);

        return dictionary;
    }

    public static DynamicDictionary ExtendWith(this object obj, IDictionary<string, object> properties)
    {
        var dictionary = DAL.Extensions.ToDynamicDictionary(obj);

        foreach (var (key, value) in properties)
        {
            dictionary.Add(key, value);
        }

        TransferEntityAttribute(obj.GetType(), dictionary);

        return dictionary;
    }

    internal static string BuildEntityName(object obj, string prefix, string suffix)
    {
        var type = obj.GetType();

        var attribute = (EntityAttribute)TypeDescriptor.GetAttributes(obj)[typeof(EntityAttribute)]!;

        if (attribute == null && (!Attribute.IsDefined(type, typeof(EntityAttribute)) ||
           (attribute = (EntityAttribute)Attribute.GetCustomAttribute(type, typeof(EntityAttribute))!) is null))
        {
            return $"{prefix}{type.Name}{suffix}";
        }

        var name = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(attribute.Schema))
        {
            name.Append($"{prefix}{attribute.Schema}{suffix}.");
        }

        name.Append($"{prefix}{(string.IsNullOrWhiteSpace(attribute.Table) ? type.Name : attribute.Table)}{suffix}");

        return name.ToString();
    }

    public static T ToObject<T>(this IDataRecord dataRecord)
    {
        var fieldCount = dataRecord.FieldCount;
        var type = typeof(T);

        if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
        {
            return (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        var obj = type.GetDefaultValue() ?? Activator.CreateInstance<T>();
        var mapped = false;

        var typeMetadata = TypeCache.GetOrAdd(type);

        for (var i = 0; i < fieldCount; i++)
        {
            var fieldName = dataRecord.GetName(i).ToLower();

            var memberInfo = typeMetadata[fieldName];

            switch (memberInfo)
            {
                case null:
                case PropertyInfo { CanWrite: false }:
                    continue;
                case PropertyInfo propertyInfo:
                    {
                        if (Attribute.IsDefined(propertyInfo, typeof(IgnoreAttribute)))
                        {
                            continue;
                        }

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
                        if (Attribute.IsDefined(fieldInfo, typeof(IgnoreAttribute)))
                        {
                            continue;
                        }

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
            : (T)TypeConverter.Convert(dataRecord.GetValue(0), type)!;
    }

    /// <summary>Gets and caches a type's properties and fields.</summary>
    /// <param name="type">Type.</param>
    /// <returns><see cref="OrderedDictionary"/> of case-insensitive member names with PropertyInfo and FieldInfo as values.</returns>
    public static OrderedDictionary GetEntityMetadata(Type type)
    {
        if (TypeCache.TryGet(type, out OrderedDictionary? value))
        {
            return value;
        }

        var orderedDictionary = new OrderedDictionary(StringComparer.InvariantCultureIgnoreCase);

        var properties = type.GetProperties();

        foreach (var propertyInfo in properties)
        {
            if (Attribute.IsDefined(propertyInfo, typeof(IgnoreAttribute)))
            {
                continue;
            }

            orderedDictionary[propertyInfo.Name] = propertyInfo;
        }

        var fields = type.GetFields();

        foreach (var fieldInfo in fields)
        {
            if (Attribute.IsDefined(fieldInfo, typeof(IgnoreAttribute)))
            {
                continue;
            }

            orderedDictionary[fieldInfo.Name] = fieldInfo;
        }

        TypeCache.Cache.Add(type, orderedDictionary);

        return orderedDictionary;
    }

    /// <summary>Maps an <see cref="IDataRecord" /> to a type of <typeparamref name="T" />.</summary>
    /// <remarks>This method internally uses caching to increase performance.</remarks>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
    /// <returns>A mapped instance of <typeparamref name="T" />.</returns>
    /// <exception cref="TypeConversionException">A value cannot be converted.</exception>
    /// <exception cref="PropertySetValueException">A converted value cannot be assigned to a property.</exception>
    /// <exception cref="FieldSetValueException">A converted value cannot be assigned to a field.</exception>
    public static T ToEntity<T>(this IDataRecord dataRecord)
    {
        var fieldCount = dataRecord.FieldCount;
        var type = typeof(T);

        // Handle mapping to primitives and strings when there is only a single field in the record
        if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
        {
            return (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        var obj = type.GetDefaultValue() ?? Activator.CreateInstance<T>();
        var mapped = false;

        // { case-insensitive property/field name, property/field info }
        var orderedDictionary = TypeCache.GetOrAdd(type);

        for (var i = 0; i < fieldCount; i++)
        {
            var fieldName = dataRecord.GetName(i).ToLower();

            // TODO: here's where we'd take advantage of name conversion...
            var memberInfo = orderedDictionary[fieldName];

            switch (memberInfo)
            {
                case null:
                case PropertyInfo { CanWrite: false }:
                    continue;
                case PropertyInfo propertyInfo:
                    {
                        if (Attribute.IsDefined(propertyInfo, typeof(IgnoreAttribute)))
                        {
                            continue;
                        }

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
                        if (Attribute.IsDefined(fieldInfo, typeof(IgnoreAttribute)))
                        {
                            continue;
                        }

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
            : (T)TypeConverter.Convert(dataRecord.GetValue(0), type)!;
    }

}