using System.Collections;

namespace Stellar.DAL;

/// <summary>
/// A dictionary that returns the default value when accessing keys that do not exist in the dictionary.
/// </summary>
public class DefaultValueDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue> dictionary;

    #region Constructors
    /// <summary>
    /// Initializes a dictionary that returns the default value when accessing keys that do not exist in the dictionary.
    /// </summary>
    public DefaultValueDictionary()
    {
        dictionary = new Dictionary<TKey, TValue>();
    }

    /// <summary>Initializes with an existing dictionary.</summary>
    /// <param name="dictionary"></param>
    public DefaultValueDictionary(IDictionary<TKey, TValue> dictionary)
    {
        this.dictionary = dictionary;
    }

    /// <summary>Initializes using the given equality comparer.</summary>
    /// <param name="comparer"></param>
    public DefaultValueDictionary(IEqualityComparer<TKey> comparer)
    {
        dictionary = new Dictionary<TKey, TValue>(comparer);
    }
    #endregion Constructors

    #region IDictionary<TKey, TValue> Members
    public void Add(TKey key, TValue value)
    {
        dictionary.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    public ICollection<TKey> Keys => dictionary.Keys;

    public bool Remove(TKey key)
    {
        return dictionary.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value!);
    }

    public ICollection<TValue> Values => dictionary.Values;

    public TValue this[TKey key] {
        get
        {
            dictionary.TryGetValue(key, out var value);

            return value!;
        }
        set => dictionary[key] = value;
    }

    #endregion IDictionary<string,TValue> Members

    #region ICollection<KeyValuePair<TKey, TValue>> Members
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        dictionary.Add(item);
    }

    public void Clear()
    {
        dictionary.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        dictionary.CopyTo(array, arrayIndex);
    }

    public int Count => dictionary.Count;

    public bool IsReadOnly => dictionary.IsReadOnly;

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return dictionary.Remove(item);
    }

    #endregion ICollection<KeyValuePair<TKey,TValue>> Members

    #region IEnumerable<KeyValuePair<TKey, TValue>> Members
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }
    #endregion
}
