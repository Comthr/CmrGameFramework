using CmrGameFramework;
using CmrGameFramework.Config;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace CmrUnityFramework.Runtime
{
    /// <summary>
    /// 默认配置辅助器。
    /// </summary>
    public class DefaultConfigHelper : ConfigHelperBase
    {
        private static readonly string[] ColumnSplitSeparator = new string[] { "\t" };
        private static readonly string BytesAssetExtension = ".bytes";
        private const int ColumnCount = 4;

        private ResourceComponent m_ResourceComponent = null;

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configManager">配置管理器。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="configAsset">配置资源。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否读取配置成功。</returns>
        public override bool ReadData(IConfigManager configManager, string configAssetName, object configAsset, object userData)
        {
            TextAsset configTextAsset = configAsset as TextAsset;
            if (configTextAsset != null)
            {
                if (configAssetName.EndsWith(BytesAssetExtension, StringComparison.Ordinal))
                {
                    return configManager.ParseData(configTextAsset.bytes, userData);
                }
                else
                {
                    return configManager.ParseData(configTextAsset.text, userData);
                }
            }

            Log.Warning("Config asset '{0}' is invalid.", configAssetName);
            return false;
        }

        /// <summary>
        /// 读取配置。
        /// </summary>
        /// <param name="configManager">配置管理器。</param>
        /// <param name="configAssetName">配置资源名称。</param>
        /// <param name="configBytes">配置二进制流。</param>
        /// <param name="startIndex">配置二进制流的起始位置。</param>
        /// <param name="length">配置二进制流的长度。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否读取配置成功。</returns>
        public override bool ReadData(IConfigManager configManager, string configAssetName, byte[] configBytes, int startIndex, int length, object userData)
        {
            if (configAssetName.EndsWith(BytesAssetExtension, StringComparison.Ordinal))
            {
                return configManager.ParseData(configBytes, startIndex, length, userData);
            }
            else
            {
                return configManager.ParseData(Utility.Converter.GetString(configBytes, startIndex, length), userData);
            }
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configManager">配置管理器。</param>
        /// <param name="configString">要解析的配置字符串。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否解析配置成功。</returns>
        public override bool ParseData(IConfigManager configManager, string configString, object userData)
        {
            try
            {
                string groupName = GetGroupName(userData);
                
                int position = 0;
                string configLineString = null;
                while ((configLineString = configString.ReadLine(ref position)) != null)
                {
                    if (configLineString[0] == '#')
                    {
                        continue;
                    }

                    string[] splitedLine = configLineString.Split(ColumnSplitSeparator, StringSplitOptions.None);
                    if (splitedLine.Length != ColumnCount)
                    {
                        Log.Warning("Can not parse config line string '{0}' which column count is invalid.", configLineString);
                        return false;
                    }

                    string configName = splitedLine[1];
                    string configValue = splitedLine[3];
                    if (!configManager.AddConfig(groupName, configName, configValue))
                    {
                        Log.Warning("Can not add config with config name '{0}' which may be invalid or duplicate.", configName);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse config string with exception '{0}'.", exception);
                return false;
            }
        }

        /// <summary>
        /// 解析配置。
        /// </summary>
        /// <param name="configManager">配置管理器。</param>
        /// <param name="configBytes">要解析的配置二进制流。</param>
        /// <param name="startIndex">配置二进制流的起始位置。</param>
        /// <param name="length">配置二进制流的长度。</param>
        /// <param name="userData">用户自定义数据（配置组名）。</param>
        /// <returns>是否解析配置成功。</returns>
        public override bool ParseData(IConfigManager configManager, byte[] configBytes, int startIndex, int length, object userData)
        {
            try
            {
                string groupName = GetGroupName(userData);
                
                using (MemoryStream memoryStream = new MemoryStream(configBytes, startIndex, length, false))
                {
                    using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                    {
                        while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                        {
                            string configName = binaryReader.ReadString();
                            string configValue = binaryReader.ReadString();
                            if (!configManager.AddConfig(groupName, configName, configValue))
                            {
                                Log.Warning("Can not add config with config name '{0}' which may be invalid or duplicate.", configName);
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                Log.Warning("Can not parse config bytes with exception '{0}'.", exception);
                return false;
            }
        }

        /// <summary>
        /// 释放配置资源。
        /// </summary>
        /// <param name="configManager">配置管理器。</param>
        /// <param name="configAsset">要释放的配置资源。</param>
        public override void ReleaseDataAsset(IConfigManager configManager, object configAsset)
        {
            m_ResourceComponent.UnloadAsset(configAsset);
        }

        private void Start()
        {
            m_ResourceComponent = GameEntry.GetComponent<ResourceComponent>();
            if (m_ResourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }

        private string GetGroupName(object userData)
        {
            if (userData == null)
            {
                Utility.Exception.ThrowException("UserData is null, cannot get group name.");
                return null;
            }

            // 1. 如果是字符串，直接使用
            if (userData is string groupName)
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    Utility.Exception.ThrowException("Group name is null or empty.");
                }
                return groupName;
            }

            // 2. 如果实现了 IConfigUserData 接口，获取 GroupName
            if (userData is IConfigUserData configUserData)
            {
                return configUserData.GroupName;
            }

            // 3. 其他情况，抛出异常
            Utility.Exception.ThrowException($"Unsupported userData type: {userData.GetType().Name}");
            return null;
        }
    }
}