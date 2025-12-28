using UnityEngine;

namespace CmrGame.Editor
{
    /// <summary>
    /// 配置结构定义基类
    /// </summary>
    public abstract class ConfigSOBase : ScriptableObject
    {
        public E_GameSession BelongSession = E_GameSession.Lobby;
    }
}