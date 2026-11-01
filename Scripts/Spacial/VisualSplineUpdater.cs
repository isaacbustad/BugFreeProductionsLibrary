// Created By   :   Isaac Bustad
// Created      :   10/31/2026

using System.Collections.Generic;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public class VisualSplineUpdater : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private Transform physicalBodyTarget;
        
        [Header("Default Timing")]
        [SerializeField] private float defaultTickDuration = 0.05f; 

        // LinkedList maintains First-In, First-Out (FIFO) queue behavior while giving us 
        // instant O(1) access to the tail (.Last) and head (.First) without array-shifting overhead.
        private LinkedList<Vector3SplineSnapshot> snapshotList = new LinkedList<Vector3SplineSnapshot>();
        
        // NOTE ON SYNTAX ('?'): 
        // The '?' after Vector3SplineSnapshot makes this a "Nullable Value Type". 
        // Normally, C# structs (like your snapshot struct) cannot be null—they default to zero-filled data. 
        // Adding the '?' allows this variable to explicitly hold a 'null' state so we know when 
        // no snapshot is currently being played.
        private Vector3SplineSnapshot? activeSnapshot;
        
        private float elapsedSnapshotTime;

        // Track velocity to maintain C^1 continuity across spline segments
        private Vector3 lastTargetVelocity = Vector3.zero;

        private void Awake()
        {
            if (physicalBodyTarget == null)
            {
                Debug.LogWarning($"[VisualSplineUpdater] No physicalBodyTarget assigned on {gameObject.name}.", this);
            }
        }

        private void Start()
        {
            if (physicalBodyTarget != null)
            {
                transform.position = physicalBodyTarget.position;
            }
        }

        /// <summary>
        /// Pushes a new state with an optional custom duration (defaults to tick duration if omitted).
        /// </summary>
        public void PushPhysicalState(Vector3 targetPosition, Vector3 targetVelocity, float customDuration = -1f)
        {
            Vector3 startPosition;

            // Determine the starting position using a strict priority hierarchy (no boolean flags required):
            // 1. If there are snapshots waiting in the list, chain off the P3 (end point) of the very last item.
            // 2. Otherwise, if an active snapshot is currently playing, chain off its P3.
            // 3. Otherwise, fall back to the physical body's current transform position.
            if (snapshotList.Last != null)
            {
                // snapshotList.Last returns a LinkedListNode wrapper. 
                // We use '.Value' to extract the actual Vector3SplineSnapshot stored inside that node.
                startPosition = snapshotList.Last.Value.P3;
            }
            else if (activeSnapshot.HasValue)
            {
                // activeSnapshot is not null, so we access its contents using '.Value.P3'
                startPosition = activeSnapshot.Value.P3;
            }
            else
            {
                // If we have no queued or active snapshots, start the new curve 
                // directly from our current visual position to prevent any visual snapping.
                startPosition = transform.position;
            }

            Vector3 startVelocity = lastTargetVelocity;
            float segmentDuration = customDuration > 0f ? customDuration : defaultTickDuration;

            // Construct the 5-parameter snapshot
            Vector3SplineSnapshot snapshot = new Vector3SplineSnapshot(
                startPosition,
                startVelocity,
                targetPosition,
                targetVelocity,
                segmentDuration
            );

            // Add the new snapshot to the tail of our LinkedList
            snapshotList.AddLast(snapshot);

            // Cache velocity for the next incoming segment's start velocity
            lastTargetVelocity = targetVelocity;
        }

        private void LateUpdate()
        {
            if (physicalBodyTarget == null) return;

            // 1. If active snapshot is null and list has items, grab the next one from the front (FIFO)
            if (!activeSnapshot.HasValue && snapshotList.Count > 0)
            {
                // .First gets the first node in the list, and .Value gets its data payload
                activeSnapshot = snapshotList.First.Value;
                
                // Remove it from the list since it is now our active playing snapshot
                snapshotList.RemoveFirst();
                
                elapsedSnapshotTime = 0f;
            }

            // 2. If we have an active snapshot, evaluate it using its built-in duration
            if (activeSnapshot.HasValue)
            {
                elapsedSnapshotTime += Time.deltaTime;
                float duration = activeSnapshot.Value.Duration;
                
                // Prevent division by zero if duration is somehow 0 or negative
                // Ternary layout: (condition ? true_result : false_result)
                float normalizedAlpha = duration > 0f ? Mathf.Clamp01(elapsedSnapshotTime / duration) : 1.0f;

                // Evaluate position using the stateless solver
                transform.position = Vector3SplineSolver.Evaluate(activeSnapshot.Value, normalizedAlpha);

                // Once this snapshot's specific time window finishes, clear it so the next one can start
                if (normalizedAlpha >= 1.0f)
                {
                    activeSnapshot = null;
                }
            }
            // 3. Fallback chasing if the queue starves (no network snapshots available)
            else
            {
                transform.position = Vector3.Lerp(transform.position, physicalBodyTarget.position, Time.deltaTime * 20f);
            }
        }

        public void ClearBuffer()
        {
            snapshotList.Clear();
            activeSnapshot = null;
            elapsedSnapshotTime = 0f;
            lastTargetVelocity = Vector3.zero;
        }
    }
}