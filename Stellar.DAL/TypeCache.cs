using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

namespace Stellar.DAL
{
    /// <summary>A <see cref="Type" /> metadata cache.</summary>
    public static class TypeCache
    {
        /// <summary>
        /// Cache that stores types as the key and the type's PropertyInfo and FieldInfo in a <see cref="OrderedDictionary"/> as the value.
        /// </summary>
        private static readonly Dictionary<Type, OrderedDictionary> Cache = new();

        /// <summary>Gets and caches a type's properties and fields.</summary>
        /// <param name="type">Type.</param>
        /// <returns><see cref="OrderedDictionary"/> of case-insensitive member names with PropertyInfo and FieldInfo as values.</returns>
        public static OrderedDictionary GetMetadata(Type type)
        {
            if (Cache.ContainsKey(type))
            {
                return Cache[type];
            }

            var orderedDictionary = new OrderedDictionary(StringComparer.InvariantCultureIgnoreCase);

            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                if (Attribute.IsDefined(propertyInfo, typeof(DALIgnoreAttribute)))
                {
                    continue;
                }

                orderedDictionary[propertyInfo.Name] = propertyInfo;
            }

            var fields = type.GetFields();

            foreach (var fieldInfo in fields)
            {
                if (Attribute.IsDefined(fieldInfo, typeof(DALIgnoreAttribute)))
                {
                    continue;
                }
                
                orderedDictionary[fieldInfo.Name] = fieldInfo;
            }

            Cache.Add(type, orderedDictionary);

            return orderedDictionary;
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

            var metadata = GetMetadata(type);

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
}
