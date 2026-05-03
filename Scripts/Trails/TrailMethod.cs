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
        #region Vars
        [SerializeField] protected int maxNumberOfPoints = 100;
        [SerializeField] protected float pointDelay = 0.1f;
        [SerializeField] protected float timeToLastPoint = 0f;

        

        #endregion Vars

        #region Methods
        






        #region TrailRendering
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
                Vector3 driftDir = Vector3.Normalize(aPos2[i] - aPos1[i]);

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

        // public void RenderTrail(List<Vector3> aPos1, LineRenderer aLR1, Vector3 aLoc1, List<Vector3> aPos2, LineRenderer aLR2,  Vector3 aLoc2, float aDrift)
        // {
        //     // record the points


        //     // render the trail
        //     RenderTrail(aPos1, aLR1, aPos2, aLR2, aDrift);
        // }

        // take in a list of line renderers and lists of positions
        public void RenderTrail(List<List<Vector3>> aAllPos, List<LineRenderer> aAllLRs)
        {
            // store new lists of positions
            List<List<Vector3>> nAllPos = CalculateNewListOfPositions(aAllPos);

            // Draw the Lines nPos1.Add(aPos1[i] - (driftDir*i*aDrift/100));
            for (int l = 0; l < nAllPos.Count; l++)
            {
                // create available line renderer indexes
                aAllLRs[l].positionCount = nAllPos[l].Count();

                // assign positions to line renderer indexes
                aAllLRs[l].SetPositions(nAllPos[l].ToArray());

            }

        }


        // take in a list of line renderers and lists of positions with drift
        public void RenderTrail(List<List<Vector3>> aAllPos, List<LineRenderer> aAllLRs, float aDrift)
        {
            // store new lists of positions
            List<List<Vector3>> nAllPos = CalculateNewListOfPositions(aAllPos);

            // find the midpoint
            int length = nAllPos[0].Count;
            int half = length / 2;
            int remainder = length % 2;


            if (remainder == 0)
            {
                // Draw the Lines nPos1.Add(aPos1[i] - (driftDir*i*aDrift/100));
                for (int l = 0; l < half; l++)
                {
                    // first pos list
                    List<Vector3> nFirstPosL = new List<Vector3>();


                    // second pos list
                    List<Vector3> nSecondPosL = new List<Vector3>();


                    for(int i = 0; i < nAllPos[l].Count(); i++)
                    {
                        // if it is the first point first point do not drift
                        if (i == 0)
                        {
                            nFirstPosL.Add(nAllPos[l][i]);

                            nSecondPosL.Add(nAllPos[nAllPos[l].Count()-1-l][i]);
                        }

                        else
                        {
                            // drift direction
                            // get general direction
                            Vector3 aDriftMod = nAllPos[nAllPos[l].Count()-1-l][i] - nAllPos[l][i];

                            // normalize
                            aDriftMod.Normalize();

                            // find the percentage through the list
                            float percentage = l / nAllPos[l].Count();

                            // multiply by the percentage through the list
                            aDriftMod = aDriftMod * percentage * aDrift;

                            
                            nFirstPosL.Add(nAllPos[l][i] + aDriftMod);

                            nSecondPosL.Add(nAllPos[nAllPos[l].Count()-1-l][i] - aDriftMod);
                        }
                    }

                    


                    // create available line renderer indexes
                    aAllLRs[l].positionCount = nFirstPosL.Count();

                    // assign positions to line renderer indexes
                    aAllLRs[l].SetPositions(nFirstPosL.ToArray());

                    // opposite line

                    // create available line renderer indexes
                    aAllLRs[nAllPos[l].Count()-1-l].positionCount = nSecondPosL.Count();

                    // assign positions to line renderer indexes for opposite line
                    aAllLRs[nAllPos[l].Count()-1-l].SetPositions(nSecondPosL.ToArray());







                    

                } 
            }

            else
            {
                // Draw the Lines nPos1.Add(aPos1[i] - (driftDir*i*aDrift/100));
                for (int l = 0; l < half; l++)
                {
                    // create available line renderer indexes
                    aAllLRs[l].positionCount = nAllPos[l].Count();

                    // assign positions to line renderer indexes
                    aAllLRs[l].SetPositions(nAllPos[l].ToArray());

                    // opposite line

                    // create available line renderer indexes
                    aAllLRs[nAllPos[l].Count()-1-l].positionCount = nAllPos[nAllPos[l].Count()-1-l].Count();

                    // assign positions to line renderer indexes for opposite line
                    aAllLRs[nAllPos[l].Count()-1-l].SetPositions(nAllPos[nAllPos[l].Count()-1-l].ToArray());

                } 
            }

            

            

        }

        #endregion TrailRendering




        #region Trail Calculating
        protected virtual int CalculateMinCount(List<List<Vector3>> aAllPos)
        {
            // store the minimum
            int minCount = int.MaxValue;

            // loop and find the minimum
            
            for (int l =0; l < aAllPos.Count; l++)
            {
                if (aAllPos[l].Count < minCount)
                {
                    minCount = aAllPos[l].Count;
                }
            }

            // return the minimum
            return minCount;
        }


        protected virtual List<List<Vector3>> CalculateNewListOfPositions(List<List<Vector3>> aAllPos)
        {
            // store the minimum
            int minCount = CalculateMinCount(aAllPos);

            // store new lists of positions
            List<List<Vector3>> nAllPos = new List<List<Vector3>>();

            
            // loop and clamp the V3 list leangths
            for (int l = 0; l < aAllPos.Count; l++)
            {
                // construct new clamped list
                List<Vector3> nPos = aAllPos[l].GetRange(0, minCount);

                // add clamped list to new all list
                nAllPos.Add(nPos);
            }

            
            // return new list of positions
            return nAllPos;
        }




        #endregion Trail Calculating




        #endregion Methods


        #region Accessors
        
        #endregion Accessors
    }
}
