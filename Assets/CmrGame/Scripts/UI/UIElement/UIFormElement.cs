using UnityEngine;

namespace CmrGame
{
    public class UIFormElement:MonoBehaviour
    {
        [SerializeField] protected string elementName;
        [SerializeField] protected bool isGroup;

        public bool IsGroup => IsGroup;
        public string ElementName => elementName;
    }
}
