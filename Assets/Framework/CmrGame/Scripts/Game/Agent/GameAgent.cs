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
        public int Priority { get; protected set; } = 0;
        public GameSession Owner { get; private set; }

        public virtual void Initialize(GameSession session)
        {
            Owner = session;
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds) { }

        public abstract void Shutdown();


    }
}