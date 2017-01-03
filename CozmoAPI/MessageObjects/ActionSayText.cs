using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{
    [CozFunction(24)]
    public class ActionSayText : IRobotActionUnion
    {
        [CozParameter(1)]
        private int mPlayEvent;
        [CozParameter(2)]
        private byte mVoiceStyle;

        public ActionSayText()
        {
            PlayEvent = CozAnimationTriggerType.Count;
            VoiceStyle = CozVoiceStyle.CozmoProcessing_Sentence;
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
        
        public CozAnimationTriggerType PlayEvent
        {
            get { return (CozAnimationTriggerType)mPlayEvent; }
            set { mPlayEvent = (int)value; }
        }
        
        public CozVoiceStyle VoiceStyle
        {
            get { return (CozVoiceStyle)mVoiceStyle; }
            set { mVoiceStyle = (byte)value; }
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
