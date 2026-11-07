// Created By   :   Isaac Bustad
// Created      :   11/2/2026

using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public class OrientationReconciliation : MonoBehaviour
    {
        [Header("Target Tracking")]
        [SerializeField] private float defaultTickDuration = 0.033f; // Fallback duration
        [SerializeField] private Transform targetTF;

        private float timeOnTweenBuffer = 0f;
        private float currentTweenDuration = 0.033f;

        public void PushCorrection(float customDuration = -1f)
        {
            float duration = (customDuration > 0) ? customDuration : defaultTickDuration;

            // Add to the existing buffer time and update duration
            timeOnTweenBuffer += duration;
            currentTweenDuration = duration;
        }

        private void LateUpdate()
        {
            if (timeOnTweenBuffer > 0f && targetTF != null)
            {
                RenderSnapshot();
            }
        }

        private void RenderSnapshot()
        {
            // Decrement remaining tween time using frame delta time for smooth per-frame rendering
            timeOnTweenBuffer -= Time.deltaTime;

            // Calculate progress ratio (0.0 to 1.0) based on remaining time vs total duration
            float t = 1f - Mathf.Clamp01(timeOnTweenBuffer / currentTweenDuration);

            // Pull live orientation data from targetTF
            OrientationData targetData = targetTF.OrientationData();

            // Blends smoothly per frame from current transform position/rotation toward the live target
            transform.position = Vector3.Lerp(transform.position, targetData.positionData, t);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetData.rotationData, t);

            // Clean up when time runs out
            if (timeOnTweenBuffer <= 0f || t >= 1f)
            {
                transform.position = targetData.positionData;
                transform.rotation = targetData.rotationData;
                timeOnTweenBuffer = 0f;
            }
        }
    }
}