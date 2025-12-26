using CmrGameFramework;
using CmrGameFramework.Resource;
using CmrUnityFramework.Runtime;
using System;
using System.Collections.Generic;

namespace CmrGame
{
    public class ResourceAgent : GameAgent
    {
        // 缓存信息类
        private class AssetCacheInfo
        {
            public object assetHandle;         // 资源句柄
            public int refCount;               // 代理内部的引用计数
            public bool isLoaded;              // 是否已加载完成
            public List<object> callbacks; // 等待加载完成的回调列表 
            public AssetCacheInfo()
            {
                refCount = 0;
                isLoaded = false;
                callbacks = new List<object>();
            }
        }

        // 缓存字典
        private readonly Dictionary<string, AssetCacheInfo> _assetCache = new Dictionary<string, AssetCacheInfo>();

        // 资源句柄 -> 路径
        private readonly Dictionary<object, string> _handleToPathMap = new Dictionary<object, string>();

        private LoadAssetCallbacks _cachedCallbacks;

        public override void Initialize(GameSession session)
        {
            base.Initialize(session);
            _cachedCallbacks = new LoadAssetCallbacks(OnLoadSuccess, OnLoadFailure);
        }

        public override void Shutdown()
        {
            foreach (var kv in _assetCache)
            {
                var info = kv.Value;
                if (info.isLoaded && info.assetHandle != null)
                {
                    GameEntry.Resource.UnloadAsset(info.assetHandle);
                }
            }

            _assetCache.Clear();
            _handleToPathMap.Clear();
        }

        public void LoadAsset<T>(string assetPath, Action<T> onSuccess, Action<string> onFailure = null) where T : UnityEngine.Object
        {
            //检查是否已经在缓存中
            if (_assetCache.TryGetValue(assetPath, out AssetCacheInfo info))
            {
                info.refCount++;
                if (info.isLoaded)
                {
                    onSuccess?.Invoke(info.assetHandle as T);
                }
                else
                {
                    //等待加载完成
                    var callbackInfo = AgentCallbackInfo<object>.Acquire(
                        (obj) => onSuccess?.Invoke(obj as T),
                        onFailure
                    );
                    info.callbacks.Add(callbackInfo);
                }
            }
            else
            {
                info = new AssetCacheInfo();
                info.refCount = 1;

                var callbackInfo = AgentCallbackInfo<object>.Acquire(
                    (obj) => onSuccess?.Invoke(obj as T),
                    onFailure
                );
                info.callbacks.Add(callbackInfo);

                _assetCache.Add(assetPath, info);

                GameEntry.Resource.LoadAsset(assetPath, typeof(T), _cachedCallbacks);
            }
        }

        public void UnloadAsset(object assetHandle)
        {
            if (assetHandle == null) 
                return;

            if (_handleToPathMap.TryGetValue(assetHandle, out string path))
            {
                if (_assetCache.TryGetValue(path, out AssetCacheInfo info))
                {
                    info.refCount--;

                    if (info.refCount <= 0)
                    {
                        GameEntry.Resource.UnloadAsset(assetHandle);

                        _assetCache.Remove(path);
                        _handleToPathMap.Remove(assetHandle);
                    }
                }
            }
        }
        /// <summary>
        /// 加载成功回调
        /// </summary>
        private void OnLoadSuccess(string assetPath, object asset, float duration, object userData)
        {

            if (_assetCache.TryGetValue(assetPath, out AssetCacheInfo info))
            {
                info.isLoaded = true;
                info.assetHandle = asset;

                if (!_handleToPathMap.ContainsKey(asset))
                {
                    _handleToPathMap.Add(asset, assetPath);
                }

                foreach (var item in info.callbacks)
                {
                    var callbackInfo = item as AgentCallbackInfo<object>;
                    callbackInfo?.onSuccess?.Invoke(asset);
                    ReferencePool.Release(callbackInfo);
                }
                info.callbacks.Clear();
            }
            else
            {
                Log.Error($"Unexpected state: AssetCacheInfo not found for path '{assetPath}' on load success.");
            }
        }
        /// <summary>
        /// 加载失败回调
        /// </summary>
        private void OnLoadFailure(string assetPath, LoadResourceStatus status, string errorMessage, object userData)
        {

            if (_assetCache.TryGetValue(assetPath, out AssetCacheInfo info))
            {
                foreach (var item in info.callbacks)
                {
                    var callbackInfo = item as AgentCallbackInfo<object>;
                    callbackInfo?.onFailure?.Invoke(errorMessage);
                    ReferencePool.Release(callbackInfo);
                }
                _assetCache.Remove(assetPath);
            }
        }
    }
}