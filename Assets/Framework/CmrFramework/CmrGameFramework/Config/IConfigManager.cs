using CmrGameFramework.Resource;

namespace CmrGameFramework.Config
{
    /// <summary>
    /// 配置管理器接口。
    /// </summary>
    public interface IConfigManager : IDataProvider<IConfigManager>
    {
        /// <summary>
        /// 获取配置分组数量。
        /// </summary>
        int ConfigGroupCount
        {
            get;
        }

        /// <summary>
        /// 获取配置项总数量。
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// 获取缓冲二进制流的大小。
        /// </summary>
        int CachedBytesSize
        {
            get;
        }

        /// <summary>
        /// 设置资源管理器。
        /// </summary>
        /// <param name="resourceManager">资源管理器。</param>
        void SetResourceManager(IResourceManager resourceManager);

        /// <summary>
        /// 设置配置数据提供者辅助器。
        /// </summary>
        /// <param name="dataProviderHelper">配置数据提供者辅助器。</param>
        void SetDataProviderHelper(IDataProviderHelper<IConfigManager> dataProviderHelper);

        /// <summary>
        /// 设置配置辅助器。
        /// </summary>
        /// <param name="configHelper">配置辅助器。</param>
        void SetConfigHelper(IConfigHelper configHelper);

        /// <summary>
        /// 确保二进制流缓存分配足够大小的内存并缓存。
        /// </summary>
        /// <param name="ensureSize">要确保二进制流缓存分配内存的大小。</param>
        void EnsureCachedBytesSize(int ensureSize);

        /// <summary>
        /// 释放缓存的二进制流。
        /// </summary>
        void FreeCachedBytes();

        /// <summary>
        /// 检查是否存在指定配置分组。
        /// </summary>
        /// <param name="groupName">要检查配置分组的名称。</param>
        /// <returns>指定的配置分组是否存在。</returns>
        bool HasConfigGroup(string groupName);

        /// <summary>
        /// 获取指定配置分组内的配置项数量。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <returns>配置项数量。</returns>
        int GetConfigCount(string groupName);

        /// <summary>
        /// 获取所有配置分组名称。
        /// </summary>
        /// <returns>所有配置分组名称。</returns>
        string[] GetAllConfigGroupNames();

        /// <summary>
        /// 检查默认配置分组中是否存在指定配置项。
        /// </summary>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        bool HasConfig(string configName);

        /// <summary>
        /// 检查指定配置分组中是否存在指定配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        bool HasConfigInGroup(string groupName, string configName);

        /// <summary>
        /// 从默认配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetBool(string configName);

        /// <summary>
        /// 从默认配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetBool(string configName, bool defaultValue);

        /// <summary>
        /// 从指定配置分组的配置项中读取布尔值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        bool GetBoolFromGroup(string groupName, string configName, bool defaultValue = false);

        /// <summary>
        /// 从默认配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        int GetInt(string configName);

        /// <summary>
        /// 从默认配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        int GetInt(string configName, int defaultValue);

        /// <summary>
        /// 从指定配置分组的配置项中读取整数值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        int GetIntFromGroup(string groupName, string configName, int defaultValue = 0);

        /// <summary>
        /// 从默认配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetFloat(string configName);

        /// <summary>
        /// 从默认配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetFloat(string configName, float defaultValue);

        /// <summary>
        /// 从指定配置分组的配置项中读取浮点数值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        float GetFloatFromGroup(string groupName, string configName, float defaultValue = 0f);

        /// <summary>
        /// 从默认配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        string GetString(string configName);

        /// <summary>
        /// 从默认配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        string GetString(string configName, string defaultValue);

        /// <summary>
        /// 从指定配置分组的配置项中读取字符串值。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        string GetStringFromGroup(string groupName, string configName, string defaultValue = null);

        /// <summary>
        /// 增加指定配置项到默认配置分组。
        /// </summary>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        bool AddConfig(string configName, string configValue);

        /// <summary>
        /// 增加指定配置项到默认配置分组。
        /// </summary>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="boolValue">配置项布尔值。</param>
        /// <param name="intValue">配置项整数值。</param>
        /// <param name="floatValue">配置项浮点数值。</param>
        /// <param name="stringValue">配置项字符串值。</param>
        /// <returns>是否增加配置项成功。</returns>
        bool AddConfig(string configName, bool boolValue, int intValue, float floatValue, string stringValue);

        /// <summary>
        /// 增加指定配置项到指定配置分组。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        bool AddConfig(string groupName, string configName, string configValue);

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
        bool AddConfig(string groupName, string configName, bool boolValue, int intValue, float floatValue, string stringValue);

        /// <summary>
        /// 增加指定配置项到指定配置分组。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要增加配置项的名称。</param>
        /// <param name="configValue">配置项的值。</param>
        /// <returns>是否增加配置项成功。</returns>
        bool AddConfigToGroup(string groupName, string configName, string configValue);

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
        bool AddConfigToGroup(string groupName, string configName, bool boolValue, int intValue, float floatValue, string stringValue);

        /// <summary>
        /// 移除默认配置分组中的指定配置项。
        /// </summary>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        bool RemoveConfig(string configName);

        /// <summary>
        /// 移除指定配置分组中的配置项。
        /// </summary>
        /// <param name="groupName">配置分组名称。</param>
        /// <param name="configName">要移除配置项的名称。</param>
        /// <returns>是否移除配置项成功。</returns>
        bool RemoveConfigFromGroup(string groupName, string configName);

        /// <summary>
        /// 移除指定配置分组。
        /// </summary>
        /// <param name="groupName">要移除的配置分组名称。</param>
        /// <returns>是否移除配置分组成功。</returns>
        bool RemoveConfigGroup(string groupName);

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        void RemoveAllConfigs();
    }
}