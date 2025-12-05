using CmrUnityGameFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public class GameForm : UIFormLogic
    {
        private ProcedureGame _procedure = null;
        public void OnStartButtonClick()
        {
            //m_ProcedureMenu.StartGame();
        }

        public void OnSettingButtonClick()
        {
            //GameEntry.UI.OpenUIForm(EUIForm.SettingForm);
        }
        public void OnQuitButtonClick()
        {
            GameEntry.UI.OpenDialog(new DialogParams()
            {
                Mode = 2,
                Title = GameEntry.Localization.GetString("AskQuitGame.Title"),
                Message = GameEntry.Localization.GetString("AskQuitGame.Message"),
                OnClickConfirm = delegate (object userData) { CmrUnityGameFramework.Runtime.GameEntry.Shutdown(ShutdownType.Quit); },
            });
        }
        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            _procedure = (ProcedureGame)userData;
            if (_procedure == null)
            {
                Log.Warning("ProcedureMenu is invalid when open MenuForm.");
                return;
            }
        }
        protected override void OnClose(bool isShutdown, object userData)
        {
            _procedure = null;

            base.OnClose(isShutdown, userData);
        }
    }

}
