using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSTool
{
    public interface IBanPickHandler
    {
        public bool IsHost { get; }
        public bool IsBlue{ get; }
        public string GetCharacterNameById(int id); 
    }

}

