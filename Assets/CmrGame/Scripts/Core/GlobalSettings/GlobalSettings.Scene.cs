using System;
using UnityEngine;

namespace CmrGame
{
    public partial class GlobalSettings
    {
        [Serializable]
        public class SceneSettings
        {
            [SerializeField] private ScriptableObjectDictionary<E_Scene,string> sceneMap;
            public bool TryGetValue(E_Scene key, out string value)
            {
                return sceneMap.TryGetValue(key, out value);
            }
            public string GetValue(E_Scene key)
            {
                return sceneMap[key];
            }
        }
    }
}
