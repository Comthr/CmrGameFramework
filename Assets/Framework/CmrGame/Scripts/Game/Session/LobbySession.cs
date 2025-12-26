using CmrUnityFramework.Runtime;

namespace CmrGame
{
    public class LobbySession : GameSession
    {
        // 大厅特有的 UI 管理器 (假设你有 UIAgent)
        //public UIAgent UI { get; private set; }

        public LobbySession() : base(E_GameSession.Lobby)
        {
            // 构造函数里注册 Lobby 特有的 Agent
            // 注意：Resources 和 Phase 已经在基类里注册好了！
           // UI = RegisterAgent<UIAgent>();
        }

        public override void Startup()
        {
            // 业务逻辑直接开始
            Log.Info("大厅启动");

            // 直接使用基类提供的 Phase Agent 启动状态机
            // 进入大厅的加载阶段
            //Phase.StartPhase<PhaseLobbyLoading>();
        }
    }
}