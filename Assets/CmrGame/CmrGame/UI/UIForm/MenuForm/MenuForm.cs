using CmrUnityGameFramework.Runtime;
using UnityEngine;

namespace CmrGame
{
    public class MenuForm:UIFormLogic 
    {
        private ProcedureMenu m_ProcedureMenu = null;
        private MenuFormView view;
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            view = GetComponent<MenuFormView>();
            view.m_StartButton.onClick += OnStartButtonClick;
        }
        public void OnStartButtonClick()
        {
            m_ProcedureMenu.StartGame();
        }

        public void OnSettingButtonClick()
        {
            //GFEntry.UI.OpenUIForm(EUIForm.SettingForm);
        }
        public void OnQuitButtonClick()
        {
            GFEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GFEntry.Localization.GetString("AskQuitGame.Title"),
                Message = GFEntry.Localization.GetString("AskQuitGame.Message"),
                OnClickConfirm = delegate (object userData) { GameEntry.Shutdown(ShutdownType.Quit); },
            });
        }
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_ProcedureMenu = (ProcedureMenu)userData;
            if (m_ProcedureMenu == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }
        protected override void OnClose(bool isShutdown, object userData)
        {
            m_ProcedureMenu = null;
            view.m_StartButton.onClick -= OnStartButtonClick;
            base.OnClose(isShutdown, userData);
        }
    }
}
