using CmrGameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public class AgentCallbackInfo<T> : IReference
    {
        public Action<T> onSuccess;
        public Action<string> onFailure;
        public object userData;

        public static AgentCallbackInfo<T> Acquire(Action<T> success, Action<string> failure, object userData = null)
        {
            AgentCallbackInfo<T> info = ReferencePool.Acquire<AgentCallbackInfo<T>>();
            info.onSuccess = success;
            info.onFailure = failure;
            info.userData = userData;
            return info;
        }

        public void Clear()
        {
            onSuccess = null;
            onFailure = null;
            userData = null;
        }
    }
}
