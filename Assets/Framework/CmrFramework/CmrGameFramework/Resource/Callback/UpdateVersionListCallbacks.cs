using System;
namespace CmrGameFramework.Resource
{
    /// <summary>
    /// 版本资源列表更新回调函数集。
    /// </summary>
    public sealed class UpdateVersionListCallbacks
    {
        private readonly UpdateVersionListSuccessCallback m_UpdateVersionListSuccessCallback;
        private readonly UpdateVersionListFailureCallback m_UpdateVersionListFailureCallback;

        /// <summary>
        /// 初始化版本资源列表更新回调函数集的新实例。
        /// </summary>
        /// <param name="updateVersionListSuccessCallback">版本资源列表更新成功回调函数。</param>
        public UpdateVersionListCallbacks(UpdateVersionListSuccessCallback updateVersionListSuccessCallback)
            : this(updateVersionListSuccessCallback, null)
        {
        }

        /// <summary>
        /// 初始化版本资源列表更新回调函数集的新实例。
        /// </summary>
        /// <param name="updateVersionListSuccessCallback">版本资源列表更新成功回调函数。</param>
        /// <param name="updateVersionListFailureCallback">版本资源列表更新失败回调函数。</param>
        public UpdateVersionListCallbacks(UpdateVersionListSuccessCallback updateVersionListSuccessCallback, UpdateVersionListFailureCallback updateVersionListFailureCallback)
        {
            if (updateVersionListSuccessCallback == null)
            {
                throw new GameFrameworkException("Update version list success callback is invalid.");
            }

            m_UpdateVersionListSuccessCallback = updateVersionListSuccessCallback;
            m_UpdateVersionListFailureCallback = updateVersionListFailureCallback;
        }

        /// <summary>
        /// 获取版本资源列表更新成功回调函数。
        /// </summary>
        public UpdateVersionListSuccessCallback UpdateVersionListSuccessCallback
        {
            get
            {
                return m_UpdateVersionListSuccessCallback;
            }
        }

        /// <summary>
        /// 获取版本资源列表更新失败回调函数。
        /// </summary>
        public UpdateVersionListFailureCallback UpdateVersionListFailureCallback
        {
            get
            {
                return m_UpdateVersionListFailureCallback;
            }
        }
    }

    /// <summary>
    /// 版本资源列表更新成功回调函数。
    /// </summary>
    /// <param name="downloadPath">版本资源列表更新后存放路径。</param>
    /// <param name="downloadUri">版本资源列表更新地址。</param>
    public delegate void UpdateVersionListSuccessCallback(string downloadPath, string downloadUri);

    /// <summary>
    /// 版本资源列表更新失败回调函数。
    /// </summary>
    /// <param name="downloadUri">版本资源列表更新地址。</param>
    /// <param name="errorMessage">错误信息。</param>
    public delegate void UpdateVersionListFailureCallback(string downloadUri, string errorMessage);
}
