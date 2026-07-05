// Created By   :   Isaac Bustad
// Created      :   7/4/2026

using UnityEngine;

namespace BugFreeProductions.Tools
{
    public class TrailItemSpawner : MonoBehaviour, ISubscriber, ISubscription
    {
        #region Vars
        protected MultipleOrientationTrailSystem trailSystem = null;

        [Header("Trail Item Spawner")]
        [SerializeField] protected SubscribingFactory_SCO trailItemFactory = null;
        [SerializeField] protected float itemPointDelay = 0.5f;
        [SerializeField] protected float timeToLastItem = 0f;



        


        #endregion Vars


        #region Methods

        #region Unity Methods
        protected virtual void OnEnable()
        {
            Setup();
            
        }

        protected virtual void Setup()
        {
            // subscribe to trail system
            if (GetComponent<MultipleOrientationTrailSystem>() is MultipleOrientationTrailSystem aTrailSystem)
            {
                trailSystem = aTrailSystem;
                trailSystem.Subscribe(this);
            }
        }
        

        #endregion Unity Methods

        #region ISubscriber methods
        
        public void OnSubscribe()
        {

        }

        public void OnUnSubscribe()
        {

        }
        public void OnNotify(ISubscriberNotification aSubMessage)
        {
            //DelayItemSpawn();
        }
        #endregion ISubscriber methods

        #region Implement ISubscription
        public void Subscribe(ISubscriber subscriber)
        {

        }
         // adds Subscriber to subscription
        public void OnSubscribe(ISubscription aSubscription)
        {
            
        }

        public void UnSubscribe(ISubscriber subscriber)
        {

        }
        public void Notify()
        {

        }

        #endregion Implement ISubscription
        
        #region Trail Item Spawning
        protected virtual void DelayItemSpawn(OrientationData aOrientationData)
        {
            
            
            

        }


        #endregion Trail Item Spawning

        #endregion Methods

    }
}