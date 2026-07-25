// Created By   :   Isaac Bustad
// Created      :   7/24/2026


using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public class TargetingNode : MonoBehaviour, ISubscriber
    {
        #region Vars
        [SerializeField] protected TargetNode targetNode = null;
        [SerializeField] protected TargetingSystemParams_SCO targetingSystemParams_SCO = null;
        

        #endregion Vars


        #region Methods

        #region Unity Methods

        #endregion Unity Methods
        
        #region Implement ISubscriber
        public void OnNotify(ISubscriberNotification aSubMessage)
        {

        }

        // adds Subscriber to subscription
        public void OnSubscribe()
        {

        }

        // adds Subscriber to subscription
        public void OnSubscribe(ISubscription aSubscription)
        {

        }

        // removes Subscriber to subscription
        public void OnUnSubscribe()
        {

        }

        // removes Subscriber to subscription
        public void OnUnSubscribe(ISubscription aSubscription)
        {

        }
        #endregion Implement ISubscriber
        
        

        #endregion Methods


        #region Accessors
        public TargetNode TargetNode
        {
            get
            {
                return targetNode;
            }
        }

        #endregion Accessors
    }


    public enum TargetingType
    {
        hold,
        toggle,
        auto
    }
}