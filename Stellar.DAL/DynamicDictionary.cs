/*    
DynamicDictionary implementation obtained from: https://github.com/randyburden/DynamicDictionary    

The MIT License (MIT)

Copyright (c) 2014 Randy Burden (http://randyburden.com) All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Stellar.DAL
{
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
    public class DynamicInstance : DynamicObject
    {
        protected readonly IDictionary<string, object> Dictionary;

        #region constructor
        public DynamicInstance(IDictionary<string, object> dictionary = null)
        {
            Dictionary = dictionary ?? new DefaultValueDictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        }
        #endregion

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
    public class DynamicDictionary : DynamicInstance, IDictionary<string, object>
    {
        #region constructor
        public DynamicDictionary(IDictionary<string, object> dictionary = null) : base(dictionary)
        {
        }
        #endregion

        #region IDictionary<string, object> Members
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

        #region ICollection<KeyValuePair<string, object>> Members
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

        #region IEnumerable<KeyValuePair<string,object>> Members
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Dictionary.GetEnumerator();
        }
        #endregion
    }
}
