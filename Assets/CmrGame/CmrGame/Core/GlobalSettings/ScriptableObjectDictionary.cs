using System;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    [Serializable]
    public class ScriptableObjectDictionary<TKey,TValue>
    {
        private Dictionary<TKey, TValue> dict;
        [SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> entries = new List<SerializableKeyValuePair<TKey, TValue>>();
        public int Count => entries.Count;
        public bool TryGetValue(TKey key, out TValue value)
        {
            EnsureDictionary();
            return dict.TryGetValue(key, out value);
        }
        public TValue this[TKey key]
        {
            get 
            {
                EnsureDictionary();
                if(!dict.ContainsKey(key))
                    throw new KeyNotFoundException();
                return dict[key];
            }
        }
        private void EnsureDictionary()
        {
            if (dict != null) return;
            dict = new Dictionary<TKey, TValue>();
            foreach (var entry in entries)
            {
                if (!dict.ContainsKey(entry.Key))
                    dict.Add(entry.Key, entry.Value);
            }
        }
    }
}
