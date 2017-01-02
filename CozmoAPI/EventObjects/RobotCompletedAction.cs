using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x48, IsEvent=true)]
    public class RobotCompletedAction
    {
        [CozParameter(0)]
        private int mRobotId = 0;
        [CozParameter(1)]
        private int mIdTag = 0;
        [CozParameter(2)]
        private short mActionType = 0;
        [CozParameter(3)]
        private byte mResult = 0;

        public RobotCompletedAction()
            : base()
        {
        }

        public int RobotId
        {
            get { return mRobotId; }           
        }

        public int IdTag
        {
            get { return mIdTag; }
        }

        public short ActionType
        {
            get { return mActionType; }
        }

        public byte Result
        {
            get { return mResult; }
        }

    }
}
