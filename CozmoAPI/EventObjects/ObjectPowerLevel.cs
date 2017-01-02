using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.EventObjects
{
    [CozFunction(0x0d, IsEvent=true)]
    public class ObjectPowerLevel : IDebugInformation
    {
        public ObjectPowerLevel()
            : base()
        {
        }

        [CozParameter(0)]
        public int ObjectID
        {
            get;
            set;
        }

        [CozParameter(1)]
        public byte BatteryLevel
        {
            get;
            set;
        }

        public override string ToString()
        {
            return String.Format("Battery At {0}", BatteryLevel);
        }
    }
}
