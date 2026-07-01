// Created By   :   Isaac Bustad
// Created      :   6/22/2026

using System;
using System.Collections.Generic;
using UnityEngine;


namespace BugFreeProductions.Tools
{
        
    public class MultipleOrientationTrailSystem : MonoBehaviour, ISubscription
    {
        #region Vars
        // class assigned data
        // notification delegate
        protected Action<ISubscriberNotification> onNotify;

        // notification data
        protected ISubscriberNotification subscriberNotification;
            


        [SerializeField] protected TrailMethod trailMethod;
        [Header("Parameters to draw line")]
        [SerializeField] protected List<LineRenderer> lineRenderers;
        [SerializeField] protected List<OrientationDirection> lineDirections = new List<OrientationDirection>();
        [SerializeField] protected int maxNumberOfPoints = 100;
        [SerializeField] protected float pointDelay = 0.1f;
        [SerializeField] protected float timeToLastPoint = 0f;

        [SerializeField] protected float lineDrift = 0f;
        [SerializeField] protected float trailOffset = 0f;

        // List of all the current trail points
        List<List<Vector3>> allPos = new List<List<Vector3>>();

        // tracked locations
        // [SerializeField] protected List<Transform> locs = new List<Transform>();




        
        

        [Header("Orientation trail vars")]
        protected List<OrientationData> orientationDatas = new List<OrientationData>();
        [SerializeField] protected Vector3 vOffset = Vector3.zero; 


        #region BoostRingVars
        [Header("Trail Item Spawner")]
        [SerializeField] protected SubscribingFactory_SCO trailItemFactory = null;
        [SerializeField] protected float itemPointDelay = 0.5f;
        [SerializeField] protected float timeToLastItem = 0f;

        #endregion BoostRingVars


        #endregion Vars




        #region Methods
        #region Unity Methods
        protected virtual void OnEnable()
        {

        }

        protected virtual void Update()
        {
            DelayPoint(Time.deltaTime);
            
            if (trailMethod != null)
            {
                RenderTrail();
            }


        }

        #endregion Unity Methods

        protected virtual void AddPoint()
        {
            //TwoPointsTrail();

            OrientationData od = new OrientationData(transform.position,transform.rotation);
            orientationDatas.Add(od);

            while(orientationDatas.Count > maxNumberOfPoints)
            {
                orientationDatas.RemoveAt(0);
            }
        }

        protected virtual void RenderTrail()
        {
            // 
            trailMethod.RenderTrail(lineRenderers, orientationDatas, lineDirections, lineDrift, trailOffset, vOffset);
        }

        protected virtual void DelayPoint(float atime)
        {
            timeToLastPoint += atime;

            if (timeToLastPoint > pointDelay)
            {
                AddPoint();

                timeToLastPoint = 0;
            }
        }

        #region Trail Item Spawning
        protected virtual void DelayItemSpawn(float aTime)
        {
            // add to time since last spawn
            timeToLastItem += aTime;

            // Call and reset
            // OrientationData aod = orientationDatas[orientationDatas.Count - 1];
            trailItemFactory.CreateItem(orientationDatas[orientationDatas.Count - 1], this);

        }
        #endregion Trail Item Spawning

        #region ImplementISubscription
        public void Subscribe(ISubscriber subscriber)
        {
            onNotify += subscriber.Notify;
        }

        public void Notify()
        {
            onNotify.Invoke(subscriberNotification);
        }

        public void UnSubscribe(ISubscriber subscriber)
        {
            onNotify -= subscriber.Notify;
        }

        #endregion ImplementISubScription
        
        #endregion Methods
    }
}