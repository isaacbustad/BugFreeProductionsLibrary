// Created By   :   Isaac Bustad
// Created      :   10/6/2026

using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    /// <summary>
    /// Manages the snapshot interpolation buffer, coordinates position splines, 
    /// and delegates rotation smoothing to specialized evaluation methods.
    /// </summary>
    public class OrientationReconciliation
    {
        #region Fields
        private readonly LinkedList<OrientationSnapshot> snapshotBuffer;
        private readonly int maxBufferSize;
        private readonly float interpolationDelay;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the OrientationReconciliation manager.
        /// </summary>
        public OrientationReconciliation(int bufferSize = 20, float delay = 0.1f)
        {
            snapshotBuffer = new LinkedList<OrientationSnapshot>();
            maxBufferSize = bufferSize;
            interpolationDelay = delay;
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Adds a new orientation snapshot to the interpolation buffer, enforcing capacity limits.
        /// </summary>
        public void AddSnapshot(OrientationSnapshot snapshot)
        {
            snapshotBuffer.AddLast(snapshot);

            if (snapshotBuffer.Count > maxBufferSize)
            {
                snapshotBuffer.RemoveFirst();
            }
        }

        /// <summary>
        /// Clears all stored snapshots from the buffer.
        /// </summary>
        public void Clear()
        {
            snapshotBuffer.Clear();
        }

        /// <summary>
        /// Evaluates the interpolated orientation state at the given render time.
        /// </summary>
        public bool Evaluate(float currentTime, out OrientationData result)
        {
            result = default(OrientationData);

            if (snapshotBuffer.Count < 2)
            {
                if (snapshotBuffer.Count == 1)
                {
                    result = snapshotBuffer.First.Value.endState;
                    return true;
                }
                return false;
            }

            float targetTime = currentTime - interpolationDelay;

            // Prune old snapshots that are completely behind the target render window
            while (snapshotBuffer.Count > 2 && snapshotBuffer.First.Next.Value.endTime <= targetTime)
            {
                snapshotBuffer.RemoveFirst();
            }

            OrientationSnapshot currentSnapshot = snapshotBuffer.First.Value;
            OrientationSnapshot nextSnapshot = snapshotBuffer.First.Next.Value;

            // If target time is ahead of our buffer, clamp to the newest available state
            if (targetTime >= nextSnapshot.endTime)
            {
                result = nextSnapshot.endState;
                return true;
            }

            // Calculate duration and normalized time (t)
            float duration = nextSnapshot.startTime - currentSnapshot.startTime;
            if (duration <= 0.0001f)
            {
                result = nextSnapshot.startState;
                return true;
            }

            float t = (targetTime - currentSnapshot.startTime) / duration;

            // Delegate position and rotation calculations to specialized sub-methods
            Vector3 interpolatedPosition = EvaluatePosition(currentSnapshot, nextSnapshot, t, duration);
            Quaternion interpolatedRotation = EvaluateRotation(currentSnapshot, nextSnapshot, t);

            result = new OrientationData(interpolatedPosition, interpolatedRotation);
            return true;
        }

        /// <summary>
        /// Evaluates position interpolation using the Vector3SplineSolver framework.
        /// </summary>
        private Vector3 EvaluatePosition(OrientationSnapshot current, OrientationSnapshot next, float t, float duration)
        {
            Vector3SplineSnapshot splineSnapshot = new Vector3SplineSnapshot(
                current.startState.positionData,
                current.endVelocity,
                next.startState.positionData,
                next.startVelocity,
                duration
            );

            return Vector3SplineSolver.Evaluate(splineSnapshot, t);
        }

        /// <summary>
        /// Evaluates rotation interpolation using the RotationReconciliation shortest-path slerp.
        /// </summary>
        private Quaternion EvaluateRotation(OrientationSnapshot current, OrientationSnapshot next, float t)
        {
            return RotationReconciliation.ShortestPathSlerp(
                current.startState.rotationData,
                next.startState.rotationData,
                Mathf.Clamp01(t)
            );
        }
        #endregion Methods
    }
}