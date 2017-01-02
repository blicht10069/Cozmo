using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.CozDataStructures
{
    [CozFunction]
    public class CozPathMotionProfile
    {
        public CozPathMotionProfile()
        {
            SpeedMMPS = 100f;
            AcclMMPS2 = 200f;
            DecclDocMMSP2 = 500f;
            TurnSpeedRadPS = 2f;
            TurnAcclRadPS2 = 100f;
            TurnDecclRadPS2 = 500f;
            DockSpeedMMS = 60f;
            AcclDockMMSP2 = 200f;
            DecclDocMMSP2 = 500f;
            ReverseSpeedMMPS = 80f;
            IsCustom = false;
        }

        [CozParameter(0)]
        public float SpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(1)]
        public float AcclMMPS2
        {
            get;
            set;
        }

        [CozParameter(2)]
        public float DecclMMPS2
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float TurnSpeedRadPS
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float TurnAcclRadPS2
        {
            get;
            set;
        }

        [CozParameter(5)]
        public float TurnDecclRadPS2
        {
            get;
            set;
        }

        [CozParameter(6)]
        public float DockSpeedMMS
        {
            get;
            set;
        }

        [CozParameter(7)]
        public float AcclDockMMSP2
        {
            get;
            set;
        }

        [CozParameter(8)]
        public float DecclDocMMSP2
        {
            get;
            set;
        }

        [CozParameter(9)]
        public float ReverseSpeedMMPS
        {
            get;
            set;
        }

        [CozParameter(10)]
        public bool IsCustom
        {
            get;
            set;
        }
    }
}
