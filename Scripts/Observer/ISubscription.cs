// Isaac Bustad
// 5/6/2025


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public interface ISubscription
    {
        // Vars
        // deligate that calls the notify method on subscribers
        //public delegate void notifySubscriber(SubMessage aMessage);
        // public Action<SubMessage> notifySubscriber;

        // Methods
        public void Subscribe(ISubscriber subscriber) { Debug.Log("impliment Subscribe Method"); }

        public void NotifySubscribers();
    }
}