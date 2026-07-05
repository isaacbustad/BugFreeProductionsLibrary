// Created By   :   Isaac Bustad
// Created      :   7/4/2026



using UnityEngine;

namespace BugFreeProductions.Tools
{

    [CreateAssetMenu(fileName = "Clock_SCO", menuName = "ScriptableObject/Clock_SCO")]
    public class Clock_SCO : ScriptableObject
    {
        #region Methods
        public virtual bool CheckTimeStep(ref float timeElapsed, float timeStep)
        {
            if (timeElapsed >= timeStep)
            {
                timeElapsed = 0;
                return true;
            }
            return false;

        }
        #endregion Methods

    }
}