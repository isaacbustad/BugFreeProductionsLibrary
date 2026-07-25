// Created By   :   Isaac Bustad
// Created      :   7/24/2026



using BugFreeProductions.Spatial;
using System.Collections.Generic;
using UnityEngine;





namespace BugFreeProductions.Tools
{
    public class TargetingSystem : GenericSingleton<TargetingSystem>
    {
        #region Vars
        protected List<TargetNode> targetNodes = new List<TargetNode>();
        
        #endregion Vars



        #region Methods
        #region Unity Methods
        public virtual List<TargetData> FindTarget( TargetNode ignoreNode, 
                                                    TargetingSystemParams_SCO targetingSystemParams_SCO, 
                                                    OrientationData aOrientationData)
        {
            // construct a node 
            List<TargetData> targetingData = new List<TargetData>();

            // collect the targeting path Values
            List<Vector3> targetingPath = CalculateTargetingPath(targetingSystemParams_SCO.TargetingRange,
                                                                targetingSystemParams_SCO.TargetingInterval,
                                                                aOrientationData);
            
            // loop through each point for each possible target
            foreach (TargetNode tn in targetNodes)
            {                
                if (tn.GetInstanceID() != ignoreNode.GetInstanceID())
                {
                    // create a Targetdata Node list 
                    // select the one with the minimum distance
                    //CalculateClosestToPath(targetingPath,tn);
                }
            }

            return targetingData;
        }

        #endregion Unity Methods

        #region Targeting Methods
        protected virtual List<Vector3> CalculateTargetingPath(float aDistance, 
                                                                float aInterval, 
                                                                OrientationData aOrientationData)
        {
            // hold the return Vector3 Positions
            List<Vector3> targetingPath = new List<Vector3>();

            // count the loops through
            // include in exit condition
            int intervalStep = 1;

            while (aInterval * intervalStep < aDistance)
            {
                // new ofset by interval
                Vector3 ofsetAtInterval = aOrientationData.positionData + 
                                            (aOrientationData.Direction(OrientationDirection.forward) * intervalStep);

                // add the calculated point to the path


                // increment to calculation
                // increment for end condition
                intervalStep ++;
            }

            // return the constructed path
            return targetingPath;
        }


        // protected virtual TargetData CalculateClosestToPath(List<Vector3> aTargPath, TargetNode aTargetNode)
        // {
        //     // calculate distance to all path nodes
        // }

        protected virtual List<TargetNode> FilterTargetNodes(List<TargetNode> aTargetNodes)
        {
            // store list of valid targets
            List<TargetNode> validTargets = new List<TargetNode>();

            //BugFreeTool
            return validTargets;
        }

        #endregion Targeting Methods


        // #region Implement ISubscription
        // public void Subscribe(ISubscriber subscriber)
        // {
        //     if(subscriber is TargetingNode aTargetingNode)
        //     {
        //         targetNodes.Add(aTargetingNode.TargetNode);
        //     }

        // }

        // public void UnSubscribe(ISubscriber subscriber)
        // {
        //     if(subscriber is TargetingNode aTargetingNode)
        //     {
        //         targetNodes.Remove(aTargetingNode.TargetNode);
        //     }
        // }
        // public void Notify()
        // {

        // }
        // #endregion Implement ISubscription

        #endregion Methods



        #region Constructors
        
        #endregion Constructors



        #region Accessors

        #endregion Accessors
    }
}