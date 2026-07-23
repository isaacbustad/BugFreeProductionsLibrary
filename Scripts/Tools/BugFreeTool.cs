// Isaac Bustad
// 8/9/2024


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BugFreeProductions.Extentions
{
    public static class BugFreeTool
    {
        #region Velocity Limmiter World Speed
        public const float worldSpeed = 25f;

        public static void LimitToWorldVelocity(this Vector3 tV3)
        {
            if (tV3.magnitude > worldSpeed)
            {
                // limmit the velosity under word speed

                // store current Velocity


                // create new velocity

                tV3.x = Mathf.Clamp(tV3.x, 0, worldSpeed);
                tV3.y = Mathf.Clamp(tV3.y, 0, worldSpeed);
                tV3.z = Mathf.Clamp(tV3.z, 0, worldSpeed);
            }

        }

        public static void LimitVelocity(this Vector3 tV3, float aMag)
        {
            tV3.x = Mathf.Clamp(tV3.x, 0, aMag);
            tV3.y = Mathf.Clamp(tV3.y, 0, aMag);
            tV3.z = Mathf.Clamp(tV3.z, 0, aMag);
        }

        public static Quaternion CalcNewRotation( Vector3 startForward, Vector3 endForward, Vector3 startUp, Vector3 endUp, float rateOfDecay)
        {
            Vector3 targetForward = (1f - rateOfDecay) * startForward + rateOfDecay * endForward;
            Vector3 targetUp = (1f - rateOfDecay) * startUp + rateOfDecay * endUp;

            return Quaternion.LookRotation(targetForward,targetUp);
        }

        

        
        public static Quaternion StepRotateWithBuffer(Quaternion current, Quaternion target, float maxDegreesDelta, float bufferAngle, out bool isFinished)
        {
            // 1. Find the remaining angle in degrees between rotations
            float remainingAngle = Quaternion.Angle(current, target);

            // 2. Check if we are within the snap threshold
            if (remainingAngle <= bufferAngle)
            {
                isFinished = true;
                return target;
            }

            // 3. Step forward by maxDegreesDelta degrees without any lerp/slerp smoothing
            isFinished = false;
            return Quaternion.RotateTowards(current, target, maxDegreesDelta);
        }

        public static Quaternion CalcNewRotation(   Quaternion currentRotation, 
                                                    Quaternion targetRotation, 
                                                    float progressStep)
        {
            // 1. Calculate how far apart the two rotations are in degrees
            float remainingAngleInDegrees = Quaternion.Angle(currentRotation, targetRotation);
    
            return Quaternion.Slerp(currentRotation, targetRotation, progressStep);
        }

        public static bool CalcNewRotation( Quaternion currentRotation, 
                                            Quaternion targetRotation, 
                                            float progressStep, 
                                            float bufferAngleInDegrees, 
                                            out Quaternion resultRotation)
        {
            // 1. Check if we are already close enough to snap
            if (Quaternion.Angle(currentRotation, targetRotation) <= bufferAngleInDegrees)
            {
                resultRotation = targetRotation; // Snap to target
                return true;                     // Rotation is finished
            }

            // 2. Otherwise, step toward the target
            resultRotation = Quaternion.Slerp(currentRotation, targetRotation, progressStep);
            return false;
        }
        public static float CalcAngleViaSides(float oppSide, float oth1, float oth2)
        {
            float top = (oppSide * oppSide) - (oth1 * oth1) - (oth2 * oth2);
            float bottom = -2 * oth1 * oth2;
            float fract = top / bottom;

            return Mathf.Acos(fract) * Mathf.Rad2Deg;
        }

        
        // Accessors
        //public static float WorldSpeed { get { return worldSpeed; } }
        #endregion
    }
}
