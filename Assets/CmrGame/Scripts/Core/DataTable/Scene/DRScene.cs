using CmrUnityGameFramework.Runtime;

namespace CmrGame
{
    public class DRScene : DataRowBase
    {
        private int m_Id = 0;
        private string sceneName;
        private string assetName;
        private int backgroundMusicId;
        private string procedureName;
        public override int Id => m_Id;
        public string SceneName =>sceneName;
        public string AssetName =>assetName;
        public int BackgroundMusicId =>backgroundMusicId;
        public string ProcedureName=>procedureName;

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataSplitSeparators);
            int index = 0;
            //index++;
            m_Id = int.Parse(columnStrings[index++]);
            sceneName = columnStrings[index++];
            assetName = columnStrings[index++];
            backgroundMusicId = int.Parse(columnStrings[index++]);
            procedureName = columnStrings[index++];
            return true;
        }
    }
}
