// Created By   :   Isaac Bustad
// Created      :   10/6/2026

using UnityEngine;

namespace BugFreeProductions.Spatial
{
    /// <summary>
    /// Handles specialized quaternion alignment, shortest-path checks, and smooth 
    /// rotational reconciliation to prevent snapping and double-cover flipping artifacts.
    /// </summary>
    public static class RotationReconciliation
    {
        #region Methods
        /// <summary>
        /// Performs a shortest-path spherical linear interpolation (Slerp) between two quaternions,
        /// avoiding the "long way around" flip caused by quaternion double-covering.
        /// </summary>
        public static Quaternion ShortestPathSlerp(Quaternion aStart, Quaternion aEnd, float aT)
        {
            // 1. Calculate the dot product to evaluate angular alignment.
            // A negative dot product means the quaternions are more than 90 degrees apart 
            // in 4D space and represent opposing hemispherical representations of the same rotation.
            float dot = Quaternion.Dot(aStart, aEnd);

            // 2. If negative, invert the target quaternion so Slerp takes the shortest path.
            if (dot < 0f)
            {
                aEnd = new Quaternion(-aEnd.x, -aEnd.y, -aEnd.z, -aEnd.w);
            }

            // 3. Execute standard Slerp along the guaranteed shortest arc
            return Quaternion.Slerp(aStart, aEnd, aT);
        }

        /// <summary>
        /// Reconciles a current orientation rotation toward a target rotation over time.
        /// </summary>
        public static Quaternion ReconcileRotation(Quaternion aCurrent, Quaternion aTarget, float aDeltaTime, float aSmoothingSpeed)
        {
            float t = Mathf.Clamp01(aDeltaTime * aSmoothingSpeed);
            return ShortestPathSlerp(aCurrent, aTarget, t);
        }
        #endregion Methods
    }
}