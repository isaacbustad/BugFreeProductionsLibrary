// Created By   :   Isaac Bustad
// Created      :   7/25/2026


using UnityEngine;



namespace BugFreeProductions.Spatial
{
    public static class OrientationExtension
    {
        

        public static float GetAngleToTarget(OrientationData current, Vector3 targetPos)
        {
            Quaternion targetRotation = RotationTool.CalculateLookAt(current.positionData, targetPos);
            // Measure the minimal angle between where we are facing vs where we NEED to face
            return Quaternion.Angle(current.rotationData, targetRotation);
        }



            
    }
}