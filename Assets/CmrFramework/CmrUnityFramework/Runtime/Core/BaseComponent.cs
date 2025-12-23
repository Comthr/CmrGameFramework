using CmrGameFramework;
using CmrGameFramework.Localization;
using CmrGameFramework.Resource;
using System;
using UnityEngine;
namespace CmrUnityFramework.Runtime
{
    [DisallowMultipleComponent]
    [AddComponentMenu("CmrFramework/Base")]
    public class BaseComponent:GameFrameworkComponent
    {
        private const int DefaultDpi = 96;  // default windows dpi

        private float m_GameSpeedBeforePause = 1f;

        [SerializeField]
        private bool m_EditorResourceMode = true;

        [SerializeField]
        private Language m_EditorLanguage = Language.Unspecified;

        [SerializeField]
        private string m_TextHelperTypeName = "CmrUnityFramework.Runtime.DefaultTextHelper";

        [SerializeField]
        private string m_VersionHelperTypeName = "CmrUnityFramework.Runtime.DefaultVersionHelper";

        [SerializeField]
        private string m_LogHelperTypeName = "CmrUnityFramework.Runtime.DefaultLogHelper";

        [SerializeField]
        private string m_CompressionHelperTypeName = "CmrUnityFramework.RuntimeDefaultCompressionHelper";

        [SerializeField]
        private string m_JsonHelperTypeName = "CmrUnityFramework.Runtime.DefaultJsonHelper";

        [SerializeField]
        private int m_FrameRate = 30;

        [SerializeField]
        private float m_GameSpeed = 1f;

        [SerializeField]
        private bool m_RunInBackground = true;

        [SerializeField]
        private bool m_NeverSleep = true;

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）
        /// </summary>
        public bool EditorResourceMode
        {
            get
            {
                return m_EditorResourceMode;
            }
            set
            {
                m_EditorResourceMode = value;
            }
        }

        /// <summary>
        /// 获取或设置编辑器语言（仅编辑器内有效）
        /// </summary>
        public Language EditorLanguage
        {
            get
            {
                return m_EditorLanguage;
            }
            set
            {
                m_EditorLanguage = value;
            }
        }

        /// <summary>
        /// 获取或设置编辑器资源辅助器。
        /// </summary>
        public IResourceManager EditorResourceHelper
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置游戏帧率
        /// </summary>
        public int FrameRate
        {
            get
            {
                return m_FrameRate;
            }
            set
            {
                Application.targetFrameRate = m_FrameRate = value;
            }
        }

        /// <summary>
        /// 获取或设置游戏速度
        /// </summary>
        public float GameSpeed
        {
            get
            {
                return m_GameSpeed;
            }
            set
            {
                Time.timeScale = m_GameSpeed = value >= 0f ? value : 0f;
            }
        }

        /// <summary>
        /// 获取游戏是否暂停
        /// </summary>
        public bool IsGamePaused
        {
            get
            {
                return m_GameSpeed <= 0f;
            }
        }

        /// <summary>
        /// 获取是否正常游戏速度
        /// </summary>
        public bool IsNormalGameSpeed
        {
            get
            {
                return m_GameSpeed == 1f;
            }
        }

        /// <summary>
        /// 获取或设置是否允许后台运行
        /// </summary>
        public bool RunInBackground
        {
            get
            {
                return m_RunInBackground;
            }
            set
            {
                Application.runInBackground = m_RunInBackground = value;
            }
        }

        /// <summary>
        /// 获取或设置是否禁止休眠
        /// </summary>
        public bool NeverSleep
        {
            get
            {
                return m_NeverSleep;
            }
            set
            {
                m_NeverSleep = value;
                Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            /*
             * CYJStep0:
             * 这里是一切的入口，Unity调用BaseComponent的Awake方法，是整个框架的入口。
             * 每个继承自GameFrameworkComponent的组件都会调用父类的Awake函数。BaseComponent是最先调用的那个
             * 下面点开基类Awake函数进入下一步→
             */
            base.Awake();

            /*
             * CYJStep0-3:
             * 接下来开始对一些Helper和工具类做初始化设置，
             * 这样BaseComponent的Awake职责就算结束了，这里可以粗略的看看具体做了什么设置，但就不过多赘述了，
             * 按照顺序，接下来是一个如果在编辑器下用作资源加载的Helper => EditorResourceManagerComponent，
             * 它不派生自Component类，却也挂载在BaseComponent节点上，拥有Awake的实现。所以这里也要对他进行说明
             * 移步至EditorResourceComponent的Awake方法→
             */
            InitTextHelper();
            InitVersionHelper();
            InitLogHelper();
            Log.Info("<Framework> CmrFramework Version: {0}", CmrGameFramework.Version.GameFrameworkVersion);
            Log.Info($"<Framework> Game Version: {CmrGameFramework.Version.GameVersion} ({CmrGameFramework.Version.InternalGameVersion})");
            Log.Info("<Framework> Unity Version: {0}", Application.unityVersion);

            InitCompressionHelper();
            InitJsonHelper();

            Utility.Converter.ScreenDpi = Screen.dpi;
            if (Utility.Converter.ScreenDpi <= 0)
                Utility.Converter.ScreenDpi = DefaultDpi;

            m_EditorResourceMode &= Application.isEditor;
            if (m_EditorResourceMode)
                Log.Info("<Framework> During this run, Game Framework will use editor resource files, which you should validate first.");

            Application.targetFrameRate = m_FrameRate;
            Time.timeScale = m_GameSpeed;
            Application.runInBackground = m_RunInBackground;
            Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;

            Application.lowMemory += OnLowMemory;
        }
        private void Update()
        {
            GameFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        }
        private void OnApplicationQuit()
        {
            Application.lowMemory -= OnLowMemory;
            StopAllCoroutines();
        }
        private void OnDestroy()
        {
            GameFrameworkEntry.Shutdown();
        }

        /// <summary> 暂停游戏 </summary>
        public void PauseGame()
        {
            if (IsGamePaused)
                return;

            m_GameSpeedBeforePause = GameSpeed;
            GameSpeed = 0f;
        }

        /// <summary> 恢复游戏 </summary>
        public void ResumeGame()
        {
            if (!IsGamePaused)
                return;

            GameSpeed = m_GameSpeedBeforePause;
        }
        /// <summary>
        /// 重置为正常游戏速度
        /// </summary>
        public void ResetNormalGameSpeed()
        {
            if (IsNormalGameSpeed)
                return;
            GameSpeed = 1f;
        }

        internal void Shutdown()
        {
            Destroy(gameObject);
        }
        private void InitTextHelper()
        {
            if (string.IsNullOrEmpty(m_TextHelperTypeName))
                return;
            //CYJ:从程序集中获取到对应类型
            Type textHelperType = Utility.Assembly.GetType(m_TextHelperTypeName);

            if (textHelperType == null)
                throw Utility.Exception.ThrowException($"Can not find text helper type '{m_TextHelperTypeName}'.");

            Utility.Text.ITextHelper textHelper = (Utility.Text.ITextHelper)Activator.CreateInstance(textHelperType);
            if (textHelper == null)
                throw Utility.Exception.ThrowException($"Can not create text helper instance '{m_TextHelperTypeName}'.");

            Utility.Text.SetTextHelper(textHelper);
        }
        private void InitVersionHelper()
        {
            if (string.IsNullOrEmpty(m_VersionHelperTypeName))
                return;

            Type versionHelperType = Utility.Assembly.GetType(m_VersionHelperTypeName);
            if (versionHelperType == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not find version helper type '{0}'.", m_VersionHelperTypeName));

            CmrGameFramework.Version.IVersionHelper versionHelper = (CmrGameFramework.Version.IVersionHelper)Activator.CreateInstance(versionHelperType);
            if (versionHelper == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not create version helper instance '{0}'.", m_VersionHelperTypeName));

            CmrGameFramework.Version.SetVersionHelper(versionHelper);
        }
        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
                return;

            Type logHelperType = Utility.Assembly.GetType(m_LogHelperTypeName);
            if (logHelperType == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not find log helper type '{0}'.", m_LogHelperTypeName));

            GameFrameworkLog.ILogHelper logHelper = (GameFrameworkLog.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
                throw Utility.Exception.ThrowException(Utility.Text.Format("Can not create log helper instance '{0}'.", m_LogHelperTypeName));

            GameFrameworkLog.SetLogHelper(logHelper);
        }

        private void InitCompressionHelper()
        {
            if (string.IsNullOrEmpty(m_CompressionHelperTypeName))
                return;

            Type compressionHelperType = Utility.Assembly.GetType(m_CompressionHelperTypeName);
            if (compressionHelperType == null)
            {
                Log.Error("Can not find compression helper type '{0}'.", m_CompressionHelperTypeName);
                return;
            }

            Utility.Compression.ICompressionHelper compressionHelper = (Utility.Compression.ICompressionHelper)Activator.CreateInstance(compressionHelperType);
            if (compressionHelper == null)
            {
                Log.Error("Can not create compression helper instance '{0}'.", m_CompressionHelperTypeName);
                return;
            }

            Utility.Compression.SetCompressionHelper(compressionHelper);
        }

        private void InitJsonHelper()
        {
            if (string.IsNullOrEmpty(m_JsonHelperTypeName))
                return;

            Type jsonHelperType = Utility.Assembly.GetType(m_JsonHelperTypeName);
            if (jsonHelperType == null)
            {
                Log.Error("Can not find JSON helper type '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.IJsonHelper jsonHelper = (Utility.Json.IJsonHelper)Activator.CreateInstance(jsonHelperType);
            if (jsonHelper == null)
            {
                Log.Error("Can not create JSON helper instance '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.SetJsonHelper(jsonHelper);
        }

        /// <summary>
        /// CYJ:低内存回调，进行对象池内存的释放以及资源的卸载
        /// </summary>
        private void OnLowMemory()
        {
            Log.Info("Low memory reported...");

            ObjectPoolComponent objectPoolComponent = GameEntry.GetComponent<ObjectPoolComponent>();
            if (objectPoolComponent != null)
                objectPoolComponent.ReleaseAllUnused();

            ResourceComponent resourceCompoent = GameEntry.GetComponent<ResourceComponent>();
            if (resourceCompoent != null)
                resourceCompoent.ForceUnloadUnusedAssets(true);
        }
    }
}
