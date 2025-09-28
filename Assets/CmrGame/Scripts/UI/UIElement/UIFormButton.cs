using UnityEngine;
using UnityEngine.EventSystems;

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
        public System.Action OnClicked;
        [SerializeField] private bool m_Interactable = true;

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
            OnClicked?.Invoke();
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
            SetAnimationState(AnimationState.Highlighted);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetAnimationState(AnimationState.Normal);
        }


        #endregion

        #region Click
        public void OnPointerDown(PointerEventData eventData)
        {
            //不只是触控要处理，鼠标和控制器也需要有按下的动画。
            SetAnimationState(AnimationState.Pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputMode mode = GetInputMode();
            if(mode != InputMode.Mouse) 
                SetAnimationState(AnimationState.Normal);
            OnConfirm();
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
                    break;
                case AnimationState.Highlighted:
                    // 高亮动画
                    break;
                case AnimationState.Pressed:
                    // 按下动画
                    break;
            }
        }
    }

}
