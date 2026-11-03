// Created By   :   Isaac Bustad
// Created      :   11/2/2026

using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public class OrientationReconciliation : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField, Tooltip("The physical root transform this visual object follows.")]
        private Transform physicalBodyTarget;
        
        [Header("Default Timing")]
        [SerializeField] private float defaultTickDuration = 0.05f; 

        [Header("Stationary Filtering")]
        [SerializeField, Tooltip("Threshold to filter out sub-millimeter physics noise from entering the snapshot queue.")]
        private float noiseTolerance = 0.001f; // 1 millimeter tolerance

        [SerializeField, Tooltip("Smoothing damping factor when settling into idle position.")]
        private float idleDamping = 25f;

        // FIFO queue matching the VisualSplineUpdater pattern
        private LinkedList<OrientationSnapshot> snapshotBuffer = new LinkedList<OrientationSnapshot>();
        
        private OrientationSnapshot? activeSnapshot;
        private float elapsedSnapshotTime;

        // Track velocities to maintain C^1 continuity across segments
        private Vector3 lastTargetVelocity = Vector3.zero;

        private void Awake()
        {
            if (physicalBodyTarget == null)
            {
                physicalBodyTarget = transform.root;
            }
        }

        private void Start()
        {
            if (physicalBodyTarget != null)
            {
                transform.position = physicalBodyTarget.position;
                transform.rotation = physicalBodyTarget.rotation;
            }
        }

        /// <summary>
        /// Pushes a new absolute orientation state into the smoothing buffer.
        /// </summary>
        public void PushOrientationState(OrientationData targetState, Vector3 targetVelocity, float customDuration = -1f)
        {
            // ROBUST NOISE FILTER: Compare against the tail of the queue or current transform/start
            OrientationData referenceState = snapshotBuffer.Last != null ? snapshotBuffer.Last.Value.endState : new OrientationData(transform.position, transform.rotation);

            float posDeltaSq = (referenceState.positionData - targetState.positionData).sqrMagnitude;
            float angleDelta = Quaternion.Angle(referenceState.rotationData, targetState.rotationData);

            // If the incoming state is within the noise threshold and velocity is near zero, discard it entirely!
            if (posDeltaSq < (noiseTolerance * noiseTolerance) && angleDelta < 0.5f && targetVelocity.sqrMagnitude < 0.001f)
            {
                return; 
            }

            OrientationData startState;
            Vector3 startVelocity = lastTargetVelocity;

            // Chain smoothly from the tail of the queue, active snapshot, or current visual position
            if (snapshotBuffer.Last != null)
            {
                startState = snapshotBuffer.Last.Value.endState;
            }
            else if (activeSnapshot.HasValue)
            {
                startState = activeSnapshot.Value.endState;
            }
            else
            {
                startState = new OrientationData(transform.position, transform.rotation);
            }

            float segmentDuration = customDuration > 0f ? customDuration : defaultTickDuration;
            float currentTime = Time.time;
            float endTime = currentTime + segmentDuration;

            OrientationSnapshot snapshot = new OrientationSnapshot(
                startState,
                targetState,
                startVelocity,
                targetVelocity,
                currentTime,
                endTime
            );

            snapshotBuffer.AddLast(snapshot);
            lastTargetVelocity = targetVelocity;
        }

        private void FixedUpdate()
        {
            if (physicalBodyTarget == null) return;

            // 1. Grab next snapshot from the FIFO queue if none is active
            if (!activeSnapshot.HasValue && snapshotBuffer.Count > 0)
            {
                activeSnapshot = snapshotBuffer.First.Value;
                snapshotBuffer.RemoveFirst();
                elapsedSnapshotTime = 0f;
            }

            // 2. Evaluate active snapshot using smooth spline (position) and slerp (rotation)
            if (activeSnapshot.HasValue)
            {
                elapsedSnapshotTime += Time.fixedDeltaTime;
                float duration = activeSnapshot.Value.Duration;
                
                float normalizedAlpha = duration > 0f ? Mathf.Clamp01(elapsedSnapshotTime / duration) : 1.0f;

                Vector3 interpolatedPosition = EvaluatePosition(activeSnapshot.Value, normalizedAlpha);
                Quaternion interpolatedRotation = EvaluateRotation(activeSnapshot.Value, normalizedAlpha);

                if (normalizedAlpha >= 1.0f)
                {
                    interpolatedPosition = activeSnapshot.Value.endState.positionData;
                    interpolatedRotation = activeSnapshot.Value.endState.rotationData;
                    activeSnapshot = null;
                }

                transform.position = interpolatedPosition;
                transform.rotation = interpolatedRotation;
            }
            // 3. Fallback: Smooth exponential decay toward physical target to eliminate solver noise without snapping
            // else
            // {
            //     float blendFactor = 1f - Mathf.Exp(-idleDamping * Time.fixedDeltaTime);

            //     transform.position = Vector3.Lerp(transform.position, physicalBodyTarget.position, blendFactor);
            //     transform.rotation = Quaternion.Slerp(transform.rotation, physicalBodyTarget.rotation, blendFactor);

            //     lastTargetVelocity = Vector3.zero;
            // }
        }

        public void Clear()
        {
            snapshotBuffer.Clear();
            activeSnapshot = null;
            elapsedSnapshotTime = 0f;
            lastTargetVelocity = Vector3.zero;
        }

        private Vector3 EvaluatePosition(OrientationSnapshot snapshot, float t)
        {
            Vector3SplineSnapshot splineSnapshot = new Vector3SplineSnapshot(
                snapshot.startState.positionData,
                snapshot.startVelocity,
                snapshot.endState.positionData,
                snapshot.endVelocity,
                snapshot.Duration
            );

            return Vector3SplineSolver.Evaluate(splineSnapshot, t);
        }

        private Quaternion EvaluateRotation(OrientationSnapshot snapshot, float t)
        {
            return RotationReconciliation.ShortestPathSlerp(
                snapshot.startState.rotationData,
                snapshot.endState.rotationData,
                Mathf.Clamp01(t)
            );
        }
    }
}