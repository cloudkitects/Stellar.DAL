using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;

using Stellar.Common;
using Stellar.DAL;

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
        var dictionary = obj.ToDynamicDictionary();

        dictionary.Add(key, value);

        TransferEntityAttribute(obj.GetType(), dictionary);

        return dictionary;
    }

    public static DynamicDictionary ExtendWith(this object obj, IDictionary<string, object> properties)
    {
        var dictionary = obj.ToDynamicDictionary();

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
            return ValueConverter.Parse<T>($"{dataRecord.GetValue(0)}");
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

                        var convertedValue = ValueConverter.Parse(value, propertyInfo.PropertyType);

                        try
                        {
                            propertyInfo.SetValue(obj, convertedValue, null);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new DAL.Extensions.PropertySetValueException(propertyInfo, convertedValue!, exception);
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

                        var convertedValue = ValueConverter.Parse(value, fieldInfo.FieldType);

                        try
                        {
                            fieldInfo.SetValue(obj, convertedValue);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new DAL.Extensions.FieldSetValueException(fieldInfo, value, exception);
                        }

                        break;
                    }
            }
        }

        return mapped || fieldCount != 1
            ? (T)obj!
            : ValueConverter.Parse<T>($"{dataRecord.GetValue(0)}");
    }

    /// <summary>Gets and caches a type's properties and fields.</summary>
    /// <param name="type">Type.</param>
    /// <returns><see cref="OrderedDictionary"/> of case-insensitive member names with PropertyInfo and FieldInfo as values.</returns>
    public static OrderedDictionary? GetEntityMetadata(Type type)
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

    /// <summary>
    /// TODO: a data record field does not hold culture info, so we need to keep it somewhere else
    /// </summary>
    public static T ToEntity<T>(this IDataRecord dataRecord)
    {
        var fieldCount = dataRecord.FieldCount;
        var type = typeof(T);

        // Handle mapping to primitives and strings when there is only a single field in the record
        if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
        {
            return ValueConverter.Parse<T>($"{dataRecord.GetValue(0)}");
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

                        // TODO: 
                        var convertedValue = ValueConverter.Parse(value, propertyInfo.PropertyType);

                        try
                        {
                            propertyInfo.SetValue(obj, convertedValue, null);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new DAL.Extensions.PropertySetValueException(propertyInfo, convertedValue!, exception);
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

                        var convertedValue = ValueConverter.Parse(value, fieldInfo.FieldType);

                        try
                        {
                            fieldInfo.SetValue(obj, convertedValue);

                            mapped = true;
                        }
                        catch (Exception exception)
                        {
                            throw new DAL.Extensions.FieldSetValueException(fieldInfo, value, exception);
                        }

                        break;
                    }
            }
        }

        return mapped || fieldCount != 1
            ? (T)obj!
            : ValueConverter.Parse<T>($"{dataRecord.GetValue(0)}");
    }

}