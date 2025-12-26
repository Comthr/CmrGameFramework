using CmrGameFramework;
using CmrGameFramework.Config;
using CmrGameFramework.Resource;
using UnityEngine;

namespace CmrUnityFramework.Runtime
{
    /// <summary>
    /// 配置组件。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ConfigComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private IConfigManager m_ConfigManager = null;
        private EventComponent m_EventComponent = null;

        [SerializeField]
        private bool m_EnableLoadConfigUpdateEvent = false;

        [SerializeField]
        private bool m_EnableLoadConfigDependencyAssetEvent = false;

        [SerializeField]
        private string m_ConfigHelperTypeName = "CmrUnityFramework.Runtime.DefaultConfigHelper";

        [SerializeField]
        private ConfigHelperBase m_CustomConfigHelper = null;

        [SerializeField]
        private int m_CachedBytesSize = 0;

        /// <summary>
        /// 获取配置分组数量。
        /// </summary>
        public int ConfigGroupCount
        {
            get
            {
                return m_ConfigManager.ConfigGroupCount;
            }
        }

        /// <summary>
        /// 获取配置项总数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_ConfigManager.Count;
            }
        }

        /// <summary>
        /// 获取缓冲二进制流的大小。
        /// </summary>
        public int CachedBytesSize
        {
            get
            {
                return m_ConfigManager.CachedBytesSize;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_ConfigManager = GameFrameworkEntry.GetModule<IConfigManager>();
            if (m_ConfigManager == null)
            {
                Log.Fatal("Config manager is invalid.");
                return;
            }

            m_ConfigManager.ReadDataSuccess += OnReadDataSuccess;
            m_ConfigManager.ReadDataFailure += OnReadDataFailure;

            if (m_EnableLoadConfigUpdateEvent)
            {
                m_ConfigManager.ReadDataUpdate += OnReadDataUpdate;
            }

            if (m_EnableLoadConfigDependencyAssetEvent)
            {
                m_ConfigManager.ReadDataDependencyAsset += OnReadDataDependencyAsset;
            }
        }

        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_EventComponent = GameEntry.GetComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Log.Fatal("Event component is invalid.");
                return;
            }

            if (baseComponent.EditorResourceMode)
            {
                m_ConfigManager.SetResourceManager(baseComponent.EditorResourceHelper);
            }
            else
            {
                m_ConfigManager.SetResourceManager(GameFrameworkEntry.GetModule<IResourceManager>());
            }

            ConfigHelperBase configHelper = Helper.CreateHelper(m_ConfigHelperTypeName, m_CustomConfigHelper);
            if (configHelper == null)
            {
                Log.Error("Can not create config helper.");
                return;
            }

            configHelper.name = "Config Helper";
            Transform transform = configHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_ConfigManager.SetDataProviderHelper(configHelper);
            m_ConfigManager.SetConfigHelper(configHelper);
            if (m_CachedBytesSize > 0)
            {
                EnsureCachedBytesSize(m_CachedBytesSize);
            }
        }

        /// <summary>
        /// 确保二进制流缓存分配足够大小的内存并缓存。
        /// </summary>
        /// <param name="ensureSize">要确保二进制流缓存分配内存的大小。</param>
        public void EnsureCachedBytesSize(int ensureSize)
        {
            m_ConfigManager.EnsureCachedBytesSize(ensureSize);
        }

        /// <summary>
        /// 释放缓存的二进制流。
        /// </summary>
        public void FreeCachedBytes()
        {
            m_ConfigManager.FreeCachedBytes();
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        public void ReadData(string configAssetName)
        {
            m_ConfigManager.ReadData(configAssetName);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        public void ReadData(string configAssetName, int priority)
        {
            m_ConfigManager.ReadData(configAssetName, priority);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ReadData(string configAssetName, object userData)
        {
            m_ConfigManager.ReadData(configAssetName, userData);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ReadData(string configAssetName, int priority, object userData)
        {
            m_ConfigManager.ReadData(configAssetName, priority, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configString">要解析的配置字符串。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(string configString)
        {
            return m_ConfigManager.ParseData(configString);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configString">要解析的配置字符串。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(string configString, object userData)
        {
            return m_ConfigManager.ParseData(configString, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes)
        {
            return m_ConfigManager.ParseData(configBytes);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes, object userData)
        {
            return m_ConfigManager.ParseData(configBytes, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="startIndex">配置二进制流的起始位置。</param>
        /// <param name="length">配置二进制流的长度。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes, int startIndex, int length)
        {
            return m_ConfigManager.ParseData(configBytes, startIndex, length);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="startIndex">配置二进制流的起始位置。</param>
        /// <param name="length">配置二进制流的长度。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes, int startIndex, int length, object userData)
        {
            return m_ConfigManager.ParseData(configBytes, startIndex, length, userData);
        }

        /// <summary>
        /// 检查是否存在指定配置分组。
        /// </summary>
        /// <param name="groupName">要检查配置分组的名称。</param>
        /// <returns>指定的配置分组是否存在。</returns>
        public bool HasConfigGroup(string groupName)
        {
            return m_ConfigManager.HasConfigGroup(groupName);
        }

        /// <summary>
        /// 获取指定配置分组内的配置项数量。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <returns>配置项数量。</returns>
        public int GetConfigCount(string groupName)
        {
            return m_ConfigManager.GetConfigCount(groupName);
        }

        /// <summary>
        /// 获取所有配置分组名称。
        /// </summary>
        /// <returns>所有配置分组名称。</returns>
        public string[] GetAllConfigGroupNames()
        {
            return m_ConfigManager.GetAllConfigGroupNames();
        }

        /// <summary>
        /// 检查是否存在指定配置项。
        /// </summary>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasConfig(string configName)
        {
            return m_ConfigManager.HasConfig(configName);
        }

        /// <summary>
        /// 检查指定配置分组中是否存在指定配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasConfigInGroup(string groupName, string configName)
        {
            return m_ConfigManager.HasConfigInGroup(groupName, configName);
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName)
        {
            return m_ConfigManager.GetBool(configName);
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName, bool defaultValue)
        {
            return m_ConfigManager.GetBool(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBoolFromGroup(string groupName, string configName, bool defaultValue = false)
        {
            return m_ConfigManager.GetBoolFromGroup(groupName, configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName)
        {
            return m_ConfigManager.GetInt(configName);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName, int defaultValue)
        {
            return m_ConfigManager.GetInt(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetIntFromGroup(string groupName, string configName, int defaultValue = 0)
        {
            return m_ConfigManager.GetIntFromGroup(groupName, configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName)
        {
            return m_ConfigManager.GetFloat(configName);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName, float defaultValue)
        {
            return m_ConfigManager.GetFloat(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloatFromGroup(string groupName, string configName, float defaultValue = 0f)
        {
            return m_ConfigManager.GetFloatFromGroup(groupName, configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName)
        {
            return m_ConfigManager.GetString(configName);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName, string defaultValue)
        {
            return m_ConfigManager.GetString(configName, defaultValue);
        }

        /// <summary>
        /// 从指定配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetStringFromGroup(string groupName, string configName, string defaultValue = null)
        {
            return m_ConfigManager.GetStringFromGroup(groupName, configName, defaultValue);
        }

        /// <summary>
        /// 增加指定配置项到当前配置分组。
        /// </summary>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="boolValue">配置项布尔值。</param>
        /// <param name="intValue">配置项整数值。</param>
        /// <param name="floatValue">配置项浮点数值。</param>
        /// <param name="stringValue">配置项字符串值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            return m_ConfigManager.AddConfig(configName, boolValue, intValue, floatValue, stringValue);
        }

        /// <summary>
        /// 增加指定配置项到指定配置分组。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="boolValue">配置项布尔值。</param>
        /// <param name="intValue">配置项整数值。</param>
        /// <param name="floatValue">配置项浮点数值。</param>
        /// <param name="stringValue">配置项字符串值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfigToGroup(string groupName, string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            return m_ConfigManager.AddConfigToGroup(groupName, configName, boolValue, intValue, floatValue, stringValue);
        }

        /// <summary>
        /// 移除指定配置项。
        /// </summary>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        public bool RemoveConfig(string configName)
        {
            return m_ConfigManager.RemoveConfig(configName);
        }

        /// <summary>
        /// 移除指定配置分组中的配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        public bool RemoveConfigFromGroup(string groupName, string configName)
        {
            return m_ConfigManager.RemoveConfigFromGroup(groupName, configName);
        }

        /// <summary>
        /// 移除指定配置分组。
        /// </summary>
        /// <param name="groupName">要移除的配置分组名称。</param>
        /// <returns>是否移除配置分组成功。</returns>
        public bool RemoveConfigGroup(string groupName)
        {
            return m_ConfigManager.RemoveConfigGroup(groupName);
        }

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        public void RemoveAllConfigs()
        {
            m_ConfigManager.RemoveAllConfigs();
        }

        private void OnReadDataSuccess(object sender, ReadDataSuccessEventArgs e)
        {
            m_EventComponent.Fire(this, LoadConfigSuccessEventArgs.Create(e));
        }

        private void OnReadDataFailure(object sender, ReadDataFailureEventArgs e)
        {
            Log.Warning("Load config failure, asset name '{0}', error message '{1}'.", e.DataAssetName, e.ErrorMessage);
            m_EventComponent.Fire(this, LoadConfigFailureEventArgs.Create(e));
        }

        private void OnReadDataUpdate(object sender, ReadDataUpdateEventArgs e)
        {
            m_EventComponent.Fire(this, LoadConfigUpdateEventArgs.Create(e));
        }

        private void OnReadDataDependencyAsset(object sender, ReadDataDependencyAssetEventArgs e)
        {
            m_EventComponent.Fire(this, LoadConfigDependencyAssetEventArgs.Create(e));
        }
    }
}