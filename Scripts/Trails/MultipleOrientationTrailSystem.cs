// Created By   :   Isaac Bustad
// Created      :   6/22/2026

using System.Collections.Generic;
using UnityEngine;


namespace BugFreeProductions.Tools
{
        
    public class MultipleOrientationTrailSystem : MonoBehaviour
    {
        #region Vars
            


        [SerializeField] protected TrailMethod trailMethod;
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
        [SerializeField] protected List<Transform> locs = new List<Transform>();




        
        

        [Header("Orientation trail vars")]
        protected List<OrientationData> orientationDatas = new List<OrientationData>();
        [SerializeField] protected Vector3 vOffset = Vector3.zero; 
        #endregion Vars

        #region Methods
        protected virtual void Update()
        {
            DelayPoint(Time.deltaTime);
            
            if (trailMethod != null)
            {
                RenderTrail();
            }


        }
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
            
            //List<List(Vector3)>
            //trailMethod.RenderTrail(allPos, lineRenderers,centerLocs,lineDrift,trailOffset);
            trailMethod.RenderTrail(2, lineRenderers, orientationDatas, lineDirections, lineDrift, trailOffset, vOffset);
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
        
        #endregion Methods
    }
}