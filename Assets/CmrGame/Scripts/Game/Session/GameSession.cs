using System;
using System.Collections.Generic;
using CmrUnityFramework.Runtime;

namespace CmrGame
{
    /// <summary>
    /// 游戏会话基类
    /// </summary>
    public abstract class GameSession
    {
        // 字典用于统一管理生命周期 (Update/Shutdown)
        private readonly Dictionary<Type, GameAgent> _agents = new Dictionary<Type, GameAgent>();

        public E_GameSession GameType { get; private set; }


        public ResourceAgent Resources { get; private set; }
        public PhaseAgent Logic { get; private set; } // 我建议叫 Logic 或 Phase

        protected GameSession(E_GameSession gameType)
        {
            GameType = gameType;

            // --- 关键点：在基类构造时，自动装配核心模块 ---
            // 这样任何继承 GameSession 的类，天生就拥有资源管理和状态机能力
            Resources = RegisterAgent<ResourceAgent>();
            //Logic = RegisterAgent<PhaseAgent>();
        }

        /// <summary>
        /// 注册代理 (供子类注册额外的 Agent)
        /// </summary>
        protected T RegisterAgent<T>() where T : GameAgent, new()
        {
            Type type = typeof(T);
            if (_agents.ContainsKey(type))
            {
                return _agents[type] as T;
            }

            T agent = new T();
            agent.Initialize(this); // 注入 Session 引用
            _agents.Add(type, agent);
            return agent;
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        public T GetAgent<T>() where T : GameAgent
        {
            if (_agents.TryGetValue(typeof(T), out var agent))
            {
                return agent as T;
            }
            return null;
        }

        /// <summary>
        /// 启动会话
        /// </summary>
        public abstract void Startup();

        // Update 和 Shutdown 保持不变，负责遍历 _agents
        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var agent in _agents.Values)
            {
                agent.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public void Shutdown()
        {
            // 反向遍历或者直接遍历都行，注意异常处理
            foreach (var agent in _agents.Values)
            {
                try { agent.Shutdown(); }
                catch (Exception e) { Log.Error(e.ToString()); }
            }
            _agents.Clear();
        }
    }
}