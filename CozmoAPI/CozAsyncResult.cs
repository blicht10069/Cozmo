using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CozmoAPI.EventObjects;
using CozmoAPI.MessageObjects;

namespace CozmoAPI
{

    public class CozAsyncResult
    {
        private ManualResetEvent mReset;
        private Func<RobotEventArgs, bool> mCallback;        

        private CozConnection mConnection;

        protected CozAsyncResult()
        {
        }

        public static CozAsyncResult CreateEmptyResult(CozConnection connection)
        {
            CozAsyncResult ret = new CozAsyncResult();
            ret.mConnection = connection;
            ret.mReset = new ManualResetEvent(true);
            ret.Command = null;
            ret.ResultCode = 1;
            ret.Completed = null;
            ret.IsComplete = true;
            return ret;
        }
        
             

        public CozAsyncResult(CozConnection connection, IRobotCommand command, Func<RobotEventArgs, bool> callback)
            : base()
        {
            IsComplete = false;
            if (callback == null) throw new ArgumentException("The callback cannot be null");
            mReset = new ManualResetEvent(false);
            mCallback = callback;
            Command = command;
            mConnection = connection;
            mConnection.RobotEvent += OnRobotEvent;
            ResultCode = 0;
        }

        private void OnRobotEvent(RobotEventArgs e)
        {
            if (mCallback(e))
            {
                mConnection.RobotEvent -= OnRobotEvent;
                mReset.Set();
                IsComplete = true;
                RobotCompletedAction rca = e.Data as RobotCompletedAction;
                if (rca != null) ResultCode = rca.Result;
                if (Completed != null)
                    Completed(new AsyncCompleteArgs(e, rca, false));
            }
        }

        public Action<AsyncCompleteArgs> Completed
        {
            get;
            set;
        }

        public byte ResultCode
        {
            get;
            private set;
        }

        public bool IsComplete
        {
            get;
            private set;
        }

        public IRobotCommand Command
        {
            get;
            protected set;
        }

        public bool Wait(TimeSpan ts)
        {
            return WaitHandle.WaitOne(ts);
        }

        public bool Wait(int timeoutMS)
        {
            return WaitHandle.WaitOne(timeoutMS);
        }

        public bool Wait()
        {
            return WaitHandle.WaitOne();
        }

        public WaitHandle WaitHandle
        {
            get { return mReset; }
        }
    }

    public class AsyncCompleteArgs : EventArgs
    {
        public AsyncCompleteArgs(RobotEventArgs e, RobotCompletedAction rca, bool wasAborted)
        {
            EventArgs = e;
            CompletedAction = rca;
            WasAborted = wasAborted;
        }

        public RobotCompletedAction CompletedAction
        {
            get;
            private set;
        }

        public RobotEventArgs EventArgs
        {
            get;
            private set;
        }

        public bool WasAborted
        {
            get;
            private set;
        }
    }
}
