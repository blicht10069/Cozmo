using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.Enums
{
    public enum CozVoiceStyle : byte
    {
        Unprocessed = 0,
        CozmoProcessing_Name = 1,
        CozmoProcessing_Sentence = 2,
        Count = 3
    }
}
