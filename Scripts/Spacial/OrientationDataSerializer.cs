// Created By   :   Isaac Bustad
// Created      :   11/2/2026


using System;
using System.Collections;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    

    public static class OrientationDataSerializer
    {
        #region Vars

        #endregion Vars

        #region Methods

        #region Serialization
        /// <summary>
        /// Serializes an absolute Vector3 position into an AbsolutePositionDataPacket struct.
        /// </summary>
        public static AbsolutePositionDataPacket SerializeAbsolutePosition(Vector3 aPosition, float scaleFactor = 1000f)
        {
            // Scale and clamp the absolute coordinates into short values
            short x = (short)Mathf.Clamp(aPosition.x * scaleFactor, -32767, 32767);
            short y = (short)Mathf.Clamp(aPosition.y * scaleFactor, -32767, 32767);
            short z = (short)Mathf.Clamp(aPosition.z * scaleFactor, -32767, 32767);

            return new AbsolutePositionDataPacket(x, y, z);
        }

        /// <summary>
        /// Serializes an absolute Quaternion rotation into an AbsoluteRotationDataPacket struct 
        /// using the "Smallest Three" compression method.
        /// </summary>
        public static AbsoluteRotationDataPacket SerializeAbsoluteRotation(Quaternion aRotation)
        {
            // 1. Ensure the quaternion is normalized to maintain valid mathematical bounds
            aRotation.Normalize();

            // 2. Identify which component (x, y, z, or w) has the largest absolute value.
            int largestIndex = 0;
            float maxAbsValue = Mathf.Abs(aRotation.x);

            if (Mathf.Abs(aRotation.y) > maxAbsValue) { maxAbsValue = Mathf.Abs(aRotation.y); largestIndex = 1; }
            if (Mathf.Abs(aRotation.z) > maxAbsValue) { maxAbsValue = Mathf.Abs(aRotation.z); largestIndex = 2; }
            if (Mathf.Abs(aRotation.w) > maxAbsValue) { maxAbsValue = Mathf.Abs(aRotation.w); largestIndex = 3; }

            float[] q = { aRotation.x, aRotation.y, aRotation.z, aRotation.w };

            // 3. Force the largest component to be positive. If it is negative, invert all components.
            float sign = (q[largestIndex] < 0f) ? -1f : 1f;

            // 4. Pack the remaining 3 components into shorts.
            const float scaleQ = 32767.0f / 0.7071067f;
            short[] compressedShorts = new short[3];
            int destIdx = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == largestIndex) continue;

                float scaledVal = q[i] * sign * scaleQ;
                compressedShorts[destIdx++] = (short)Mathf.Clamp(scaledVal, -32767, 32767);
            }

            // 5. Instantiate and return using the struct constructor
            return new AbsoluteRotationDataPacket(compressedShorts[0], compressedShorts[1], compressedShorts[2], (byte)largestIndex);
        }

        /// <summary>
        /// Combines absolute position and absolute rotation data into a unified orientation packet container.
        /// </summary>
        public static AbsoluteOrientationDataPacket SerializeAbsoluteOrientation(OrientationData aOD)
        {
            return new AbsoluteOrientationDataPacket(
                SerializeAbsolutePosition(aOD.positionData),
                SerializeAbsoluteRotation(aOD.rotationData)
            );
        }

        #region Vars
        private const int PayloadSize = 13; // Position (6 bytes) + Rotation (7 bytes)
        #endregion Vars

        /// <summary>
        /// Serializes an AbsoluteOrientationDataPacket into a compact 13-byte array for network transmission.
        /// </summary>
        public static byte[] SerializePackage(AbsoluteOrientationDataPacket aPacket)
        {
            byte[] buffer = new byte[PayloadSize];
            int offset = 0;

            // 1. Serialize Position Data (6 bytes: 3 shorts)
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.positionData.x); 
            offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.positionData.y); 
            offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.positionData.z); 
            offset += 2;

            // 2. Serialize Rotation Data (7 bytes: 3 shorts + 1 byte index)
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.rotationData.rx); 
            offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.rotationData.ry); 
            offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), aPacket.rotationData.rz); 
            offset += 2;
            buffer[offset++] = aPacket.rotationData.largestIndex;

            return buffer;
        }

        #endregion Serialization

        #region DeSerialization
        /// <summary>
        /// Deserializes a 13-byte network buffer back into an AbsoluteOrientationDataPacket struct.
        /// </summary>
        public static AbsoluteOrientationDataPacket DeserializePackage(byte[] payload)
        {
            //Debug.Log($"[Serializer] Deserializing payload of length: {(payload != null ? payload.Length : 0)}");
            if (payload == null || payload.Length < PayloadSize)
            {
                throw new ArgumentException($"Buffer is too small to deserialize AbsoluteOrientationDataPacket. Expected at least {PayloadSize} bytes.");
            }

            int offset = 0;

            // 1. Read Position Data (6 bytes: 3 shorts)
            short x = BitConverter.ToInt16(payload, offset);
            offset += 2;
            short y = BitConverter.ToInt16(payload, offset);
            offset += 2;
            short z = BitConverter.ToInt16(payload, offset);
            offset += 2;

            AbsolutePositionDataPacket posPacket = new AbsolutePositionDataPacket(x, y, z);

            // 2. Read Rotation Data (7 bytes: 3 shorts + 1 byte index)
            short rx = BitConverter.ToInt16(payload, offset);
            offset += 2;
            short ry = BitConverter.ToInt16(payload, offset);
            offset += 2;
            short rz = BitConverter.ToInt16(payload, offset);
            offset += 2;
            byte largestIndex = payload[offset++];

            AbsoluteRotationDataPacket rotPacket = new AbsoluteRotationDataPacket(rx, ry, rz, largestIndex);

            // 3. Package and return the master container
            return new AbsoluteOrientationDataPacket(posPacket, rotPacket);
        }

        /// <summary>
        /// Deserializes an AbsolutePositionDataPacket back into a world-space Vector3 position.
        /// </summary>
        public static Vector3 DeserializeAbsolutePosition(AbsolutePositionDataPacket aAPDP, float scaleFactor = 1000f)
        {
            float x = aAPDP.x / scaleFactor;
            float y = aAPDP.y / scaleFactor;
            float z = aAPDP.z / scaleFactor;

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Deserializes an AbsoluteRotationDataPacket back into a world-space Quaternion 
        /// by reconstructing the dropped component using the "Smallest Three" algorithm.
        /// </summary>
        public static Quaternion DeserializeAbsoluteRotation(AbsoluteRotationDataPacket aARDP)
        {
            const float scaleQ = 32767.0f / 0.7071067f;
            
            float[] q = new float[4];
            int largestIndex = aARDP.largestIndex;

            // 1. Unpack the 3 stored short values back into normalized float components
            float[] compressedVals = { 
                aARDP.rx / scaleQ, 
                aARDP.ry / scaleQ, 
                aARDP.rz / scaleQ 
            };

            int srcIdx = 0;
            float sumOfSquares = 0f;

            for (int i = 0; i < 4; i++)
            {
                if (i == largestIndex) continue;

                q[i] = compressedVals[srcIdx++];
                sumOfSquares += q[i] * q[i];
            }

            // 2. Reconstruct the dropped largest component using the identity: x^2 + y^2 + z^2 + w^2 = 1
            q[largestIndex] = Mathf.Sqrt(Mathf.Clamp01(1f - sumOfSquares));

            return new Quaternion(q[0], q[1], q[2], q[3]);
        }

        /// <summary>
        /// Deserializes an AbsoluteOrientationDataPacket container into a complete OrientationData structure.
        /// </summary>
        public static OrientationData DeserializeOrientation(AbsoluteOrientationDataPacket aAODP)
        {
            return new OrientationData(
                DeserializeAbsolutePosition(aAODP.positionData),
                DeserializeAbsoluteRotation(aAODP.rotationData)
            );
        }

        #endregion DeSerialization
        #endregion Methods
    }
}