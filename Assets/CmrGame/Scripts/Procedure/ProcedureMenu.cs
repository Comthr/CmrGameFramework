using CmrGameFramework.Event;
using CmrGameFramework.Procedure;
using CmrUnityGameFramework.Runtime;
using ProcedureOwner = CmrGameFramework.Fsm.IFsm<CmrGameFramework.Procedure.IProcedureManager>;

namespace CmrGame
{
    public class ProcedureMenu : ProcedureBase
    {
        private MenuForm m_MenuForm = null;
        private bool m_IsStart;
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            GameEntry.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
            GameEntry.UI.OpenUIForm(EUIForm.MenuForm, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);

            GameEntry.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);

            if (m_MenuForm != null)
            {
                GameEntry.UI.CloseUIForm(m_MenuForm.UIForm);
                m_MenuForm = null;
            }
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if(m_IsStart)
            {
                this.SetSceneData(procedureOwner, EScene.Game);
                ChangeState<ProcedureChangeScene>(procedureOwner);
            }
        }

        private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
        {
            OpenUIFormSuccessEventArgs ne = (OpenUIFormSuccessEventArgs)e;
            if (ne.UserData != this)
            {
                return;
            }

            m_MenuForm = (MenuForm)ne.UIForm.Logic;
        }
        public void StartGame()
        {
            m_IsStart = true;
            //¹Ø±ÕÆäËû£¿
        }
    }
}
