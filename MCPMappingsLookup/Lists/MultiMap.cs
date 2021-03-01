using System.Collections.Generic;
using System.Threading;

namespace MCPMappingsLookup.Lists
{
    /// <summary>
    ///     An extension to the normal Dictionary, But where the Key returns a HashSet rather than a single value.
    /// <para>
    ///     When adding the same Key, it will fetch the HashSet at the given key, and add the 
    ///     values to that (if it exists that is. Otherwise, it creates the key and adds to the HashSet) 
    /// </para>
    /// <para>
    ///     When removing, you have the <see cref="Remove(TKey, TValue)"/> function to remove a value from the HashSet 
    ///     at a given key, instead of the key itself. Same with the <see cref="ContainsValue(TKey, TValue)"/> function
    /// </para>
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class MultiMap<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            if (!TryGetValue(key, out HashSet<TValue> container))
            {
                container = new HashSet<TValue>();
                base.Add(key, container);
            }
            container.Add(value);
        }


        public bool ContainsValue(TKey key, TValue value)
        {
            if (TryGetValue(key, out HashSet<TValue> values))
            {
                return values.Contains(value);
            }
            return false;
        }


        public bool Remove(TKey key, TValue value)
        {
            if (TryGetValue(key, out HashSet<TValue> values))
            {
                bool removed = values.Remove(value);
                if (values.Count <= 0)
                {
                    return base.Remove(key) && removed;
                }
            }
            return false;
        }
    }
}