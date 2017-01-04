using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CozmoAPI
{
    public class MessageStack
    {
        private Stack<StackedItem> mStack = new Stack<StackedItem>();
        private Thread mStackProcessorThread;
        private AutoResetEvent mReset = new AutoResetEvent(false);
        private object mSync = new object();

        public MessageStack()
        {
            mStackProcessorThread = new Thread(RunStackProcessor);
            mStackProcessorThread.IsBackground = true;
            mStackProcessorThread.Start();
        }

        public static readonly MessageStack Default = new MessageStack();

        private void RunStackProcessor(object obj)
        {
            while (true)
            {
                try
                {
                    RunInternal();
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error In Message Stack: " + ex.ToString());
                }
            }
        }

        public void Push(Action<object> action)
        {
            Push(null, action);
        }
        
        public void Push(object state, Action<object> action)
        {
            lock (mSync)
            {
                mStack.Push(new StackedItem(action, state));
            }
            mReset.Set();
        }

        private void RunInternal()
        {
            if (mReset.WaitOne(TimeSpan.FromSeconds(10)))
            {
                StackedItem action = null;
                int count = 1;
                while (count > 0)
                {
                    lock (mSync)
                    {
                        action = null;
                        count = mStack.Count;
                        if (count == 0) break;
                        if (mStack.Count > 0)
                            action = mStack.Pop();
                    }
                    if (action != null)
                        action.Action(action.State);
                }
            }
        }

        private class StackedItem
        {
            public StackedItem(Action<object> action, object state)
            {
                Action = action;
                State = state;
            }

            public Action<object> Action
            {
                get;
                private set;
            }

            public object State
            {
                get;
                private set;
            }
        }
    }
}
