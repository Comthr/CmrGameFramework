using CmrUnityGameFramework.Runtime;

namespace CmrGame
{
    public class MenuForm:UIFormLogic
    {
        public void OnStartButtonClick()
        {

        }

        public void OnSettingButtonClick()
        {

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
        protected override void OnClose(bool isShutdown, object userData)
        {
            

            base.OnClose(isShutdown, userData);
        }
    }
}
