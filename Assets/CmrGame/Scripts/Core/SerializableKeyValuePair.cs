using System;
using UnityEngine;

namespace CmrGame
{
    [Serializable]
    public struct SerializableKeyValuePair<TKey, TValue>
    {
        [SerializeField] TKey key;
        [SerializeField] TValue value;
        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key => key;
        public TValue Value => value;
    }
}
