// Created By   :   Isaac Bustad
// Created      :   4/1/2026



using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace BugFreeProductions.Tools
{

    public class MultipleTrailSystem : MonoBehaviour
    {
        #region Vars
        [SerializeField] protected bool isLineTrail;


        [SerializeField] protected TrailMethod trailMethod;
        [SerializeField] protected List<LineRenderer> lineRenderers;
        [SerializeField] protected int maxNumberOfPoints = 100;
        [SerializeField] protected float pointDelay = 0.1f;
        [SerializeField] protected float timeToLastPoint = 0f;

        [SerializeField] protected float lineDrift = 0f;

        // List of all the current trail points
        List<List<Vector3>> allPos = new List<List<Vector3>>();

        // tracked locations
        [SerializeField] protected List<Transform> Locs = new List<Transform>();

        // lists by location
        protected Dictionary<int, List<Vector3>> trailsByLocation = new Dictionary<int, List<Vector3>>();
        #endregion Vars

        #region Methods
        protected virtual void OnEnable()
        {
            for (int i = 0; i < trailsByLocation.Count; i++)
            {
                trailsByLocation[i].Add(Locs[i].position);
                
            }


        }
        
        protected virtual void Update()
        {
            DelayPoint(Time.deltaTime);
            if (trailMethod != null)
            {
                RenderTrail();
            }
        }

        protected virtual void DelayPoint(float atime)
        {
            timeToLastPoint += pointDelay;

            if (timeToLastPoint > pointDelay)
            {
                AddPoint();

                timeToLastPoint = 0;
            }
        }

        protected virtual void AddPoint()
        {
            // hold a list of all position list
            allPos = new List<List<Vector3>>();

            foreach (KeyValuePair<int, List<Vector3>> aKVP in trailsByLocation)
            {
                // add a single position
                aKVP.Value.Add(Locs[aKVP.Key].position);

                // update all positions
                allPos.Add(aKVP.Value) ;
                
            }

            

            // trailPoints2.Add()
            // if (trailPoints1.Count > maxNumberOfPoints)
            // {
            //     for (int i = 0; i<= trailPoints1.Count - maxNumberOfPoints; i++)
            //     {
            //         trailPoints1.RemoveAt(0);
            //     }
            // }

            
        }

        protected virtual void RenderTrail()
        {
            trailMethod.RenderTrail(allPos, lineRenderers,lineDrift);
        }




        #endregion Methods
    }
}