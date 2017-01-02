using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    
    [CozFunction(CozMessageType.SetHeadlight)]
    public class HeadlightControl
    {
        public HeadlightControl()
        {
        }

        [CozParameter(0)]
        public bool IsLightOn
        {
            get;
            set;
        }
    }
}
