// Created By   :   Isaac Bustad
// Created      :   5/27/2026

using UnityEngine;

namespace BugFreeProductions.Tools
{
    public class SubMessage
    {
        #region Vars

        protected ISubscription subscription = null;

        #endregion Vars



        #region Constructors
        public SubMessage(ISubscription aSubscription)
        {
            subscription = aSubscription;
        }

        #endregion Constructors

        #region Accessors

        #endregion Accessors
    }
}
