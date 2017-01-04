using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x1e, IsEvent = true)]
    public class RobotConnectionResponse
    {
        private int mRobotID;
        private byte mConnectionResult;
        private int mVersion;
        public int mSerialNumber;

        public int RobotID
        {
            get { return mRobotID; }
        }

        public CozConnectionResult ConnectionResult
        {
            get { return (CozConnectionResult)mConnectionResult; }
        }

        public int Version
        {
            get { return mVersion; }
        }

        public int SerialNumber
        {
            get { return mSerialNumber; }
        }

        public override string ToString()
        {
            return String.Format("Robot {0}, Version {1}, Serial Number {2}: Connected - {3}",
                RobotID, ConnectionResult, Version, SerialNumber);
        }
    }
}
