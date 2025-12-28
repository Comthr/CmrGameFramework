using System.Collections.Generic;
using System.Linq;

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
            return ParseIntArray(raw);
        }

        protected float[] GetFloatArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ParseFloatArray(raw);
        }

        protected string[] GetStringArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ParseStringArray(raw);
        }

        protected bool[] GetBoolArray(string key)
        {
            string raw = Agent.GetString(GroupName, key, "");
            return ParseBoolArray(raw);
        }

        private const char SEPARATOR = '|';

        public static int[] ParseIntArray(string value)
        {
            if (string.IsNullOrEmpty(value)) return new int[0];
            return value.Split(SEPARATOR)
                .Select(s => int.TryParse(s, out var v) ? v : 0)
                .ToArray();
        }

        public static float[] ParseFloatArray(string value)
        {
            if (string.IsNullOrEmpty(value)) return new float[0];
            return value.Split(SEPARATOR)
                .Select(s => float.TryParse(s, out var v) ? v : 0f)
                .ToArray();
        }

        public static string[] ParseStringArray(string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return new string[0];
            return value.Split(SEPARATOR);
        }

        public static bool[] ParseBoolArray(string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return new bool[0];
            return value.Split(SEPARATOR)
                .Select(s => bool.TryParse(s, out var v) ? v : false)
                .ToArray();
        }

        public static string SaveArray<T>(IEnumerable<T> list)
        {
            if (list == null) 
                return "";
            return string.Join(SEPARATOR.ToString(), list);
        }
    }
}