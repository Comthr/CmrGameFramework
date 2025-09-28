using CmrUnityGameFramework.Runtime;
using UnityEngine;

namespace CmrGame
{
    public class UIFormNavigator
    {
        private IUIFormSelectable m_currentSelectedUI;
        private bool m_isFormActive;
        public void SetSelectUI(IUIFormSelectable selectedUI)
        {
            if(!m_isFormActive)
            {
                Log.Debug("UIForm未激活你是怎么进来的？");
            }
            if(m_currentSelectedUI == null)
            {
                Log.Debug("不应该有失焦的状态才对呀");
            }
            m_currentSelectedUI.OnDeselect();
            
            m_currentSelectedUI = selectedUI;
            m_currentSelectedUI.OnSelect();
        }
       
    }
}
