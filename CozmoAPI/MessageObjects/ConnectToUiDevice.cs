using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(CozMessageType.ConnectToUiDevice)]
    public class ConnectToUiDevice
    {
        [CozParameter(0)]
        private byte mConnectionType;

        public ConnectToUiDevice()
        {
            DeviceID = 0;
        }
        public CozConnectionType ConnectionType
        {
            get { return (CozConnectionType)mConnectionType; }
            set { mConnectionType = (byte)value; }
        }

        [CozParameter(1)]
        public byte DeviceID
        {
            get;
            set;
        }
    }
}
