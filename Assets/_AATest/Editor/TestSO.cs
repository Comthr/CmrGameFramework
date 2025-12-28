using UnityEngine;

namespace CmrGame.Editor
{
    [CreateAssetMenu(fileName = "TestSO", menuName = "CmrGame/Lobby/Config/TestSO")]
    public class TestSO : ConfigSOBase
    {
        public string testStrA;
        public string[] textStrArr;
        public int testInt;
        public bool testBool;
        public int[] testIntArr;
    }
}
