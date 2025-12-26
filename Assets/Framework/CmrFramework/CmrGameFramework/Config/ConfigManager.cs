using CmrGameFramework.Resource;
using System;
using System.Collections.Generic;

namespace CmrGameFramework.Config
{
    /// <summary>
    /// 配置管理器。
    /// </summary>
    internal sealed partial class ConfigManager : GameFrameworkModule, IConfigManager
    {
        /// <summary>
        /// 默认配置组名。
        /// </summary>
        public const string DEFAULT_GROUP_NAME = "DEFAULT";

        private readonly Dictionary<string, ConfigGroup> m_ConfigGroups;
        private readonly DataProvider<IConfigManager> m_DataProvider;
        private IConfigHelper m_ConfigHelper;

        /// <summary>
        /// 初始化配置管理器的新实例。
        /// </summary>
        public ConfigManager()
        {
            m_ConfigGroups = new Dictionary<string, ConfigGroup>(StringComparer.Ordinal);
            m_DataProvider = new DataProvider<IConfigManager>(this);
            m_ConfigHelper = null;
        }

        /// <summary>
        /// 获取配置分组数量。
        /// </summary>
        public int ConfigGroupCount
        {
            get
            {
                return m_ConfigGroups.Count;
            }
        }

        /// <summary>
        /// 获取配置项总数量。
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var group in m_ConfigGroups.Values)
                {
                    count += group.Count;
                }
                return count;
            }
        }

        /// <summary>
        /// 获取缓冲二进制流的大小。
        /// </summary>
        public int CachedBytesSize
        {
            get
            {
                return DataProvider<IConfigManager>.CachedBytesSize;
            }
        }

        /// <summary>
        /// 读取配置成功事件。
        /// </summary>
        public event EventHandler<ReadDataSuccessEventArgs> ReadDataSuccess
        {
            add
            {
                m_DataProvider.ReadDataSuccess += value;
            }
            remove
            {
                m_DataProvider.ReadDataSuccess -= value;
            }
        }

        /// <summary>
        /// 读取配置失败事件。
        /// </summary>
        public event EventHandler<ReadDataFailureEventArgs> ReadDataFailure
        {
            add
            {
                m_DataProvider.ReadDataFailure += value;
            }
            remove
            {
                m_DataProvider.ReadDataFailure -= value;
            }
        }

        /// <summary>
        /// 读取配置更新事件。
        /// </summary>
        public event EventHandler<ReadDataUpdateEventArgs> ReadDataUpdate
        {
            add
            {
                m_DataProvider.ReadDataUpdate += value;
            }
            remove
            {
                m_DataProvider.ReadDataUpdate -= value;
            }
        }

        /// <summary>
        /// 读取配置时加载依赖资源事件。
        /// </summary>
        public event EventHandler<ReadDataDependencyAssetEventArgs> ReadDataDependencyAsset
        {
            add
            {
                m_DataProvider.ReadDataDependencyAsset += value;
            }
            remove
            {
                m_DataProvider.ReadDataDependencyAsset -= value;
            }
        }

        /// <summary>
        /// 配置管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 关闭并清理配置管理器。
        /// </summary>
        internal override void Shutdown()
        {
            m_ConfigGroups.Clear();
        }

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resourceManager">资源管理器。</param>
        public void SetResourceManager(IResourceManager resourceManager)
        {
            m_DataProvider.SetResourceManager(resourceManager);
        }

        /// <summary>
        /// 设置配置数据提供者辅助器。
        /// </summary>
        /// <param name="dataProviderHelper">配置数据提供者辅助器。</param>
        public void SetDataProviderHelper(IDataProviderHelper<IConfigManager> dataProviderHelper)
        {
            m_DataProvider.SetDataProviderHelper(dataProviderHelper);
        }

        /// <summary>
        /// 设置配置辅助器。
        /// </summary>
        /// <param name="configHelper">配置辅助器。</param>
        public void SetConfigHelper(IConfigHelper configHelper)
        {
            if (configHelper == null)
            {
                throw new GameFrameworkException("Config helper is invalid.");
            }

            m_ConfigHelper = configHelper;
        }

        /// <summary>
        /// 确保二进制流缓存分配足够大小的内存并缓存。
        /// </summary>
        /// <param name="ensureSize">要确保二进制流缓存分配内存的大小。</param>
        public void EnsureCachedBytesSize(int ensureSize)
        {
            DataProvider<IConfigManager>.EnsureCachedBytesSize(ensureSize);
        }

        /// <summary>
        /// 释放缓存的二进制流。
        /// </summary>
        public void FreeCachedBytes()
        {
            DataProvider<IConfigManager>.FreeCachedBytes();
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        public void ReadData(string configAssetName)
        {
            m_DataProvider.ReadData(configAssetName, configAssetName);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        public void ReadData(string configAssetName, int priority)
        {
            m_DataProvider.ReadData(configAssetName, priority, configAssetName);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        public void ReadData(string configAssetName, object userData)
        {
            m_DataProvider.ReadData(configAssetName, userData);
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="priority">加载配置资源的优先级。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        public void ReadData(string configAssetName, int priority, object userData)
        {
            m_DataProvider.ReadData(configAssetName, priority, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configString">要解析的配置字符串。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(string configString)
        {
            return m_DataProvider.ParseData(configString, DEFAULT_GROUP_NAME);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configString">要解析的配置字符串。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(string configString, object userData)
        {
            if (userData == null)
            {
                return m_DataProvider.ParseData(configString,DEFAULT_GROUP_NAME);
            }
            return m_DataProvider.ParseData(configString, userData);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes)
        {
            return m_DataProvider.ParseData(configBytes, DEFAULT_GROUP_NAME);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes, object userData)
        {
            return m_DataProvider.ParseData(configBytes, userData);
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
            return m_DataProvider.ParseData(configBytes, startIndex, length, DEFAULT_GROUP_NAME);
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="startIndex">配置二进制流的起始位置。</param>
        /// <param name="length">配置二进制流的长度。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否解析配置成功。</returns>
        public bool ParseData(byte[] configBytes, int startIndex, int length, object userData)
        {
            return m_DataProvider.ParseData(configBytes, startIndex, length, userData);
        }

        /// <summary>
        /// 检查是否存在指定配置分组。
        /// </summary>
        /// <param name="groupName">要检查配置分组的名称。</param>
        /// <returns>指定的配置分组是否存在。</returns>
        public bool HasConfigGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("Config group name is invalid.");
            }

            return m_ConfigGroups.ContainsKey(groupName);
        }

        /// <summary>
        /// 获取指定配置分组内的配置项数量。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <returns>配置项数量。</returns>
        public int GetConfigCount(string groupName)
        {
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                throw new GameFrameworkException(Utility.Text.Format("Config group '{0}' is not exist.", groupName));
            }

            return configGroup.Count;
        }

        /// <summary>
        /// 获取所有配置分组名称。
        /// </summary>
        /// <returns>所有配置分组名称。</returns>
        public string[] GetAllConfigGroupNames()
        {
            int index = 0;
            string[] groupNames = new string[m_ConfigGroups.Count];
            foreach (var groupName in m_ConfigGroups.Keys)
            {
                groupNames[index++] = groupName;
            }
            return groupNames;
        }

        /// <summary>
        /// 检查默认配置分组中是否存在指定配置项。
        /// </summary>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasConfig(string configName)
        {
            return HasConfigInGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 检查指定配置分组中是否存在指定配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public bool HasConfigInGroup(string groupName, string configName)
        {
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return false;
            }

            return configGroup.HasConfig(configName);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName)
        {
            return GetBoolFromGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public bool GetBool(string configName, bool defaultValue)
        {
            return GetBoolFromGroup(DEFAULT_GROUP_NAME, configName, defaultValue);
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
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return defaultValue;
            }

            return configGroup.GetBool(configName, defaultValue);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName)
        {
            return GetIntFromGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public int GetInt(string configName, int defaultValue)
        {
            return GetIntFromGroup(DEFAULT_GROUP_NAME, configName, defaultValue);
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
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return defaultValue;
            }

            return configGroup.GetInt(configName, defaultValue);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName)
        {
            return GetFloatFromGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public float GetFloat(string configName, float defaultValue)
        {
            return GetFloatFromGroup(DEFAULT_GROUP_NAME, configName, defaultValue);
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
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return defaultValue;
            }

            return configGroup.GetFloat(configName, defaultValue);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName)
        {
            return GetStringFromGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 从默认配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public string GetString(string configName, string defaultValue)
        {
            return GetStringFromGroup(DEFAULT_GROUP_NAME, configName, defaultValue);
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
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return defaultValue;
            }

            return configGroup.GetString(configName, defaultValue);
        }

        /// <summary>
        /// 增加指定配置项到默认配置分组。
        /// </summary>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfig(string configName, string configValue)
        {
            return AddConfigToGroup(DEFAULT_GROUP_NAME, configName, configValue);
        }

        /// <summary>
        /// 增加指定配置项到默认配置分组。
        /// </summary>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="boolValue">配置项布尔值。</param>
        /// <param name="intValue">配置项整数值。</param>
        /// <param name="floatValue">配置项浮点数值。</param>
        /// <param name="stringValue">配置项字符串值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            return AddConfigToGroup(DEFAULT_GROUP_NAME, configName, boolValue, intValue, floatValue, stringValue);
        }

        /// <summary>
        /// 增加指定配置项到指定配置分组。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfig(string groupName, string configName, string configValue)
        {
            return AddConfigToGroup(groupName, configName, configValue);
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
        public bool AddConfig(string groupName, string configName, bool boolValue, int intValue, float floatValue, string stringValue)
        {
            return AddConfigToGroup(groupName, configName, boolValue, intValue, floatValue, stringValue);
        }

        /// <summary>
        /// 增加指定配置项到指定配置分组。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        public bool AddConfigToGroup(string groupName, string configName, string configValue)
        {
            ConfigGroup configGroup = GetOrCreateConfigGroup(groupName);
            return configGroup.AddConfig(configName, configValue);
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
            ConfigGroup configGroup = GetOrCreateConfigGroup(groupName);
            return configGroup.AddConfig(configName, boolValue, intValue, floatValue, stringValue);
        }

        /// <summary>
        /// 移除默认配置分组中的指定配置项。
        /// </summary>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        public bool RemoveConfig(string configName)
        {
            return RemoveConfigFromGroup(DEFAULT_GROUP_NAME, configName);
        }

        /// <summary>
        /// 移除指定配置分组中的配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        public bool RemoveConfigFromGroup(string groupName, string configName)
        {
            ConfigGroup configGroup = GetConfigGroup(groupName);
            if (configGroup == null)
            {
                return false;
            }

            return configGroup.RemoveConfig(configName);
        }

        /// <summary>
        /// 移除指定配置分组。
        /// </summary>
        /// <param name="groupName">要移除的配置分组名称。</param>
        /// <returns>是否移除配置分组成功。</returns>
        public bool RemoveConfigGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("Config group name is invalid.");
            }

            return m_ConfigGroups.Remove(groupName);
        }

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        public void RemoveAllConfigs()
        {
            m_ConfigGroups.Clear();
        }

        private ConfigGroup GetConfigGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("Config group name is invalid.");
            }

            ConfigGroup configGroup = null;
            if (m_ConfigGroups.TryGetValue(groupName, out configGroup))
            {
                return configGroup;
            }

            return null;
        }

        private ConfigGroup GetOrCreateConfigGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new GameFrameworkException("Config group name is invalid.");
            }

            ConfigGroup configGroup = null;
            if (!m_ConfigGroups.TryGetValue(groupName, out configGroup))
            {
                configGroup = new ConfigGroup(groupName);
                m_ConfigGroups.Add(groupName, configGroup);
            }

            return configGroup;
        }
        // 在 ConfigManager 类中添加

        ///// <summary>
        ///// 【新增】直接获取配置分组对象的引用
        ///// <para>供 ConfigAccessor 绑定使用，避免重复哈希查找。</para>
        ///// </summary>
        //public ConfigGroup GetConfigGroupObject(string groupName)
        //{
        //    if (string.IsNullOrEmpty(groupName)) return null;

        //    if (m_ConfigGroups.TryGetValue(groupName, out ConfigGroup group))
        //    {
        //        return group;
        //    }
        //    return null;
        //}
    }
}