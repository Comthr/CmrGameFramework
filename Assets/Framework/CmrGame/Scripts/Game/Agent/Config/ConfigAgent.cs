using CmrGameFramework;
using CmrGameFramework.Config;
using CmrGameFramework.Event;
using CmrUnityFramework.Runtime;
using System;
using System.Collections.Generic;

namespace CmrGame
{
    /// <summary>
    /// 配置加载请求信息
    /// </summary>
    public class ConfigLoadRequest : IReference, IConfigUserData
    {
        public Type AccessorType { get; private set; }
        public string GroupName { get; private set; }
        public Action<ConfigAccessor> OnComplete { get; private set; }

        public static ConfigLoadRequest Create(Type type, string groupName, Action<ConfigAccessor> onComplete)
        {
            ConfigLoadRequest request = ReferencePool.Acquire<ConfigLoadRequest>();
            request.AccessorType = type;
            request.GroupName = groupName;
            request.OnComplete = onComplete;
            return request;
        }

        public void Clear()
        {
            AccessorType = null;
            GroupName = null;
            OnComplete = null;
        }
    }

    /// <summary>
    /// 配置代理
    /// </summary>
    public class ConfigAgent : GameAgent
    {
        // 缓存已加载的 Accessor [Type -> Instance]
        private readonly Dictionary<Type, ConfigAccessor> m_Accessors = new Dictionary<Type, ConfigAccessor>();

        // 缓存 Type 到 GroupName 的映射 (用于卸载)
        private readonly Dictionary<Type, string> m_ConfigGroups = new Dictionary<Type, string>();

        // 等待加载完成的请求 [GroupName -> Request]
        private readonly Dictionary<string, ConfigLoadRequest> m_PendingRequests = new Dictionary<string, ConfigLoadRequest>();

        // 事件句柄缓存
        private EventHandler<GameEventArgs> m_OnSuccessHandler;
        private EventHandler<GameEventArgs> m_OnFailureHandler;

        public override void Initialize(GameSession session)
        {
            base.Initialize(session);

            m_OnSuccessHandler = OnLoadConfigSuccess;
            m_OnFailureHandler = OnLoadConfigFailure;

            GameEntry.Event.Subscribe(LoadConfigSuccessEventArgs.EventId, m_OnSuccessHandler);
            GameEntry.Event.Subscribe(LoadConfigFailureEventArgs.EventId, m_OnFailureHandler);
        }

        public override void Shutdown()
        {
            GameEntry.Event.Unsubscribe(LoadConfigSuccessEventArgs.EventId, m_OnSuccessHandler);
            GameEntry.Event.Unsubscribe(LoadConfigFailureEventArgs.EventId, m_OnFailureHandler);

            foreach (var groupName in m_ConfigGroups.Values)
            {
                if (GameEntry.Config.HasConfigGroup(groupName))
                {
                    GameEntry.Config.RemoveConfigGroup(groupName);
                }
            }

            foreach (var request in m_PendingRequests.Values)
            {
                ReferencePool.Release(request);
            }

            m_Accessors.Clear();
            m_ConfigGroups.Clear();
            m_PendingRequests.Clear();
        }

        /// <summary>
        /// 加载配置并生成访问器
        /// </summary>
        /// <typeparam name="T">目标 Accessor 类型</typeparam>
        /// <param name="assetRelativePath">资源相对路径 (例如 "Lobby/Config_Lobby")</param>
        /// <param name="onComplete">完成回调</param>
        public void LoadConfig<T>(string assetRelativePath, Action<T> onComplete = null) where T : ConfigAccessor, new()
        {
            Type type = typeof(T);

            // 1. 已加载，直接返回
            if (m_Accessors.TryGetValue(type, out var existingAccessor))
            {
                onComplete?.Invoke(existingAccessor as T);
                return;
            }

            // 2. 构建资源路径和组名
            string fullPath = Utility.Asset.GetConfigAsset(Owner.GameType, assetRelativePath);
            string groupName = GetGroupNameFromPath(fullPath);

            // 3. 正在加载中，追加回调
            if (m_PendingRequests.ContainsKey(groupName))
            {
                Log.Warning($"[ConfigAgent] Config '{groupName}' is already loading, callback will be ignored.");
                return;
            }

            // 4. 创建请求并缓存
            var request = ConfigLoadRequest.Create(type, groupName, (accessor) =>
            {
                onComplete?.Invoke(accessor as T);
            });
            m_PendingRequests.Add(groupName, request);

            Log.Info($"[ConfigAgent] Request Load: {type.Name} -> {fullPath}");

            // 5. 调用底层接口，使用 request 作为 userData
            GameEntry.Config.ReadData(fullPath, request);
        }

        /// <summary>
        /// 获取已加载的访问器
        /// </summary>
        public T GetConfig<T>() where T : ConfigAccessor
        {
            if (m_Accessors.TryGetValue(typeof(T), out var accessor))
            {
                return accessor as T;
            }
            Log.Warning($"[ConfigAgent] ConfigAccessor '{typeof(T).Name}' has not been loaded yet.");
            return null;
        }

        /// <summary>
        /// 手动卸载指定配置
        /// </summary>
        public void UnloadConfig<T>() where T : ConfigAccessor
        {
            Type type = typeof(T);
            if (m_Accessors.Remove(type))
            {
                if (m_ConfigGroups.TryGetValue(type, out string groupName))
                {
                    GameEntry.Config.RemoveConfigGroup(groupName);
                    m_ConfigGroups.Remove(type);
                }
            }
        }

        #region Config Value Accessors

        public int GetInt(string groupName, string key, int defaultValue = 0)
        {
            return GameEntry.Config.GetIntFromGroup(groupName, key, defaultValue);
        }

        public float GetFloat(string groupName, string key, float defaultValue = 0f)
        {
            return GameEntry.Config.GetFloatFromGroup(groupName, key, defaultValue);
        }

        public string GetString(string groupName, string key, string defaultValue = null)
        {
            return GameEntry.Config.GetStringFromGroup(groupName, key, defaultValue);
        }

        public bool GetBool(string groupName, string key, bool defaultValue = false)
        {
            return GameEntry.Config.GetBoolFromGroup(groupName, key, defaultValue);
        }

        #endregion

        #region Event Handlers

        private void OnLoadConfigSuccess(object sender, GameEventArgs e)
        {
            var ne = (LoadConfigSuccessEventArgs)e;
            var request = ne.UserData as ConfigLoadRequest;

            if (request == null || !m_PendingRequests.ContainsKey(request.GroupName))
                return;

            m_PendingRequests.Remove(request.GroupName);

            try
            {
                // 创建 Accessor 实例
                var accessor = (ConfigAccessor)Activator.CreateInstance(request.AccessorType);
                accessor.Initialize(this, request.GroupName);

                // 缓存
                m_Accessors[request.AccessorType] = accessor;
                m_ConfigGroups[request.AccessorType] = request.GroupName;

                Log.Info($"[ConfigAgent] Load success: {request.AccessorType.Name}");
                request.OnComplete?.Invoke(accessor);
            }
            catch (Exception ex)
            {
                Log.Error($"[ConfigAgent] Failed to create accessor {request.AccessorType.Name}: {ex}");
                request.OnComplete?.Invoke(null);
            }
            finally
            {
                ReferencePool.Release(request);
            }
        }

        private void OnLoadConfigFailure(object sender, GameEventArgs e)
        {
            var ne = (LoadConfigFailureEventArgs)e;
            var request = ne.UserData as ConfigLoadRequest;

            if (request == null || !m_PendingRequests.ContainsKey(request.GroupName))
                return;

            m_PendingRequests.Remove(request.GroupName);

            Log.Error($"[ConfigAgent] Load failed for {request.AccessorType.Name}: {ne.ErrorMessage}");
            request.OnComplete?.Invoke(null);

            ReferencePool.Release(request);
        }

        #endregion

        private string GetGroupNameFromPath(string assetPath)
        {
            // 从路径提取文件名作为组名
            int lastSlash = assetPath.LastIndexOf('/');
            string fileName = lastSlash >= 0 ? assetPath.Substring(lastSlash + 1) : assetPath;
            int lastDot = fileName.LastIndexOf('.');
            return lastDot >= 0 ? fileName.Substring(0, lastDot) : fileName;
        }
    }
}