using UnityEngine;
using System.Collections.Generic;

namespace CmrGame.Editor
{
    /// <summary>
    /// Session ∆Ù∂Ø≈‰÷√ª˘¿‡
    /// </summary>
    public abstract class SessionConfigSO : ConfigSOBase 
    {
        public List<string> AvailablePhases = new List<string>();
        public string DefaultPhase;
    }
}