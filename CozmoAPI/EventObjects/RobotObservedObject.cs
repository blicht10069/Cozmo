using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x34, IsEvent=true)]
    public class RobotObservedObject
    {
        public RobotObservedObject()
             : base()
        {
        }

        [CozParameter(0)]
        public int RobotID
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int Timestamp
        {
            get;
            set;
        }

        [CozParameter(2)]
        private int mFamilyType = 0;
        public ObjectFamilyTypes FamilyType
        {
            get { return (ObjectFamilyTypes)mFamilyType; }
        }

        [CozParameter(3)]
        private int mObjectType = 0;
        public ObjectType ObjectType
        {
            get { return (ObjectType)mObjectType; }
        }

        [CozParameter(4)]
        public int ObjectID
        {
            get;
            set;
        }

        [CozParameter(5)]
        public CozRect ImageRect
        {
            get;
            set;
        }

        [CozParameter(6)]
        public CozPose3D Post
        {
            get;
            set;
        }

        [CozParameter(7)]
        public float TopFaceOrientationInRad
        {
            get;
            set;
        }

        [CozParameter(8)]
        public bool IsActive
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Utilities.DebugObject(this);
        }
    }
}
