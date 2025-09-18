using UnityEngine;
using UnityEngine.EventSystems;

namespace CmrGame
{
    // 定义可选择表单控件接口
    public interface IUIFormSelectable
    {
        void OnSelect();    // 选中时调用
        void OnDeselect();  // 取消选中时调用
        void OnConfirm();   // 确认（激活）时调用
    }
    public enum InputMode
    {
        KeyboardController,
        Mouse,
        Touch
    }
    public class UIFormButton : MonoBehaviour, IUIFormSelectable,
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
        IPointerDownHandler, IPointerUpHandler
    {
        public string Label;
        public System.Action OnClicked;

        // 当前状态
        private enum AnimationState
        {
            Normal, Hover, Highlighted, Pressed
        }
        private AnimationState m_state = AnimationState.Normal;

        // 由外部 InputManager 决定，这里暂时写这里。
        public InputMode CurrentInputMode { get; set; } = InputMode.KeyboardController;

        #region IFormSelectable
        public void OnSelect()
        {
            if (CurrentInputMode != InputMode.Mouse && CurrentInputMode != InputMode.Touch)
            {
                SetAnimationState(AnimationState.Highlighted);
            }
        }

        public void OnDeselect()
        {
            if (CurrentInputMode != InputMode.Mouse && CurrentInputMode != InputMode.Touch)
            {
                SetAnimationState(AnimationState.Normal);
            }
        }

        public void OnConfirm()
        {
            OnClicked?.Invoke();
        }
        #endregion

        #region Mouse
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CurrentInputMode == InputMode.Mouse)
            {
                SetAnimationState(AnimationState.Hover);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentInputMode == InputMode.Mouse)
            {
                // 鼠标退出不触发 Navigator Deselect
                if (m_state == AnimationState.Hover)
                    SetAnimationState(AnimationState.Normal);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (CurrentInputMode == InputMode.Mouse)
            {
                OnConfirm();
                // 可以选择是否触发 Navigator 的 OnSelect，这里不自动触发

            }
        }
        #endregion

        #region Touch
        public void OnPointerDown(PointerEventData eventData)
        {
            if (CurrentInputMode == InputMode.Touch)
            {
                SetAnimationState(AnimationState.Pressed);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (CurrentInputMode == InputMode.Touch)
            {
                SetAnimationState(AnimationState.Normal);
                OnConfirm();
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
                    break;
                case AnimationState.Hover:
                case AnimationState.Highlighted:
                    // 高亮动画
                    break;
                case AnimationState.Pressed:
                    // 按下动画
                    break;
            }
            Debug.Log($"{Label} -> {m_state}");
        }
    }

}
