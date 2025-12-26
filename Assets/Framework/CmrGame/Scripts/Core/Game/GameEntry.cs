using CmrUnityFramework.Runtime;
using UnityEngine;

namespace CmrGame
{
    public class GameEntry:MonoBehaviour
    {
        private static E_GameSession currentGame;
        public static E_GameSession CurrentGame => currentGame;
        #region Components Ref
        public static BaseComponent Base { get; private set; }
        public static ConfigComponent Config { get; private set; }
        public static DataNodeComponent DataNode { get; private set; }
        public static DataTableComponent DataTable { get; private set; }
        public static DebuggerComponent Debugger { get; private set; }
        public static DownloadComponent Download { get; private set; }
        public static EntityComponent Entity { get; private set; }
        public static EventComponent Event { get; private set; }
        public static FileSystemComponent FileSystem { get; private set; }
        public static FsmComponent Fsm { get; private set; }
        public static LocalizationComponent Localization { get; private set; }
        public static NetworkComponent Network { get; private set; }
        public static ObjectPoolComponent ObjectPool { get; private set; }
        public static ProcedureComponent Procedure { get; private set; }
        public static ReferencePoolComponent ReferencePool { get; private set; }
        public static ResourceComponent Resource { get; private set; }
        public static SceneComponent Scene { get; private set; }
        public static SettingComponent Setting { get; private set; }
        public static SoundComponent Sound { get; private set; }
        public static UIComponent UI { get; private set; }
        public static WebRequestComponent WebRequest { get; private set; }

        #endregion
        private void Awake()
        {

        }
        private void Start()
        {
            InitFrameworkComponents();
        }
        private void InitFrameworkComponents()
        {
            Base = CmrUnityFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
            Config = CmrUnityFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            DataNode = CmrUnityFramework.Runtime.GameEntry.GetComponent<DataNodeComponent>();
            DataTable = CmrUnityFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
            Debugger = CmrUnityFramework.Runtime.GameEntry.GetComponent<DebuggerComponent>();
            Download = CmrUnityFramework.Runtime.GameEntry.GetComponent<DownloadComponent>();
            Entity = CmrUnityFramework.Runtime.GameEntry.GetComponent<EntityComponent>();
            Event = CmrUnityFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            FileSystem = CmrUnityFramework.Runtime.GameEntry.GetComponent<FileSystemComponent>();
            Fsm = CmrUnityFramework.Runtime.GameEntry.GetComponent<FsmComponent>();
            Localization = CmrUnityFramework.Runtime.GameEntry.GetComponent<LocalizationComponent>();
            Network = CmrUnityFramework.Runtime.GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = CmrUnityFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = CmrUnityFramework.Runtime.GameEntry.GetComponent<ProcedureComponent>();
            ReferencePool = CmrUnityFramework.Runtime.GameEntry.GetComponent<ReferencePoolComponent>();
            Resource = CmrUnityFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            Scene = CmrUnityFramework.Runtime.GameEntry.GetComponent<SceneComponent>();
            Setting = CmrUnityFramework.Runtime.GameEntry.GetComponent<SettingComponent>();
            Sound = CmrUnityFramework.Runtime.GameEntry.GetComponent<SoundComponent>();
            UI = CmrUnityFramework.Runtime.GameEntry.GetComponent<UIComponent>();
            WebRequest = CmrUnityFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();

        }
    }
}
