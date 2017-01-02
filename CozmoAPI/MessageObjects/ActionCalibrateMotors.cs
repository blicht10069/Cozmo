using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(ActionType.calibrateMotors)]
    public class ActionCalibrateMotors : IRobotActionUnion
    {
        public ActionCalibrateMotors()
        {
            CalibrateHead = true;
            CalibrateLift = true;

        }

        [CozParameter(0)]
        public bool CalibrateHead
        {
            get;
            set;
        }

        [CozParameter(1)]
        public bool CalibrateLift
        {
            get;
            set;
        }
    }
}
