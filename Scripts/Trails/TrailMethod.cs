// Created by   :   Isaac Bustad   
// Created      :   4/26/2026

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BugFreeProductions.Tools
{
    public enum TrailMethodType {standard,drift,boost}



    [CreateAssetMenu(fileName = "TrailMethod", menuName = "ScriptableObject/TrailMethod")]
    public class TrailMethod : ScriptableObject
    {
        #region Methods
        public void RenderTrail(List<Vector3> positions, LineRenderer lineRenderer)
        {    
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }

        // Calculates for multiple trails
        public void RenderTrail(List<Vector3> aPos1, LineRenderer aLR1, List<Vector3> aPos2, LineRenderer aLR2)
        {    
            aLR1.positionCount = aPos1.Count;
            aLR1.SetPositions(aPos1.ToArray());

            aLR2.positionCount = aPos2.Count;
            aLR2.SetPositions(aPos2.ToArray());            
        }

        // calculates via type
        // public void RenderTrail(List<Vector3> aPos1, LineRenderer aLR1, List<Vector3> aPos2, LineRenderer aLR2)
        // {    
                
        // }

        // Calculates for multiple trails with Drift
        public void RenderTrail(List<Vector3> aPos1, LineRenderer aLR1, List<Vector3> aPos2, LineRenderer aLR2, float aDrift)
        {    
            // Ensure aPos1 and aPos2 are the same length
            if (aPos1.Count != aPos2.Count)
            {
                int minLength = Math.Min(aPos1.Count, aPos2.Count);
                aPos1 = aPos1.GetRange(0, minLength);
                aPos2 = aPos2.GetRange(0, minLength);
            }

            // drift the paired points in opposite directions
            // Declare new list of points
            List<Vector3> nPos1 = new List<Vector3>();
            List<Vector3> nPos2 = new List<Vector3>();

            // to - from - to + drifts the points apart
            for(int i = 0; i < aPos1.Count; i++)
            {
                Vector3 driftDir = Vector3.Normalize(aPos2[2] - aPos1[i]);

                // calculate a new position for drifted from the initial positions
                if(i == 0)
                {
                    nPos1.Add(aPos1[i]);
                    nPos2.Add(aPos2[i]);
                }
                
                else
                {
                    nPos1.Add(aPos1[i] - (driftDir*i*aDrift/100));
                    nPos2.Add(aPos2[i] + (driftDir*i*aDrift/100));
                }

                

                //

            }


            // set the positions of the line renderers
            aLR1.positionCount = nPos1.Count;
            aLR1.SetPositions(nPos1.ToArray());

            aLR2.positionCount = nPos2.Count;
            aLR2.SetPositions(nPos2.ToArray());            
        }


        #endregion Methods
    }
}
