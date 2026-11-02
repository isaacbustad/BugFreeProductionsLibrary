// Created By   :   Isaac Bustad
// Created      :   11/2/2026


using System;
using UnityEngine;

namespace BugFreeProductions.Spatial
{
    /// <summary>
    /// Master container packet holding absolute position and rotation data structs.
    /// </summary>
    public struct AbsoluteOrientationDataPacket
    {
        #region Vars
        public AbsolutePositionDataPacket positionData;
        public AbsoluteRotationDataPacket rotationData;
        #endregion Vars

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AbsoluteOrientationDataPacket struct.
        /// </summary>
        public AbsoluteOrientationDataPacket(AbsolutePositionDataPacket aAPDP, AbsoluteRotationDataPacket aARDP)
        {
            positionData = aAPDP;
            rotationData = aARDP;
        }
        #endregion Constructors
    }

    /// <summary>
    /// Compressed absolute coordinate packet representing X, Y, and Z positions as 16-bit shorts.
    /// </summary>
    public struct AbsolutePositionDataPacket
    {
        #region AbsolutePositionDataPacket Data
        public short x;
        public short y;
        public short z;
        #endregion AbsolutePositionDataPacket Data

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AbsolutePositionDataPacket struct with quantized coordinates.
        /// </summary>
        public AbsolutePositionDataPacket(short ax, short ay, short az)
        {
            x = ax;
            y = ay;
            z = az;
        }
        #endregion Constructors
    }

    /// <summary>
    /// Compressed absolute rotation packet using the "Smallest Three" quaternion compression format.
    /// </summary>
    public struct AbsoluteRotationDataPacket
    {
        #region AbsoluteRotationDataPacket Data
        public short rx;
        public short ry;
        public short rz;
        public byte largestIndex;
        #endregion AbsoluteRotationDataPacket Data

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the AbsoluteRotationDataPacket struct.
        /// </summary>
        public AbsoluteRotationDataPacket(short arx, short ary, short arz, byte ali)
        {
            rx = arx;
            ry = ary;
            rz = arz;
            largestIndex = ali;
        }
        #endregion Constructors
    }
}