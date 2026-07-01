// Isaac Bustad
// 5/6/2024

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public interface ISubscriber
    {
        // Methods
        // runs when Subscription notifies subscribers
        // add this to subscriber delegate
        public void Notify(ISubscriberNotification aSubMessage);

        // adds Subscriber to subscription
        public void Subscribe();

        // removes Subscriber to subscription
        public void UnSubscribe();


    }
}