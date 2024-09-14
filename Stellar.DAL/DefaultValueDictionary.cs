using System.Collections;
using System.Collections.Generic;

namespace Stellar.DAL;

/// <summary>
/// A dictionary that returns the default value when accessing keys that do not exist in the dictionary.
/// </summary>
public class DefaultValueDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly IDictionary<TKey, TValue> _dictionary;

    #region Constructors
    /// <summary>
    /// Initializes a dictionary that returns the default value when accessing keys that do not exist in the dictionary.
    /// </summary>
    public DefaultValueDictionary()
    {
        _dictionary = new Dictionary<TKey, TValue>();
    }

    /// <summary>Initializes with an existing dictionary.</summary>
    /// <param name="dictionary"></param>
    public DefaultValueDictionary(IDictionary<TKey, TValue> dictionary)
    {
        _dictionary = dictionary;
    }

    /// <summary>Initializes using the given equality comparer.</summary>
    /// <param name="comparer"></param>
    public DefaultValueDictionary(IEqualityComparer<TKey> comparer)
    {
        _dictionary = new Dictionary<TKey, TValue>(comparer);
    }
    #endregion Constructors

    #region IDictionary<TKey, TValue> Members
    public void Add(TKey key, TValue value) => _dictionary.Add(key, value);

    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

    public ICollection<TKey> Keys => _dictionary.Keys;

    public bool Remove(TKey key) => _dictionary.Remove(key);

    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

    public ICollection<TValue> Values => _dictionary.Values;

    public TValue this[TKey key] {
        get
        {
            _dictionary.TryGetValue(key, out var value);

            return value;
        }
        set => _dictionary[key] = value;
    }

    #endregion IDictionary<string,TValue> Members

    #region ICollection<KeyValuePair<TKey, TValue>> Members
    public void Add(KeyValuePair<TKey, TValue> item) => _dictionary.Add(item);

    public void Clear() => _dictionary.Clear();

    public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

    public int Count => _dictionary.Count;

    public bool IsReadOnly => _dictionary.IsReadOnly;

    public bool Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item);

    #endregion ICollection<KeyValuePair<TKey,TValue>> Members

    #region IEnumerable<KeyValuePair<TKey, TValue>> Members
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();
    #endregion
}
