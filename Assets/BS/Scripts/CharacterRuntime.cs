using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSTool
{
    public class CharacterRuntime
    {
        private DRCharacter m_Source;

        private bool isBanned;//ÊÇ·ñ±»ban

        public int Id=>m_Source.Id;
        public string Name =>m_Source.Name;

        public bool IsBanned { get { return isBanned; } set { IsBanned = value; } }
        public CharacterRuntime(DRCharacter data)
        {
            m_Source = data;
            isBanned = false;
        }
    }
}


