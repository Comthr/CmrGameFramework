namespace CmrGameFramework.Config
{
    /// <summary>
    /// 配置用户数据接口，用于传递配置组名。
    /// </summary>
    public interface IConfigUserData
    {
        /// <summary>
        /// 获取配置组名。
        /// </summary>
        string GroupName { get; }
    }
}