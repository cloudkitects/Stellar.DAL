using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace Stellar.DAL;

/// <summary>A <see cref="Type" /> metadata cache.</summary>
public static class TypeCache
{
    /// <summary>
    /// Cache that stores types as the key and the type's PropertyInfo and FieldInfo in a <see cref="OrderedDictionary"/> as the value.
    /// </summary>
    public static readonly Dictionary<Type, OrderedDictionary> Cache = [];

    /// <summary>Gets and caches a type's properties and fields.</summary>
    /// <param name="type">Type.</param>
    /// <returns><see cref="OrderedDictionary"/> of case-insensitive member names with PropertyInfo and FieldInfo as values.</returns>
    public static OrderedDictionary Get(Type type, Func<MemberInfo, bool> ignore = null)
    {
        if (Cache.TryGetValue(type, out OrderedDictionary value))
        {
            return value;
        }

        var typeMetadata = new OrderedDictionary(StringComparer.InvariantCultureIgnoreCase);

        var properties = type.GetProperties();

        foreach (var propertyInfo in properties)
        {
            if (!(ignore?.Invoke(propertyInfo) ?? false))
            {
                typeMetadata[propertyInfo.Name] = propertyInfo;
            }
        }

        var fields = type.GetFields();

        foreach (var fieldInfo in fields)
        {
            if (!(ignore?.Invoke(fieldInfo) ?? false))
            {
                typeMetadata[fieldInfo.Name] = fieldInfo;
            }
        }

        Cache.Add(type, typeMetadata);

        return typeMetadata;
    }

    /// <summary>Gets a dictionary containing the objects property and field names and values.</summary>
    /// <param name="instance">Object to get names and values from.</param>
    /// <returns>Dictionary containing property and field names and values.</returns>
    public static IDictionary<string, object> GetMetadataAndValues(object instance)
    {
        // support dynamic objects backed by a dictionary of string object
        if (instance is IDictionary<string, object> instanceAsDictionary)
        {
            return instanceAsDictionary;
        }

        var type = instance.GetType();

        var metadata = Get(type);

        var dictionary = new Dictionary<string, object>();

        foreach (DictionaryEntry entry in metadata)
        {
            var value = entry.Value switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(instance),
                PropertyInfo propertyInfo => propertyInfo.GetValue(instance, null),
                _ => null
            };

            dictionary.Add(entry.Key.ToString() ?? string.Empty, value);
        }

        return dictionary;
    }
}
