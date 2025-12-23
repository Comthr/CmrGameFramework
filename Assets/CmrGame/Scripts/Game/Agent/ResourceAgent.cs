using CmrGameFramework;
using CmrGameFramework.Resource;
using CmrUnityFramework.Runtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public class ResourceAgent : GameAgent
    {
        private readonly HashSet<object> _loadedAssetHandles = new HashSet<object>();
        private LoadAssetCallbacks _cachedCallbacks;

        public override void Initialize(GameSession session)
        {
            base.Initialize(session);
            _cachedCallbacks = new LoadAssetCallbacks(OnLoadSuccess, OnLoadFailure);
        }

        public override void Shutdown()
        {
            foreach (var handle in _loadedAssetHandles)
            {
                if (handle != null) GameEntry.Resource.UnloadAsset(handle);
            }
            _loadedAssetHandles.Clear();
        }

        public void LoadAsset<T>(string assetName, Action<T> onSuccess, Action<string> onFailure = null) where T : UnityEngine.Object
        {
            var info = AgentCallbackInfo<object>.Acquire(
                (assetObj) => onSuccess?.Invoke(assetObj as T),
                onFailure
            );

            GameEntry.Resource.LoadAsset(assetName, typeof(T), _cachedCallbacks, info);
        }


        private void OnLoadSuccess(string assetName, object asset, float duration, object userData)
        {
            _loadedAssetHandles.Add(asset);

            var info = userData as AgentCallbackInfo<object>;
            if (info != null)
            {
                info.OnSuccess?.Invoke(asset);
                ReferencePool.Release(info);
            }
        }

        private void OnLoadFailure(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error($"Load Failure: {assetName}");
            var info = userData as AgentCallbackInfo<object>;
            if (info != null)
            {
                info.OnFailure?.Invoke(errorMessage);
                ReferencePool.Release(info);
            }
        }
    }
}