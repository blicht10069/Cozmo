using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CozmoAPI.Enums;

namespace CozmoAPI.MessageObjects
{

    public interface ILightIndex
    {
        int OnColor { get; set; }
        int OffColor { get; set; }
        int OnPeriodMS { get; set; }
        int OffPeriodMS { get; set; }
        int TransitionOnPeriodMS { get; set; }
        int TransitionOffPeriodMS { get; set; }
        int Offset { get; set; }
    }

    [CozFunction(CozMessageType.SetBackpackLEDs)]
    public class SetBackpackLED
    {
        [CozParameter(0)]
        private Int32Container[] mOnColor;
        [CozParameter(1)]
        private Int32Container[] mOffColor;
        [CozParameter(2)]
        private Int32Container[] mOnPeriodMS;
        [CozParameter(3)]
        private Int32Container[] mOffPeriodMS;
        [CozParameter(4)]
        private Int32Container[] mTransitionOnPeriodMS;
        [CozParameter(5)]
        private Int32Container[] mTransitionOffPeriodMS;
        [CozParameter(6)]
        private Int32Container[] mOffset;

        public SetBackpackLED()
            : base()
        {
            mOnColor = Int32Container.Create(5);
            mOffColor = Int32Container.Create(5);
            mOnPeriodMS = Int32Container.Create(5);
            mOffPeriodMS = Int32Container.Create(5);
            mTransitionOnPeriodMS = Int32Container.Create(5);
            mTransitionOffPeriodMS = Int32Container.Create(5);
            mOffset = Int32Container.Create(5);
        }

        public ILightIndex this[int index]
        {
            get { return new LightIndex(this, index); }
        }

        [CozParameter(7)]
        public byte RobotID
        {
            get;
            set;
        }

        public ILightIndex SetLightState(BackpackLightID light, Color color)
        {
            return SetLightState((int)light, color);
        }

        public void TurnLightOff(BackpackLightID light)
        {
            TurnLightOff((int)light);
        }

        public ILightIndex SetLightState(int lightIndex, Color color)
        {
            int c = color.R * (256 * 256 * 256) + color.G * (256 * 256 ) + color.B * 256 + 255;
            ILightIndex light = this[lightIndex];
            light.OnColor =c;
            light.OffColor = c;
            light.OnPeriodMS = 0;
            light.OffPeriodMS = 0;
            light.TransitionOnPeriodMS = 0;
            light.TransitionOffPeriodMS = 0;
            light.Offset = 0;
            return light;
        }
       
        public void TurnLightOff(int lightIndex)
        {
            SetLightState(lightIndex, Color.Black);
        }

        private class LightIndex : ILightIndex
        {
            private SetBackpackLED mOwner;
            private int mIndex;

            public LightIndex(SetBackpackLED owner, int index)
            {
                mOwner = owner;
                mIndex = index;
            }

            public int OnColor
            {
                get
                {
                    return mOwner.mOnColor[mIndex].Value;
                }
                set
                {
                    mOwner.mOnColor[mIndex].Value = value;
                }
            }

            public int OffColor
            {
                get
                {
                    return mOwner.mOffColor[mIndex].Value;
                }
                set
                {
                    mOwner.mOffColor[mIndex].Value = value;
                }

            }

            public int OnPeriodMS
            {
                get
                {
                    return mOwner.mOnPeriodMS[mIndex].Value;
                }
                set
                {
                    mOwner.mOnPeriodMS[mIndex].Value = value;
                }

            }

            public int OffPeriodMS
            {
                get
                {
                    return mOwner.mOffPeriodMS[mIndex].Value;
                }
                set
                {
                    mOwner.mOffPeriodMS[mIndex].Value = value;
                }

            }

            public int TransitionOnPeriodMS
            {
                get
                {
                    return mOwner.mTransitionOffPeriodMS[mIndex].Value;
                }
                set
                {
                    mOwner.mTransitionOffPeriodMS[mIndex].Value = value;
                }

            }

            public int TransitionOffPeriodMS
            {
                get
                {
                    return mOwner.mTransitionOnPeriodMS[mIndex].Value;
                }
                set
                {
                    mOwner.mTransitionOnPeriodMS[mIndex].Value = value;
                }

            }


            public int Offset
            {
                get
                {
                    return mOwner.mOffset[mIndex].Value;
                }
                set
                {
                    mOwner.mOffset[mIndex].Value = value;
                }

            }
        }
        
    }


    
}
