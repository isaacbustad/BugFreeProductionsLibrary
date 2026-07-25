// Created By   :   Isaac Bustad
// Created      :   7/25/2026



using UnityEngine;



namespace BugFreeProductions
{
    [CreateAssetMenu(fileName = "TargetingSystemParams_SCO", menuName = "ScriptableObject/TargetingSystemParams_SCO")]
    public class TargetingSystemParams_SCO : ScriptableObject
    {
        #region Vars
        protected float targetingRange;
        protected float targetingInterval;

        #endregion Vars



        #region Methods

        #endregion Methods



        #region Accessors
        public float TargetingRange
        {
            get
            {
                return targetingRange;
            }
        }

        public float TargetingInterval
        {
            get
            {
                return targetingInterval;
            }
        }

        #endregion Accessors
        
    }
}