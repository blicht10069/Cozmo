using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CozmoAPI
{
    public class TaskQueue : IDisposable
    {
        private Queue<ITask> mStack = new Queue<ITask>();
        private Thread mStackProcessorThread;
        private AutoResetEvent mReset = new AutoResetEvent(false);
        private object mSync = new object();
        private ITask mCurrentTask = null;
        private CozConnection mSourceConnection;
        private CozConnection mNullConnection;
        private bool mAbortAllTaskMode = false;

        public TaskQueue(CozConnection source)
        {
            IsAborted = false;
            mSourceConnection = source.Clone();
            //mSourceConnection = source;
            mNullConnection = mSourceConnection.CreateNullConnection();
            mStackProcessorThread = new Thread(RunStackProcessor);
            mStackProcessorThread.IsBackground = true;
            mStackProcessorThread.Start();
        }

        public void Dispose()
        {
            if (mStackProcessorThread != null)
            {
                AbortCurrentTask();
                if (mStackProcessorThread.IsAlive)
                    try
                    {
                        mStackProcessorThread.Abort();
                    }
                    catch { };
                mStackProcessorThread = null;
            }
        }

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
                    Console.WriteLine("Error In Task Stack: " + ex.ToString());
                }
            }
        }

        public void AbortAllTasks()
        {
            mAbortAllTaskMode = true;
            AbortCurrentTask();
        }

        public void AbortCurrentTask()
        {
            if (!IsAborted)
            {
                IsAborted = true;
                if (!mSourceConnection.LastAction.Result.IsComplete)
                    mSourceConnection.AbortByIdTag(mSourceConnection.LastAction.QueueSingleAction.IdTag);
            }
        }

        public bool IsAborted
        {
            get;
            private set;
        }

        public ITask CurrentTask
        {
            get { return mCurrentTask; }
        }

        public CozConnection Connection
        {
            get
            {
                if (IsAborted)
                    return mNullConnection;
                else
                    return mSourceConnection;
            }
        }

        public void Push(Action<TaskEventArgs> code)
        {
            Push(0, String.Empty, code);
        }
        public void Push(int id = 0, string name = "", Action<TaskEventArgs> code = null)
        {
            Push(new Task(id, name, code));
        }

        public void Push(ITask task)
        {
            lock (mSync)
            {
                mStack.Enqueue(task);
            }
            mReset.Set();

        }

        private void RunInternal()
        {
            if (mReset.WaitOne(TimeSpan.FromSeconds(10)))
            {
                ITask task = null;
                int count = 1;
                TaskEventArgs arg = new TaskEventArgs(this);
                while (count > 0)
                {
                    lock (mSync)
                    {
                        task = null;
                        count = mStack.Count;
                        if (count == 0) break;
                        if (mStack.Count > 0)
                            task = mStack.Dequeue();
                        mCurrentTask = task;
                    }
                    if (!mAbortAllTaskMode) IsAborted = false;
                    task.Execute(arg);
                }
            }
        }


    }

    public class TaskStack : TaskQueue
    {
        public TaskStack(CozConnection source)
            : base(source)
        {
        }
    }

    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(TaskQueue stack)
        {
            Stack = stack;
        }

        public TaskQueue Stack
        {
            get;
            private set;
        }
    }

    public interface ITask
    {
        string Name { get; }
        int Id { get; }
        void Execute(TaskEventArgs e);
    }

    public abstract class TaskBase : ITask
    {

        public TaskBase()
        {
        }

        public string Name
        {
            get;
            protected set;
        }

        public int Id
        {
            get;
            protected set;
        }

        protected abstract void DoTask(TaskEventArgs e);

        public void Execute(TaskEventArgs e)
        {
            DoTask(e);
        }
    }

    public class Task : TaskBase
    {
        public Task(int id = 0, string name = "", Action<TaskEventArgs> code = null)
        {
            Id = id;
            Name = name;
            Code = code;
        }

        protected override void DoTask(TaskEventArgs e)
        {
            Code(e);
        }

        public Action<TaskEventArgs> Code
        {
            get;
            set;
        }

    }
}
