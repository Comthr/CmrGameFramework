using CmrUnityFramework.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSTool
{
    public class DRCharacter : DataRowBase
    {
        private int m_Id;
        private string m_Name;
        public override int Id => m_Id;
        public string Name => m_Name;

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataSplitSeparators);
            int index = 0;
            m_Id = int.Parse(columnStrings[index++]);
            m_Name = columnStrings[index++];
            return true;
        }
    }

}

