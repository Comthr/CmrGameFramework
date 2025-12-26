using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CmrGame
{
    // 定义可选择表单控件接口
    public interface IUIFormSelectable
    {
        void OnSelect();
        void OnDeselect();
        void OnConfirm(); 
    }
    public enum InputMode
    {
        KeyboardController,
        Mouse,
        Touch
    }
    public class UIFormButton : UIFormElement, IUIFormSelectable,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler
    {

        private bool isPressed = false;
        private bool isPointerInside = false;
        public event System.Action onClick;
        [Header("Button")]
        [SerializeField] private bool m_Interactable = true;
        [SerializeField] private Image m_Icon;
        [SerializeField] private Text m_Text;
        [SerializeField] private float m_AnimateTime = 0.15f; 


        // 当前状态
        private enum AnimationState
        {
            Normal, Highlighted, Pressed
        }
        private AnimationState m_state = AnimationState.Normal;


        public InputMode GetInputMode()
        {
            return InputMode.Mouse;
        }

        #region IFormSelectable
        public void OnSelect()
        {
            //不用考虑鼠标是否要触发OnSelect，因为OnSelect只有在上级管理的时候分发焦点时才会触发
            SetAnimationState(AnimationState.Highlighted);
        }

        public void OnDeselect()
        {
            SetAnimationState(AnimationState.Normal);
        }

        public void OnConfirm()
        {
            Debug.Log("Confirm");
            onClick?.Invoke();
            //CYJTodo:感觉在这里去设置焦点不太合适
            //如果是鼠标或触控，则需要额外将当前控件设置为焦点
            if (GetInputMode() != InputMode.KeyboardController)
            {
                //CYJTodo:告知Form设置焦点。
            }
        }
        #endregion

        #region Mouse
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("Enter");
            isPointerInside = true;
            if(!isPressed)
                SetAnimationState(AnimationState.Highlighted);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            if(!isPressed)
                SetAnimationState(AnimationState.Normal);
        }


        #endregion

        #region Click
        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pressed");
            isPressed = true;
            //不只是触控要处理，鼠标和控制器也需要有按下的动画。
            SetAnimationState(AnimationState.Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Up");
            if (isPressed && isPointerInside)
            {
                OnConfirm();
            }
            isPressed = false;
            if (GetInputMode() != InputMode.Mouse||!isPointerInside)
                SetAnimationState(AnimationState.Normal);
            else 
            {
                SetAnimationState(AnimationState.Highlighted);
            }

        }
        #endregion
        // 状态切换统一处理动画
        private void SetAnimationState(AnimationState state)
        {
            m_state = state;

            switch (m_state)
            {
                case AnimationState.Normal:
                    // 恢复默认
                    StartCoroutine(AnimateToNormal());
                    break;
                case AnimationState.Highlighted:
                    StartCoroutine(AnimateToHighlighted());
                    break;
                case AnimationState.Pressed:
                    // 按下动画
                    StartCoroutine(AnimateToPressed());
                    break;
            }
        }
        protected virtual IEnumerator AnimateToNormal()
        {
            yield return this.WhenAll(m_Text.SmoothColor(new Color(1, 1, 1, 1), m_AnimateTime), 
                m_Icon.SmoothColor(new Color(1, 1, 1, 1), m_AnimateTime),
                m_Text.SmoothTextSize(32, 0.05f));
        }
        protected virtual IEnumerator AnimateToHighlighted()
        {
            yield return this.WhenAll(m_Text.SmoothColor(new Color(1, 1, 1, 0.3f),m_AnimateTime),
                m_Icon.SmoothColor(new Color(1, 1, 1, 0.3f), m_AnimateTime),
                m_Text.SmoothTextSize(32, 0.05f));
        }
        protected virtual IEnumerator AnimateToPressed()
        {
            yield return this.WhenAll(m_Text.SmoothColor(new Color(1, 1, 1, 0.3f), m_AnimateTime),
                m_Icon.SmoothColor(new Color(1, 1, 1, 0.3f), m_AnimateTime),
                m_Text.SmoothTextSize(26, 0.05f));
        }
    }

}
