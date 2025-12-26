using System;
using System.Collections.Generic;

namespace CmrGameFramework.Config
{
    internal sealed partial class ConfigManager : GameFrameworkModule, IConfigManager
    {
        /// <summary>
        /// 配置数据项。
        /// </summary>
        private struct ConfigData
        {
            private readonly bool m_BoolValue;
            private readonly int m_IntValue;
            private readonly float m_FloatValue;
            private readonly string m_StringValue;

            public ConfigData(bool boolValue, int intValue, float floatValue, string stringValue)
            {
                m_BoolValue = boolValue;
                m_IntValue = intValue;
                m_FloatValue = floatValue;
                m_StringValue = stringValue;
            }

            public bool BoolValue
            {
                get
                {
                    return m_BoolValue;
                }
            }

            public int IntValue
            {
                get
                {
                    return m_IntValue;
                }
            }

            public float FloatValue
            {
                get
                {
                    return m_FloatValue;
                }
            }

            public string StringValue
            {
                get
                {
                    return m_StringValue;
                }
            }
        }

        /// <summary>
        /// 配置分组，按表分组存储配置数据。
        /// </summary>
        private sealed class ConfigGroup
        {
            private readonly string m_GroupName;
            private readonly Dictionary<string, ConfigData> m_ConfigDatas;

            public ConfigGroup(string groupName)
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    throw new GameFrameworkException("Config group name is invalid.");
                }

                m_GroupName = groupName;
                m_ConfigDatas = new Dictionary<string, ConfigData>(StringComparer.Ordinal);
            }

            /// <summary>
            /// 获取配置分组名称。
            /// </summary>
            public string GroupName
            {
                get
                {
                    return m_GroupName;
                }
            }

            /// <summary>
            /// 获取配置分组内配置项数量。
            /// </summary>
            public int Count
            {
                get
                {
                    return m_ConfigDatas.Count;
                }
            }

            /// <summary>
            /// 检查是否存在指定配置项。
            /// </summary>
            /// <param name="configName">要检查配置项的名称。</param>
            /// <returns>指定的配置项是否存在。</returns>
            public bool HasConfig(string configName)
            {
                return GetConfigData(configName).HasValue;
            }

            /// <summary>
            /// 从指定配置项中读取布尔值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <returns>读取的布尔值。</returns>
            public bool GetBool(string configName)
            {
                ConfigData? configData = GetConfigData(configName);
                if (!configData.HasValue)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Config name '{0}' is not exist in group '{1}'.", configName, m_GroupName));
                }

                return configData.Value.BoolValue;
            }

            /// <summary>
            /// 从指定配置项中读取布尔值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
            /// <returns>读取的布尔值。</returns>
            public bool GetBool(string configName, bool defaultValue)
            {
                ConfigData? configData = GetConfigData(configName);
                return configData.HasValue ? configData.Value.BoolValue : defaultValue;
            }

            /// <summary>
            /// 从指定配置项中读取整数值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <returns>读取的整数值。</returns>
            public int GetInt(string configName)
            {
                ConfigData? configData = GetConfigData(configName);
                if (!configData.HasValue)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Config name '{0}' is not exist in group '{1}'.", configName, m_GroupName));
                }

                return configData.Value.IntValue;
            }

            /// <summary>
            /// 从指定配置项中读取整数值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
            /// <returns>读取的整数值。</returns>
            public int GetInt(string configName, int defaultValue)
            {
                ConfigData? configData = GetConfigData(configName);
                return configData.HasValue ? configData.Value.IntValue : defaultValue;
            }

            /// <summary>
            /// 从指定配置项中读取浮点数值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <returns>读取的浮点数值。</returns>
            public float GetFloat(string configName)
            {
                ConfigData? configData = GetConfigData(configName);
                if (!configData.HasValue)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Config name '{0}' is not exist in group '{1}'.", configName, m_GroupName));
                }

                return configData.Value.FloatValue;
            }

            /// <summary>
            /// 从指定配置项中读取浮点数值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
            /// <returns>读取的浮点数值。</returns>
            public float GetFloat(string configName, float defaultValue)
            {
                ConfigData? configData = GetConfigData(configName);
                return configData.HasValue ? configData.Value.FloatValue : defaultValue;
            }

            /// <summary>
            /// 从指定配置项中读取字符串值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <returns>读取的字符串值。</returns>
            public string GetString(string configName)
            {
                ConfigData? configData = GetConfigData(configName);
                if (!configData.HasValue)
                {
                    throw new GameFrameworkException(Utility.Text.Format("Config name '{0}' is not exist in group '{1}'.", configName, m_GroupName));
                }

                return configData.Value.StringValue;
            }

            /// <summary>
            /// 从指定配置项中读取字符串值。
            /// </summary>
            /// <param name="configName">要获取配置项的名称。</param>
            /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
            /// <returns>读取的字符串值。</returns>
            public string GetString(string configName, string defaultValue)
            {
                ConfigData? configData = GetConfigData(configName);
                return configData.HasValue ? configData.Value.StringValue : defaultValue;
            }

            /// <summary>
            /// 增加指定配置项。
            /// </summary>
            /// <param name="configName">要增加配置项的名称。</param>
            /// <param name="configValue">配置项的值。</param>
            /// <returns>是否增加配置项成功。</returns>
            public bool AddConfig(string configName, string configValue)
            {
                bool boolValue = false;
                bool.TryParse(configValue, out boolValue);

                int intValue = 0;
                int.TryParse(configValue, out intValue);

                float floatValue = 0f;
                float.TryParse(configValue, out floatValue);

                return AddConfig(configName, boolValue, intValue, floatValue, configValue);
            }

            /// <summary>
            /// 增加指定配置项。
            /// </summary>
            /// <param name="configName">要增加配置项的名称。</param>
            /// <param name="boolValue">配置项布尔值。</param>
            /// <param name="intValue">配置项整数值。</param>
            /// <param name="floatValue">配置项浮点数值。</param>
            /// <param name="stringValue">配置项字符串值。</param>
            /// <returns>是否增加配置项成功。</returns>
            public bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue)
            {
                if (HasConfig(configName))
                {
                    return false;
                }

                m_ConfigDatas.Add(configName, new ConfigData(boolValue, intValue, floatValue, stringValue));
                return true;
            }

            /// <summary>
            /// 移除指定配置项。
            /// </summary>
            /// <param name="configName">要移除配置项的名称。</param>
            /// <returns>是否移除配置项成功。</returns>
            public bool RemoveConfig(string configName)
            {
                return m_ConfigDatas.Remove(configName);
            }

            /// <summary>
            /// 清空所有配置项。
            /// </summary>
            public void RemoveAllConfigs()
            {
                m_ConfigDatas.Clear();
            }

            private ConfigData? GetConfigData(string configName)
            {
                if (string.IsNullOrEmpty(configName))
                {
                    throw new GameFrameworkException("Config name is invalid.");
                }

                ConfigData configData = default(ConfigData);
                if (m_ConfigDatas.TryGetValue(configName, out configData))
                {
                    return configData;
                }

                return null;
            }
        }
    }
}
