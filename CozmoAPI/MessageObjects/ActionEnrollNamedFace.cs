using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.enrollNamedFace)]
    public class ActionEnrollNamedFace : IRobotActionUnion
    {
        [CozParameter(3)]
        private byte mFaceEnrollmentSequence;

        public ActionEnrollNamedFace()
            : base()
        {
            SaveToRobot = true;
            Name = String.Empty;
            FaceEnrollmentSequence = FaceEnrollmentSequenceType.Default;
            FaceID = 0;
            MergeIntoID = 0;
        }

        [CozParameter(0)]
        public int FaceID
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int MergeIntoID
        {
            get;
            set;
        }

        [CozParameter(2)]
        public string Name
        {
            get;
            set;
        }

        public FaceEnrollmentSequenceType FaceEnrollmentSequence
        {
            get { return (FaceEnrollmentSequenceType)mFaceEnrollmentSequence; }
            set { mFaceEnrollmentSequence = (byte)value; }
        }

        [CozParameter(4)]
        public bool SaveToRobot
        {
            get;
            set;
        }
    }
}
