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
        public void OnNotify(SubMessage aSubMessage);

        // adds Subscriber to subscription
        protected void AddSubscription();

        // removes Subscriber to subscription
        protected void RemoveSubscription();


    }
}