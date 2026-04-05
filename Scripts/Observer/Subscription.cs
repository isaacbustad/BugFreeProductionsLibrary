// Isaac Bustad
// 5/6/2025


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Tools
{
    public interface Subscription
    {
        // Vars
        // deligate that calls the notify method on subscribers
        public delegate void notifySubscriber();
    }
}