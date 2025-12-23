using CmrGameFramework;
using System;
using UnityEngine;

namespace CmrGame
{
    /// <summary>
    /// 游戏代理基类
    /// </summary>
    public abstract class GameAgent
    {
        public GameSession OwnerSession { get; private set; }

        public virtual void Initialize(GameSession session)
        {
            OwnerSession = session;
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds) { }

        public abstract void Shutdown();


    }
}