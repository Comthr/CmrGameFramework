namespace CmrGame
{
    /// <summary>
    /// 配置访问器基类
    /// </summary>
    public abstract class ConfigAccessor
    {
        protected ConfigAgent Agent { get; private set; }
        protected string GroupName { get; private set; }

        public virtual void Initialize(ConfigAgent agent, string groupName)
        {
            Agent = agent;
            GroupName = groupName;
        }

        /// <summary>
        /// 访问器是否有效
        /// </summary>
        public bool IsValid => Agent != null && !string.IsNullOrEmpty(GroupName);

        // =========================================================
        // 核心封装：向 Agent 请求数据
        // =========================================================

        protected int GetInt(string key, int defaultValue = 0)
        {
            return Agent.GetInt(GroupName, key, defaultValue);
        }

        protected float GetFloat(string key, float defaultValue = 0f)
        {
            return Agent.GetFloat(GroupName, key, defaultValue);
        }

        protected string GetString(string key, string defaultValue = "")
        {
            return Agent.GetString(GroupName, key, defaultValue);
        }

        protected bool GetBool(string key, bool defaultValue = false)
        {
            return Agent.GetBool(GroupName, key, defaultValue);
        }
        protected int[] GetIntArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ConfigConverter.ParseIntArray(raw);
        }

        protected float[] GetFloatArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ConfigConverter.ParseFloatArray(raw);
        }

        protected string[] GetStringArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ConfigConverter.ParseStringArray(raw);
        }

        protected bool[] GetBoolArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ConfigConverter.ParseBoolArray(raw);
        }
    }
}