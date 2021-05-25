using System;
using System.Data;
using System.Reflection;

namespace Stellar.DAL
{
    /// <summary>Provides methods for mapping an <see cref="IDataRecord" /> to different types.</summary>
    public static class DataRecordMapper
    {
        /// <summary>Maps an <see cref="IDataRecord" /> to a type of <typeparamref name="T" />.</summary>
        /// <remarks>This method internally uses caching to increase performance.</remarks>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A mapped instance of <typeparamref name="T" />.</returns>
        /// <exception cref="PropertySetValueException">
        /// Thrown when an error occurs when attempting to assign a value to a property.
        /// </exception>
        /// <exception cref="FieldSetValueException">Thrown when an error occurs when attempting to assign a value to a field.</exception>
        public static T Map<T>(this IDataRecord dataRecord)
        {
            var type = typeof(T);

            var fieldCount = dataRecord.FieldCount;

            // Handle mapping to primitives and strings when there is only a single field in the record
            if (fieldCount == 1 && (type.IsPrimitive || type == typeof(string)))
            {
                var convertedValue = TypeConverter.Convert(dataRecord.GetValue(0), type);

                return (T)convertedValue;
            }

            var mappedObject = type.GetDefaultValue() ?? Activator.CreateInstance<T>();

            var didAssignValues = false;

            // ordered dictionary with case-insensitive property or field name as key
            // and PropertyInfo or FieldInfo as value
            var orderedDictionary = TypeCache.GetMetadata(type);

            for (var i = 0; i < fieldCount; i++)
            {
                var dataRecordFieldName = dataRecord.GetName(i).ToLower();

                var memberInfo = orderedDictionary[dataRecordFieldName];

                switch (memberInfo)
                {
                    case null:
                    case PropertyInfo {CanWrite: false}:
                        continue;
                    case PropertyInfo propertyInfo:
                    {
                        if (Attribute.IsDefined(propertyInfo, typeof(DALIgnoreAttribute)))
                        {
                            continue;
                        }

                        var value = dataRecord.GetValue(i);

                        var convertedValue = TypeConverter.Convert(value, propertyInfo.PropertyType);

                        try
                        {
                            propertyInfo.SetValue(mappedObject, convertedValue, null);

                            didAssignValues = true;
                        }
                        catch (Exception exception)
                        {
                            throw new PropertySetValueException(
                                $"An error occurred while attempting to assign the value '{value}' to property '{propertyInfo.Name}' of type '{propertyInfo.PropertyType}' on class type {type}", exception);
                        }

                        break;
                    }
                    case FieldInfo fieldInfo:
                    {
                        if (Attribute.IsDefined(fieldInfo, typeof(DALIgnoreAttribute)))
                        {
                            continue;
                        }
                        
                        var value = dataRecord.GetValue(i);

                        var convertedValue = TypeConverter.Convert(value, fieldInfo.FieldType);

                        try
                        {
                            fieldInfo.SetValue(mappedObject, convertedValue);

                            didAssignValues = true;
                        }
                        catch (Exception exception)
                        {
                            throw new FieldSetValueException(
                                $"An error occurred while attempting to assign the value '{value}' to field '{fieldInfo.Name}' of type '{fieldInfo.FieldType}' on class type {type}", exception);
                        }

                        break;
                    }
                }
            }

            // if no values were assigned, attempt to map the value directly to the type
            return didAssignValues || fieldCount != 1
                ? (T)mappedObject
                : (T)TypeConverter.Convert(dataRecord.GetValue(0), type);
        }

        /// <summary>Maps an <see cref="IDataRecord" /> to a type of dynamic object.</summary>
        /// <param name="dataRecord">The <see cref="IDataRecord" /> to map from.</param>
        /// <returns>A dynamic object.</returns>
        public static dynamic MapDynamic(this IDataRecord dataRecord)
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
        public class FieldSetValueException : Exception
        {
            /// <summary>Instantiates a new <see cref="FieldSetValueException" /> with a specified error message.</summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">
            /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
            /// exception is specified.
            /// </param>
            public FieldSetValueException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }

        /// <summary>Exception thrown when setting a properties value.</summary>
        [Serializable]
        public class PropertySetValueException : Exception
        {
            /// <summary>Instantiates a new <see cref="PropertySetValueException" /> with a specified error message.</summary>
            /// <param name="message">The message that describes the error.</param>
            /// <param name="innerException">
            /// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner
            /// exception is specified.
            /// </param>
            public PropertySetValueException(string message, Exception innerException)
                : base(message, innerException)
            {
            }
        }
    }
}
