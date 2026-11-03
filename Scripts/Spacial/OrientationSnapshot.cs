// Created By   :   Isaac Bustad
// Created      :   11/2/2026


using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public struct OrientationSnapshot
    {
        public OrientationData startState;
        public OrientationData endState;
        public Vector3 startVelocity;
        public Vector3 endVelocity;
        public float startTime; // Assigned locally when the start packet was processed
        public float endTime;   // Assigned locally when the end packet was processed

        public OrientationSnapshot(
            OrientationData aStartState, 
            OrientationData aEndState, 
            Vector3 aStartVelocity, 
            Vector3 aEndVelocity, 
            float aStartTime, 
            float aEndTime)
        {
            startState = aStartState;
            endState = aEndState;
            startVelocity = aStartVelocity;
            endVelocity = aEndVelocity;
            startTime = aStartTime;
            // Ensure endTime can never precede or match startTime
            endTime = Mathf.Max(aEndTime, startTime + 0.01f);
        }

        public float Duration => endTime - startTime;
    }
}