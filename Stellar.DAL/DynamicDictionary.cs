using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Stellar.DAL;

/// <summary>
/// A dynamic dictionary allowing case-insensitive access and returns null when accessing non-existent properties.
/// </summary>
/// <example>
/// // Non-existent properties will return null
/// dynamic obj = new DynamicDictionary();
/// var firstName = obj.FirstName;
/// Assert.Null(firstName);
///
/// // Allows case-insensitive property access
/// dynamic obj = new DynamicDictionary();
/// obj.SuperHero = "Superman";
/// Assert.That(obj.SUPERHERO == "Superman");
/// Assert.That(obj.superhero == obj.sUpErHeRo);
/// </example>
public class DynamicInstance(IDictionary<string, object> dictionary = null) : DynamicObject
{
    protected readonly IDictionary<string, object> Dictionary = dictionary ?? new DefaultValueDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

    #region DynamicObject Overrides
    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        result = Dictionary[binder.Name];

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
        if (Dictionary.ContainsKey(binder.Name))
        {
            Dictionary[binder.Name] = value;
        }
        else
        {
            Dictionary.Add(binder.Name, value);
        }

        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
    {
        if (!Dictionary.ContainsKey(binder.Name) || !(Dictionary[binder.Name] is Delegate))
        {
            return base.TryInvokeMember(binder, args, out result);
        }

        var delegateValue = Dictionary[binder.Name] as Delegate;

        result = delegateValue?.DynamicInvoke(args);

        return true;
    }
    #endregion
}

/// <summary>
/// A dynamic dictionary allowing case-insensitive access and returns null when accessing non-existent properties.
/// </summary>
/// <example>
/// // Non-existent properties will return null
/// dynamic obj = new DynamicDictionary();
/// var firstName = obj.FirstName;
/// Assert.Null(firstName);
///
/// // Allows case-insensitive property access
/// dynamic obj = new DynamicDictionary();
/// obj.SuperHero = "Superman";
/// Assert.That(obj.SUPERHERO == "Superman");
/// Assert.That(obj.superhero == obj.sUpErHeRo);
/// </example>
public class DynamicDictionary(IDictionary<string, object> dictionary = null) : DynamicInstance(dictionary), IDictionary<string, object>
{
    #region IDictionary<string, object> Members
    public void Add(string key, object value) => Dictionary.Add(key, value);

    public bool ContainsKey(string key) => Dictionary.ContainsKey(key);

    public ICollection<string> Keys => Dictionary.Keys;

    public bool Remove(string key) => Dictionary.Remove(key);

    public bool TryGetValue(string key, out object value) => Dictionary.TryGetValue(key, out value);

    public ICollection<object> Values => Dictionary.Values;

    public object this[string key]
    {
        get
        {
            Dictionary.TryGetValue(key, out var value);

            return value;
        }
        set => Dictionary[key] = value;
    }
    #endregion

    #region ICollection<KeyValuePair<string, object>> Members
    public void Add(KeyValuePair<string, object> item) => Dictionary.Add(item);

    public void Clear() => Dictionary.Clear();

    public bool Contains(KeyValuePair<string, object> item) => Dictionary.Contains(item);

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => Dictionary.CopyTo(array, arrayIndex);

    public int Count => Dictionary.Count;

    public bool IsReadOnly => Dictionary.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item) => Dictionary.Remove(item);
    #endregion

    #region IEnumerable<KeyValuePair<string,object>> Members
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => Dictionary.GetEnumerator();
    #endregion

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator() => Dictionary.GetEnumerator();
    #endregion
}
