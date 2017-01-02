using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(24)]
    public class ActionSayText : IRobotActionUnion
    {
        public ActionSayText()
        {
            PlayEvent = 316;
            VoiceStyle = 2;
            DurationScalar = 1.8f;
            VoicePitch = 0f;
            FitToDuration = false;
        }

        [CozParameter(0)]
        public string Text
        {
            get;
            set;
        }

        [CozParameter(1)]
        public int PlayEvent
        {
            get;
            set;
        }

        [CozParameter(2)]
        public byte VoiceStyle
        {
            get;
            set;
        }

        [CozParameter(3)]
        public float DurationScalar
        {
            get;
            set;
        }

        [CozParameter(4)]
        public float VoicePitch
        {
            get;
            set;
        }

        [CozParameter(5)]
        public bool FitToDuration
        {
            get;
            set;
        }
    }
}
