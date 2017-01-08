using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CozmoAPI.Enums;

namespace CozmoAPI
{
    public class CozEventWait : IDisposable
    {
        private ManualResetEvent mReset;

        public CozEventWait(CozConnection connection, CozEventWaitSetup setup)
        {
            Setup = setup;
            mReset = new ManualResetEvent(false);
            Connection = connection;
            Connection.RobotEvent += OnRobotEvent;
        }


        public CozEventWait(CozConnection connection, params CozEventType[] events)
            : this(connection, new CozEventWaitSetup(events))
        {

        }

        public static CozEventWait CreateNullWait(CozConnection connection, CozEventWaitSetup setup)
        {
            CozEventWait ret = new CozEventWait(connection, setup);
            ret.mReset.Set();
            return ret;
        }

        public void Dispose()
        {
            IsComplete = true;
            Connection.RobotEvent -= OnRobotEvent;
            Connection.RemoveWait(this);
            OnDispose();
        }

        protected void InvokeSetupCallbacks()
        {
            if (Result != null && Setup.Success != null)
                Setup.Success(Result);
            else if (Result == null && Setup.Fail != null)
                Setup.Fail();
        }

        public RobotEventArgs Result
        {
            get;
            private set;
        }

        protected CozEventWaitSetup Setup
        {
            get;
            private set;
        }

        protected CozConnection Connection
        {
            get;
            private set;
        }

        protected virtual void RobotEvent(RobotEventArgs e)
        {
        }

        protected virtual void OnDispose()
        {
        }

        public bool IsComplete
        {
            get;
            private set;
        }

        public void Abort()
        {
            mReset.Set();            
            Dispose();
        }

        public CozEventWait Wait()
        {
            mReset.WaitOne();
            InvokeSetupCallbacks();
            return this;
        }

        public CozEventWait Wait(TimeSpan timeout)
        {
            mReset.WaitOne(timeout);
            InvokeSetupCallbacks();
            return this;
        }


        private void OnRobotEvent(RobotEventArgs e)
        {
            RobotEvent(e);
            if (Setup.HasEvent(e.EventType))
            {
                Result = e;
                mReset.Set();
                Dispose();
            }
        }

    }

    public class CozEventWaitSetup
    {
        private HashSet<CozEventType> mEventTypes;

        public CozEventWaitSetup()
        {
            mEventTypes = new HashSet<CozEventType>();
        }

        public CozEventWaitSetup(params CozEventType[] events)
            : this()
        {
            Add(events);
        }

        public void Add(params CozEventType[] events)
        {
            foreach (CozEventType evt in events)
            {
                if (!mEventTypes.Contains(evt))
                    mEventTypes.Add(evt);
            }
        }

        public bool HasEvent(CozEventType evt)
        {
            return mEventTypes.Contains(evt);
        }

        public Action<RobotEventArgs> Success
        {
            get;
            set;
        }

        public Action Fail
        {
            get;
            set;
        }
    }
}
