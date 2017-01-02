using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;
using CozmoAPI.Enums;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x37, IsEvent=true)]
    public class RobotObservedFace
    {
        [CozParameter(0)]
        private int mFaceID;
        [CozParameter(1)]
        private int mRobotID;
        [CozParameter(2)]
        private int mTimestamp;
        [CozParameter(3)]
        private CozPose3D mPose3D;
        [CozParameter(4)]
        private CozRect mImageRect;
        [CozParameter(5)]
        private string mName;
        [CozParameter(6)]
        private byte mFacialExpression;
        [CozParameter(7, ArrayOfSpecialType=typeof(CozPoint), OverrideType=typeof(CozPoint))]
        private CozPoint[] mLeftEye;
        [CozParameter(8, ArrayOfSpecialType = typeof(CozPoint), OverrideType = typeof(CozPoint))]
        private CozPoint[] mRightEye;
        [CozParameter(9, ArrayOfSpecialType = typeof(CozPoint), OverrideType = typeof(CozPoint))]
        private CozPoint[] mNose;
        [CozParameter(10, ArrayOfSpecialType = typeof(CozPoint), OverrideType = typeof(CozPoint))]
        private CozPoint[] mMouth;

        public RobotObservedFace()
            : base()
        {
        }

        public int FaceID
        {
            get { return mFaceID; }
        }

        public int RobotID
        {
            get { return mRobotID; }
        }

        public int Timestamp
        {
            get { return mTimestamp; }
        }

        public CozPose3D Pose3D
        {
            get { return mPose3D; }
        }

        public CozRect ImageRect
        {
            get { return mImageRect; }
        }

        public string Name
        {
            get { return mName; }
        }

        public FacialExpressionType FacialExpression
        {
            get { return (FacialExpressionType)mFacialExpression; }
        }

        public CozPoint[] LeftEye
        {
            get { return mLeftEye; }
        }

        public CozPoint[] RightEye
        {
            get { return mRightEye; }
        }

        public CozPoint[] Nose
        {
            get { return mNose; }
        }

        public CozPoint[] Mouth
        {
            get { return mMouth; }
        }
    }
}
