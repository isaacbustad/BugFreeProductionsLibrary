// Created By   :   Isaac Bustad
// Created      :   10/26/2026



using UnityEngine;

namespace BugFreeProductions.Spatial
{
    /// <summary>
    /// Represents a spatial state snapshot containing position and velocity 
    /// for cubic Hermite spline interpolation.
    /// </summary>
    public struct Vector3SplineSnapshot
    {
        public Vector3 P0;
        public Vector3 P1;
        public Vector3 P2;
        public Vector3 P3;


        public Vector3SplineSnapshot(Vector3 aStartPosition, Vector3 aStartVelocity, Vector3 aEndPosition, Vector3 aEndVelocity)
        {
            P0 = aStartPosition;
            P1 = aStartPosition + aStartVelocity;
            P2 = aEndPosition - aEndVelocity;
            P3 = aEndPosition;

        }
    }

    public static class Vector3SplineSolver
    {
        /// <summary>
        /// Evaluates a 3D Hermite cubic spline position between two snapshots 
        /// at normalized time step t (0.0 to 1.0).
        /// </summary>
        public static Vector3 Evaluate(Vector3SplineSnapshot aSnapshot, float t)
        {
            t = Mathf.Clamp01(t);
            float tSquare = t * t;
            float tCube = tSquare * t;


            // 2. Compute pre-factored kinematic polynomial coefficients
            Vector3 D = aSnapshot.P0;
            Vector3 C = 3f * (aSnapshot.P1 - aSnapshot.P0);
            Vector3 B = 3f * (aSnapshot.P2 - (2f * aSnapshot.P1) + aSnapshot.P0);
            Vector3 A = aSnapshot.P3 - (3f * aSnapshot.P2) + (3f * aSnapshot.P1) - aSnapshot.P0;

            // 3. Parametric evaluation: P(t) = A*t^3 + B*t^2 + C*t + D
            return (A * tCube) + (B * tSquare) + (C * t) + D;
        }
    }
}