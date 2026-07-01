// Created By   :   Isaac Bustad
// Created      :   6/30/2026

using UnityEngine;



namespace BugFreeProductions.Tools
{
    [CreateAssetMenu(fileName = "TrailFactory_SCO", menuName = "ScriptableObject/TrailFactory_SCO")]
    public class TrailFactory_SCO : GenericFactory_SCO
    {
        #region Vars
        protected ISubscription subscription = null;
        #endregion Vars



        #region Methods
        // orientation data, pool , the thing I want to subscribe to
        protected virtual void CreateItem(OrientationData aOD, ISubscription aSubscription)
        {
            CreateItem(aOD);
        }


        #endregion Methods
    }
}
