// Created By   :   Isaac Bustad
// Created      :   11/2/2026


using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public struct OrientationSnapshot
    {
        public OrientationData startState;
        public OrientationData endState;
        //public Vector3 startVelocity;
        //public Vector3 endVelocity;
        public float duration; // Simple duration to lerp/apply over



        public OrientationSnapshot(
            OrientationData aStartState,
            OrientationData aEndState,
            float aDuration)

        {
            startState = aStartState;
            endState = aEndState;
            duration = aDuration;
        }

        public float Duration => duration;
    }
}
