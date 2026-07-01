// Created By   :   Isaac Bustad
// Created      :   7/1/2026


using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public class SubscribingFactoryItem : FactoryItem, ISubscriber
    {
        #region Vars
        protected List<ISubscription> subscriptions = new List<ISubscription>();
        #endregion Vars


        #region Methods

        #region Implement ISubscriber
        public void Notify(ISubscriberNotification aSubMessage)
        {

        }

        // adds Subscriber to subscription
        public void Subscribe()
        {

        }

        // adds Subscriber to subscription
        public void Subscribe(ISubscription aSubscription)
        {
            aSubscription.Subscribe(this);
            subscriptions.Add(aSubscription);
        }

        // removes Subscriber to subscription
        public void UnSubscribe()
        {
            foreach (ISubscription aSubscription in subscriptions)
            {
                aSubscription.UnSubscribe(this);
                subscriptions.Remove(aSubscription);
            }
        }
        
        #endregion Implement ISubscriber

        #endregion Methods

    }
}