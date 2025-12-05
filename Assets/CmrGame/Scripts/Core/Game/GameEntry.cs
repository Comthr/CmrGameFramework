using CmrUnityGameFramework.Runtime;
using UnityEngine;

namespace CmrGame
{
    public class GameEntry:MonoBehaviour
    {
        private static E_Game currentGame;
        public static E_Game CurrentGame => currentGame;
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
            Base = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<BaseComponent>();
            Config = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<ConfigComponent>();
            DataNode = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<DataNodeComponent>();
            DataTable = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
            Debugger = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<DebuggerComponent>();
            Download = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<DownloadComponent>();
            Entity = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<EntityComponent>();
            Event = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
            FileSystem = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<FileSystemComponent>();
            Fsm = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<FsmComponent>();
            Localization = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<LocalizationComponent>();
            Network = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<ProcedureComponent>();
            ReferencePool = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<ReferencePoolComponent>();
            Resource = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            Scene = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<SceneComponent>();
            Setting = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<SettingComponent>();
            Sound = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<SoundComponent>();
            UI = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
            WebRequest = CmrUnityGameFramework.Runtime.GameEntry.GetComponent<WebRequestComponent>();

        }
    }
}
