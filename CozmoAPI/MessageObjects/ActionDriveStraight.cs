using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(5)]
    public class ActionDriveStraight : IRobotActionUnion
    {
        public ActionDriveStraight()
        {
            SpeedMMPS = 10;
            DistMM = 50;
            ShouldPlayAnimation = false;
        }

        [CozParameter(0)]
        public float SpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float DistMM
        {
            get;
            set;
        }

        [CozParameter(2)]
        public bool ShouldPlayAnimation
        {
            get;
            set;
        }
    }
}
