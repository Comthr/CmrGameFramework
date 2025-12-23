using CmrUnityFramework.Runtime;

namespace CmrGame
{
    public class DRUIForm:DataRowBase
    {
        private int m_Id = 0;
        private string m_AssetName;
        private string m_UIGroupName;
        private bool m_AllowMultiInstance;
        /// <summary>
        /// 获取界面编号。
        /// </summary>
        public override int Id => m_Id;

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName => m_AssetName;
        /// <summary>
        /// 获取界面组名称。
        /// </summary>
        public string UIGroupName => m_UIGroupName;
        /// <summary>
        /// 获取是否允许多个界面实例。
        /// </summary>
        public bool AllowMultiInstance => m_AllowMultiInstance;
        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataSplitSeparators);

            int index = 0;
            m_Id = int.Parse(columnStrings[index++]);
            m_AssetName = columnStrings[index++];
            m_UIGroupName = columnStrings[index++];
            m_AllowMultiInstance = int.Parse(columnStrings[index++]) == 1 ? true : false;
            // PauseCoveredUIForm = bool.Parse(columnStrings[index++]);

            return true;
        }
    }
}
