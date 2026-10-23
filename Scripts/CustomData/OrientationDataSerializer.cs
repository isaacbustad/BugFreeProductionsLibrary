// Created By   :   Isaac Bustad
// Created      :   10/6/2026

using System;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    public struct OrientationDataPacket
    {
        public short qx;
        public short qy;
        public short qz;
        public byte mask;
    }

    public static class OrientationDataSerializer
    {
        private const int PayloadSize = 7;

        /// <summary>
        /// Evaluates the delta and serializes into a compact 7-byte array.
        /// </summary>
        public static byte[] Serialize(Vector3 delta, float threshold, float scaleFactor = 1000f)
        {
            // Check if ANY axis exceeds the threshold. If not, don't allocate or send anything!
            bool exceedsThreshold = Mathf.Abs(delta.x) >= threshold || 
                                    Mathf.Abs(delta.y) >= threshold || 
                                    Mathf.Abs(delta.z) >= threshold;

            if (!exceedsThreshold) return null;

            byte mask = 0;
            short qx = 0, qy = 0, qz = 0;

            if (Mathf.Abs(delta.x) >= threshold) { mask |= 1; qx = (short)(delta.x * scaleFactor); }
            if (Mathf.Abs(delta.y) >= threshold) { mask |= 2; qy = (short)(delta.y * scaleFactor); }
            if (Mathf.Abs(delta.z) >= threshold) { mask |= 4; qz = (short)(delta.z * scaleFactor); }

            byte[] buffer = new byte[PayloadSize];
            int offset = 0;

            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), qx); offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), qy); offset += 2;
            BitConverter.TryWriteBytes(buffer.AsSpan(offset, 2), qz); offset += 2;
            buffer[offset++] = mask;

            return buffer;
        }

        /// <summary>
        /// Deserializes a 7-byte array back into an OrientationDataPacket.
        /// </summary>
        public static OrientationDataPacket Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length < PayloadSize)
            {
                throw new ArgumentException($"Buffer is too small to deserialize OrientationDataPacket. Expected {PayloadSize} bytes.");
            }

            OrientationDataPacket data = new OrientationDataPacket();
            int offset = 0;

            data.qx = BitConverter.ToInt16(buffer, offset);
            offset += 2;
            data.qy = BitConverter.ToInt16(buffer, offset);
            offset += 2;
            data.qz = BitConverter.ToInt16(buffer, offset);
            offset += 2;
            data.mask = buffer[offset++];

            return data;
        }

        public static Vector3 DeserializePosition(byte[] buffer)
        {
            OrientationDataPacket packet = Deserialize(buffer);

            // Reconstruct the Vector3 delta using the scale factor (e.g., 1000f)
            float scaleFactor = 1000f;
            
            // Check the bitmask or directly unpack the shorts depending on how strict you want to be
            float dx = ((packet.mask & 1) != 0) ? (packet.qx / scaleFactor) : 0f;
            float dy = ((packet.mask & 2) != 0) ? (packet.qy / scaleFactor) : 0f;
            float dz = ((packet.mask & 4) != 0) ? (packet.qz / scaleFactor) : 0f;

            // Create new vector 3
            return new Vector3 (dx, dy, dz); 
        }


        public static OrientationData DeserializeOrientation(byte[] buffer)
        {
            OrientationDataPacket packet = Deserialize(buffer);

            // Reconstruct the Vector3 delta using the scale factor (e.g., 1000f)
            float scaleFactor = 1000f;
            
            // Check the bitmask or directly unpack the shorts depending on how strict you want to be
            float dx = ((packet.mask & 1) != 0) ? (packet.qx / scaleFactor) : 0f;
            float dy = ((packet.mask & 2) != 0) ? (packet.qy / scaleFactor) : 0f;
            float dz = ((packet.mask & 4) != 0) ? (packet.qz / scaleFactor) : 0f;

            // Create new vector 3
            return new OrientationData(new Vector3(dx, dy, dz), Quaternion.identity) ; 
        }
    }
}