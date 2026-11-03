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
        [SerializeField, Tooltip("Deadzone threshold to filter out sub-millimeter physics solver noise when held still.")]
        private float stationaryThreshold = 0.0005f;

        // FIFO queue matching the VisualSplineUpdater pattern
        private LinkedList<OrientationSnapshot> snapshotBuffer = new LinkedList<OrientationSnapshot>();
        
        private OrientationSnapshot? activeSnapshot;
        private float elapsedSnapshotTime;

        // Track velocities to maintain C^1 continuity across segments
        private Vector3 lastTargetVelocity = Vector3.zero;

        // Latch fields to eliminate stationary physics jitter
        private Vector3 lastStablePosition;
        private Quaternion lastStableRotation;
        private bool isLatched = false;

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
                lastStablePosition = transform.position;
                lastStableRotation = transform.rotation;
            }
        }

        /// <summary>
        /// Pushes a new absolute orientation state into the smoothing buffer.
        /// </summary>
        public void PushOrientationState(OrientationData targetState, Vector3 targetVelocity, float customDuration = -1f)
        {
            // FIX: If the new target matches the last queued snapshot and velocity is zero, 
            // skip adding a redundant segment to prevent spline micro-stutter.
            if (snapshotBuffer.Last != null)
            {
                OrientationData lastEndState = snapshotBuffer.Last.Value.endState;
                bool isSamePosition = (lastEndState.positionData - targetState.positionData).sqrMagnitude < 0.00001f;
                bool isSameRotation = Quaternion.Angle(lastEndState.rotationData, targetState.rotationData) < 0.01f;

                if (isSamePosition && isSameRotation && targetVelocity.sqrMagnitude < 0.0001f)
                {
                    return; // Discard redundant duplicate packet
                }
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
                isLatched = false; // Unlatch when a new movement segment starts
            }

            // 2. Evaluate active snapshot using smooth spline (position) and slerp (rotation)
            if (activeSnapshot.HasValue)
            {
                isLatched = false;
                elapsedSnapshotTime += Time.fixedDeltaTime;
                float duration = activeSnapshot.Value.Duration;
                
                float normalizedAlpha = duration > 0f ? Mathf.Clamp01(elapsedSnapshotTime / duration) : 1.0f;

                // Evaluate Position via Vector3SplineSolver
                Vector3 interpolatedPosition = EvaluatePosition(activeSnapshot.Value, normalizedAlpha);
                
                // Evaluate Rotation via ShortestPathSlerp
                Quaternion interpolatedRotation = EvaluateRotation(activeSnapshot.Value, normalizedAlpha);

                // If we hit or exceed the end of the snapshot, snap cleanly to avoid overshoots
                if (normalizedAlpha >= 1.0f)
                {
                    interpolatedPosition = activeSnapshot.Value.endState.positionData;
                    interpolatedRotation = activeSnapshot.Value.endState.rotationData;
                    activeSnapshot = null;
                }

                transform.position = interpolatedPosition;
                transform.rotation = interpolatedRotation;

                lastStablePosition = interpolatedPosition;
                lastStableRotation = interpolatedRotation;
            }
            // 3. Fallback: When buffer is empty, apply stationary deadzone latch to ignore PhysX solver noise
            else
            {
                float sqrDist = (physicalBodyTarget.position - lastStablePosition).sqrMagnitude;
                float angleDiff = Quaternion.Angle(transform.rotation, physicalBodyTarget.rotation);

                if (sqrDist > stationaryThreshold * stationaryThreshold || angleDiff > 0.1f || !isLatched)
                {
                    transform.position = physicalBodyTarget.position;
                    transform.rotation = physicalBodyTarget.rotation;

                    lastStablePosition = transform.position;
                    lastStableRotation = transform.rotation;
                    isLatched = true;
                }
                else
                {
                    // Lock transform completely still against micro-wiggles
                    transform.position = lastStablePosition;
                    transform.rotation = lastStableRotation;
                }

                lastTargetVelocity = Vector3.zero;
            }
        }

        public void Clear()
        {
            snapshotBuffer.Clear();
            activeSnapshot = null;
            elapsedSnapshotTime = 0f;
            lastTargetVelocity = Vector3.zero;
            isLatched = false;
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