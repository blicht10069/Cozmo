using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.SetCameraSettings)]
    public class SetCameraSettings
    {
        public SetCameraSettings()
        {
            ExposureMS = 0;
            Gain = 0;
        }

        public short ExposureMS
        {
            get;
            set;
        }

        public float Gain
        {
            get;
            set;
        }
    
    }
}
