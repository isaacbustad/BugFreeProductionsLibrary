// Created By   :   Isaac Bustad
// Created      :   6/21/2026

using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;


namespace BugFreeProductions.Tools
{
    // Make a singleton
    public class TrailerSystemManager : MonoBehaviour, ISubscription
    {
        #region Vars
        // hold a list of subscribers
        protected List<ISubscriber> subscribers = new List<ISubscriber>();

        // holds a list of managed Trail Sys 
        protected List<MultipleTrailSystem> trailSystems = new List<MultipleTrailSystem>();

        // subMessage reference
        protected SubMessage subMessage;

        // Vars from inherited class
        public Action<SubMessage> notifySubscriber;
        #endregion Vars

        #region Methods

        #region UnityMethods
        protected virtual void OnEnable()
        {
            Setup();
        }
        #endregion UnityMethods

        // Setup References on start
        protected virtual void Setup()
        {
            subMessage = new SubMessage(this);
        }
        // Manage Trail Systems


        #region ISubscription
        public void Subscribe(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
            notifySubscriber += subscriber.OnNotify;
        }

        public void NotifySubscribers()
        {
            notifySubscriber(subMessage);
        }

        #endregion ISubscription


        #endregion Methods

        #region Accessors

        #endregion Accessors

    }
}