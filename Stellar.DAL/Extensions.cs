using System;
using System.Collections.Concurrent;

namespace Stellar.DAL;

public static partial class Extensions
{
    #region object
    /// <summary>Indicates if the object is an anonymous type.</summary>
    /// <param name="item">Object instance.</param>
    /// <returns>Returns true if the object is an anonymous type.</returns>
    public static bool IsAnonymousType(this object item)
    {
        return item != null && item.GetType().Namespace == null;
    }

    /// <summary>Converts the given object to a type of <typeparamref name="T" />.</summary>
    /// <typeparam name="T">Type to convert to.</typeparam>
    /// <param name="item">Object to convert.</param>
    /// <returns>Instance of type <typeparamref name="T" />.</returns>
    /// <exception cref="TypeConversionException">
    /// Thrown when an error occurs attempting to convert a value to an enum or a type.
    /// </exception>
    public static T ConvertTo<T>(this object item)
    {
        return (T)TypeConverter.Convert(item, typeof(T));
    }

    /// <summary>Converts the given object to an <see cref="int" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Int representation of the object.</returns>
    public static int ToInt(this object obj)
    {
        return ConvertTo<int>(obj);
    }

    /// <summary>Converts the given object to an <see cref="int" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Int representation of the object.</returns>
    public static int? ToNullableInt(this object obj)
    {
        return ConvertTo<int?>(obj);
    }

    /// <summary>Converts the given object to an <see cref="long" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Long representation of the object.</returns>
    public static long ToLong(this object obj)
    {
        return ConvertTo<long>(obj);
    }

    /// <summary>Converts the given object to an <see cref="long" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Long representation of the object.</returns>
    public static long? ToNullableLong(this object obj)
    {
        return ConvertTo<long?>(obj);
    }

    /// <summary>Converts the given object to an <see cref="double" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Double representation of the object.</returns>
    public static double ToDouble(this object obj)
    {
        return ConvertTo<double>(obj);
    }

    /// <summary>Converts the given object to an <see cref="double" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Double representation of the object.</returns>
    public static double? ToNullableDouble(this object obj)
    {
        return ConvertTo<double?>(obj);
    }

    /// <summary>Converts the given object to an <see cref="decimal" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Decimal representation of the object.</returns>
    public static decimal ToDecimal(this object obj)
    {
        return ConvertTo<decimal>(obj);
    }

    /// <summary>Converts the given object to an <see cref="decimal" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Decimal representation of the object.</returns>
    public static decimal? ToNullableDecimal(this object obj)
    {
        return ConvertTo<decimal?>(obj);
    }

    /// <summary>Converts the given object to an <see cref="DateTime" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>DateTime representation of the object.</returns>
    public static DateTime ToDateTime(this object obj)
    {
        return ConvertTo<DateTime>(obj);
    }

    /// <summary>Converts the given object to an <see cref="DateTime" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>DateTime representation of the object.</returns>
    public static DateTime? ToNullableDateTime(this object obj)
    {
        return ConvertTo<DateTime?>(obj);
    }

    /// <summary>Converts the given object to an <see cref="bool" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Bool representation of the object.</returns>
    public static bool ToBool(this object obj)
    {
        return ConvertTo<bool>(obj);
    }

    /// <summary>Converts the given object to an <see cref="bool" />.</summary>
    /// <param name="obj">Object to convert.</param>
    /// <returns>Bool representation of the object.</returns>
    public static bool? ToNullableBool(this object obj)
    {
        return ConvertTo<bool?>(obj);
    }

    /// <summary>
    /// Convert an object into a <see cref="DynamicDictionary"/>, leveraging the type cache.
    /// </summary>
    /// <param name="obj">The object to convert.</param>
    /// <returns>The object converted to a dynamic dictionary.</returns>
    public static DynamicDictionary ToDynamicDictionary(this object obj)
    {
        return new(TypeCache.GetMetadataAndValues(obj));
    }
    #endregion

    #region Type
    /// <summary>
    /// Cache of value types' default values to reduce Activator.CreateInstance calls to a minimum.
    /// </summary>
    public static readonly ConcurrentDictionary<Type, object> DefaultValueCache = new();

    /// <summary>Gets the default value for the given type.</summary>
    /// <param name="type">Type to get the default value for.</param>
    /// <returns>Default value of the given type.</returns>
    public static object GetDefaultValue(this Type type)
    {
        return type.IsValueType
            ? DefaultValueCache.GetOrAdd(type, Activator.CreateInstance)
            : null;
    }
    #endregion
}
