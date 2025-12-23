using CmrGameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CmrGame
{
    public class AgentCallbackInfo<T> : IReference
    {
        public Action<T> OnSuccess;
        public Action<string> OnFailure;
        public object UserData;

        public static AgentCallbackInfo<T> Acquire(Action<T> success, Action<string> failure, object userData = null)
        {
            AgentCallbackInfo<T> info = ReferencePool.Acquire<AgentCallbackInfo<T>>();
            info.OnSuccess = success;
            info.OnFailure = failure;
            info.UserData = userData;
            return info;
        }

        public void Clear()
        {
            OnSuccess = null;
            OnFailure = null;
            UserData = null;
        }
    }
}
