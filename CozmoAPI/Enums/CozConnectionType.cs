using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Enums
{
    public enum CozConnectionType : byte
    {
        UI = 0,
        SdkOverUdp = 1,
        SdkOverTcp = 2,
        Count = 3
    }

    public enum CozConnectionResult : byte
    {
        Success = 0,
        ConnectionFailure = 1,
        ConnectionRejected = 2,
        OutdatedFirmware = 3,
        OutdatedApp = 4,
        NeedsPin = 5,
        InvalidPin = 6,
        PinMaxAttemptsReached = 7
    }
}
