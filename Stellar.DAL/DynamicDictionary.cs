using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Stellar.DAL;

/// <summary>
/// A System.Dynamic object override whose members are implemented using a Stellar.DAL default value dictionary.
/// </summary>
/// <example>
/// Whereas: 
///   dynamic person = new { Age = 42 };
///   Assert.Throws<RuntimeBinderException>(var name = person.Name); // person does not contain a definition for 'Name'
//
///   var person = new DynamicInstance();
///   Assert.Null(person.FirstName);
///   person.Alterego = "Superman";
///   Assert.Equals("Superman", person.ALTEREGO);
/// </example>
public class DynamicInstance(IDictionary<string, object> dictionary = null) : DynamicObject
{
    protected readonly IDictionary<string, object> Dictionary = dictionary ?? new DefaultValueDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

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
        if (!Dictionary.TryGetValue(binder.Name, out object value) || value is not Delegate)
        {
            return base.TryInvokeMember(binder, args, out result);
        }

        var delegateValue = value as Delegate;

        result = delegateValue?.DynamicInvoke(args);

        return true;
    }
}

/// <summary>
/// A dictionary implementation based on Stellar.DAL DynamicInstance.
/// </summary>
public class DynamicDictionary(IDictionary<string, object> dictionary = null) : DynamicInstance(dictionary), IDictionary<string, object>
{
    #region IDictionary<string, object>
    public void Add(string key, object value)
    {
        Dictionary.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return Dictionary.ContainsKey(key);
    }

    public ICollection<string> Keys => Dictionary.Keys;

    public bool Remove(string key)
    {
        return Dictionary.Remove(key);
    }

    public bool TryGetValue(string key, out object value)
    {
        return Dictionary.TryGetValue(key, out value);
    }

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

    #region ICollection<KeyValuePair<string, object>>
    public void Add(KeyValuePair<string, object> item)
    {
        Dictionary.Add(item);
    }

    public void Clear()
    {
        Dictionary.Clear();
    }

    public bool Contains(KeyValuePair<string, object> item)
    {
        return Dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        Dictionary.CopyTo(array, arrayIndex);
    }

    public int Count => Dictionary.Count;

    public bool IsReadOnly => Dictionary.IsReadOnly;

    public bool Remove(KeyValuePair<string, object> item)
    {
        return Dictionary.Remove(item);
    }
    #endregion

    #region IEnumerable<KeyValuePair<string,object>>
    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }
    #endregion

    #region IEnumerable
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }
    #endregion
}
