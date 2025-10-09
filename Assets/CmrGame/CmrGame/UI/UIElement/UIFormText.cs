using UnityEngine;
using UnityEngine.UI;

namespace CmrGame
{
    [RequireComponent(typeof(Text))]
    public class UIFormText:UIFormElement
    {
        private Text m_Text;
        public string text
        {
            get => m_Text.text;
            set
            {
                if (m_Text.text != value)
                {
                    m_Text.text = value;
                }
            }
        }
    }
}
