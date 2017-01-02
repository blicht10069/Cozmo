using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(97)]
    public class AbortAction
    {
        public AbortAction()
            : base()
        {
            RobotId = 1;
        }

        
        [CozParameter(0)]
        public int IdTag
        {
            get;
            set;
        }

        [CozParameter(1)]
        public byte RobotId
        {
            get;
            set;
        }
    }
}
