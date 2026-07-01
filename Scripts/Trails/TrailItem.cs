// Created By   :   Isaac Bustad
// Created      :   6/30/2026

using BugFreeProductions.Tools;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public class TrailItem : FactoryItem, ISubscriber
    {
        #region ISubscriber Implimentation
        public void Notify(ISubscriberNotification aSubMessage)
        {

        }

        // adds Subscriber to subscription
        public void Subscribe()
        {

        }

        // removes Subscriber to subscription
        public void UnSubscribe()
        {
            
        }
        #endregion ISubscriber Implimentation
    }
}