using System;
using CmrGameFramework;
using CmrUnityFramework.Runtime;

namespace CmrGame
{
    /// <summary>
    /// 游戏会话基类
    /// </summary>
    public abstract class GameSession
    {
        private readonly CachedLinkList<GameAgent> m_Agents = new CachedLinkList<GameAgent>();

        public E_GameSession GameType { get; private set; }

        public ResourceAgent Resources => GetAgent<ResourceAgent>();
        public ConfigAgent Config => GetAgent<ConfigAgent>();
        public PhaseAgent Phase => GetAgent<PhaseAgent>();

        protected GameSession(E_GameSession gameType)
        {
            GameType = gameType;
        }

        /// <summary>
        /// 获取或创建 Agent
        /// </summary>
        public T GetAgent<T>() where T : GameAgent, new()
        {
            Type type = typeof(T);

            foreach (var agent in m_Agents)
            {
                if (agent.GetType().Equals(type))
                {
                    return agent as T;
                }
            }
            T newAgent = RegisterAgent<T>();
            newAgent.Initialize(this);
            return newAgent;
        }
        public T RegisterAgent<T>() where T : GameAgent, new()
        {
            T newAgent = new T();
            m_Agents.AddLast(newAgent);
            return newAgent;
        }

        public abstract void Startup();

        public void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            var current = m_Agents.First;
            while (current != null)
            {
                var nextNode = current.Next;
                current.Value.Update(elapseSeconds, realElapseSeconds);
                current = nextNode;
            }
        }

        public void Shutdown()
        {
            foreach (var agent in m_Agents)
            {
                try
                {
                    agent.Shutdown();
                }
                catch (Exception e)
                {
                    Log.Error($"Agent Shutdown Error [{agent.GetType().Name}]: {e}");
                }
            }
            m_Agents.Clear();
        }
    }
}