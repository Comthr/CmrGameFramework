using System;
using UnityEngine;

namespace CmrGame
{
    public partial class GlobalSettings
    {
        [Serializable]
        public class SceneSettings
        {
            [SerializeField] private ScriptableObjectDictionary<EScene,string> sceneMap;
            public bool TryGetValue(EScene key, out string value)
            {
                return sceneMap.TryGetValue(key, out value);
            }
            public string GetValue(EScene key)
            {
                return sceneMap[key];
            }
        }
    }
}
