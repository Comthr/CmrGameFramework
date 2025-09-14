using System;

namespace CmrGameFramework
{
    public abstract class Variable: IReference
    {
        public abstract Type Type { get; }

        public Variable() { }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <returns> 变量值 </returns>
        public abstract object GetValue();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <returns> 变量值 </returns>
        public abstract void SetValue(object value);

        /// <summary> 清理变量值 </summary>
        public abstract void Clear();
    }
}
