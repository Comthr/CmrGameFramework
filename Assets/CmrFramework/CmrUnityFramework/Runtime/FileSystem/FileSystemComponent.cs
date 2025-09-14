using CmrGameFramework;
using CmrGameFramework.FileSystem;
using System.Collections.Generic;
using UnityEngine;

namespace CmrUnityGameFramework.Runtime
{
    /// <summary>
    /// 文件系统件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("CmrFramework/File System")]
    public sealed class FileSystemComponent : GameFrameworkComponent
    {
        private IFileSystemManager m_FileSystemManager = null;

        [SerializeField]
        private string m_FileSystemHelperTypeName = "CmrUnityGameFramework.Runtime.DefaultFileSystemHelper";

        [SerializeField]
        private FileSystemHelperBase m_CustomFileSystemHelper = null;

        /// <summary>
        /// 获取文件系统数量。
        /// </summary>
        public int Count
        {
            get
            {
                return m_FileSystemManager.Count;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            /*
             * CYJStep0-6:
             * 显然，有些模块需要其专属Helper类，获取Helper类就是根据类型名去实例化一个，
             * 但如果场景中已经有对应类型的Helper（基本算是防止重复调用的处理）
             * 对应的Helper游戏物体就会直接设置在组件的子物体下。
             * FileSystem和SettingComponent类似，就是除了获取模块以外，还会创建一个Helper子物体
             * 一部分模块也会创建Helper子物体。只不过他们可能依赖其他模块，就不会在Awake中创建，这些特例我们放到Start的地方说
             * 至此，我们就把所有组件都说完了，接下来我们就要依次去看Start方法了，
             * 移步至ConfigComponent.Start→
             */

            base.Awake();

            m_FileSystemManager = GameFrameworkEntry.GetModule<IFileSystemManager>();
            if (m_FileSystemManager == null)
            {
                Log.Fatal("File system manager is invalid.");
                return;
            }

            FileSystemHelperBase fileSystemHelper = Helper.CreateHelper(m_FileSystemHelperTypeName, m_CustomFileSystemHelper);
            if (fileSystemHelper == null)
            {
                Log.Error("Can not create fileSystem helper.");
                return;
            }

            fileSystemHelper.name = "FileSystem Helper";
            Transform transform = fileSystemHelper.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_FileSystemManager.SetFileSystemHelper(fileSystemHelper);
        }

        /// <summary>
        /// 检查是否存在文件系统。
        /// </summary>
        /// <param name="fullPath">要检查的文件系统的完整路径。</param>
        /// <returns>是否存在文件系统。</returns>
        public bool HasFileSystem(string fullPath)
        {
            return m_FileSystemManager.HasFileSystem(fullPath);
        }

        /// <summary>
        /// 获取文件系统。
        /// </summary>
        /// <param name="fullPath">要获取的文件系统的完整路径。</param>
        /// <returns>获取的文件系统。</returns>
        public IFileSystem GetFileSystem(string fullPath)
        {
            return m_FileSystemManager.GetFileSystem(fullPath);
        }

        /// <summary>
        /// 创建文件系统。
        /// </summary>
        /// <param name="fullPath">要创建的文件系统的完整路径。</param>
        /// <param name="access">要创建的文件系统的访问方式。</param>
        /// <param name="maxFileCount">要创建的文件系统的最大文件数量。</param>
        /// <param name="maxBlockCount">要创建的文件系统的最大块数据数量。</param>
        /// <returns>创建的文件系统。</returns>
        public IFileSystem CreateFileSystem(string fullPath, FileSystemAccess access, int maxFileCount, int maxBlockCount)
        {
            return m_FileSystemManager.CreateFileSystem(fullPath, access, maxFileCount, maxBlockCount);
        }

        /// <summary>
        /// 加载文件系统。
        /// </summary>
        /// <param name="fullPath">要加载的文件系统的完整路径。</param>
        /// <param name="access">要加载的文件系统的访问方式。</param>
        /// <returns>加载的文件系统。</returns>
        public IFileSystem LoadFileSystem(string fullPath, FileSystemAccess access)
        {
            return m_FileSystemManager.LoadFileSystem(fullPath, access);
        }

        /// <summary>
        /// 销毁文件系统。
        /// </summary>
        /// <param name="fileSystem">要销毁的文件系统。</param>
        /// <param name="deletePhysicalFile">是否删除文件系统对应的物理文件。</param>
        public void DestroyFileSystem(IFileSystem fileSystem, bool deletePhysicalFile)
        {
            m_FileSystemManager.DestroyFileSystem(fileSystem, deletePhysicalFile);
        }

        /// <summary>
        /// 获取所有文件系统集合。
        /// </summary>
        /// <returns>获取的所有文件系统集合。</returns>
        public IFileSystem[] GetAllFileSystems()
        {
            return m_FileSystemManager.GetAllFileSystems();
        }

        /// <summary>
        /// 获取所有文件系统集合。
        /// </summary>
        /// <param name="results">获取的所有文件系统集合。</param>
        public void GetAllFileSystems(List<IFileSystem> results)
        {
            m_FileSystemManager.GetAllFileSystems(results);
        }
    }
}
