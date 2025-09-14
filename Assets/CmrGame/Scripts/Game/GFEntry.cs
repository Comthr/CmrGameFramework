using CmrUnityGameFramework.Runtime;
using UnityEngine;

namespace CmrGame
{
    public class GFEntry:MonoBehaviour
    {
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
        
        [SerializeField]private GlobalSettings _globalSettings;
        public static GlobalSettings GlobalSettings { get; private set; }

        private void Awake()
        {
            GlobalSettings = _globalSettings;
        }
        private void Start()
        {
            //Awake后，所有组件都注册完成了
            InitFrameworkComponents();
        }
        private void InitFrameworkComponents()
        {
            Base = GameEntry.GetComponent<BaseComponent>();
            Config = GameEntry.GetComponent<ConfigComponent>();
            DataNode = GameEntry.GetComponent<DataNodeComponent>();
            DataTable = GameEntry.GetComponent<DataTableComponent>();
            Debugger = GameEntry.GetComponent<DebuggerComponent>();
            Download = GameEntry.GetComponent<DownloadComponent>();
            Entity = GameEntry.GetComponent<EntityComponent>();
            Event = GameEntry.GetComponent<EventComponent>();
            FileSystem = GameEntry.GetComponent<FileSystemComponent>();
            Fsm = GameEntry.GetComponent<FsmComponent>();
            Localization = GameEntry.GetComponent<LocalizationComponent>();
            Network = GameEntry.GetComponent<NetworkComponent>();
            ObjectPool = GameEntry.GetComponent<ObjectPoolComponent>();
            Procedure = GameEntry.GetComponent<ProcedureComponent>();
            ReferencePool = GameEntry.GetComponent<ReferencePoolComponent>();
            Resource = GameEntry.GetComponent<ResourceComponent>();
            Scene = GameEntry.GetComponent<SceneComponent>();
            Setting = GameEntry.GetComponent<SettingComponent>();
            Sound = GameEntry.GetComponent<SoundComponent>();
            UI = GameEntry.GetComponent<UIComponent>();
            WebRequest = GameEntry.GetComponent<WebRequestComponent>();

        }
    }
}
