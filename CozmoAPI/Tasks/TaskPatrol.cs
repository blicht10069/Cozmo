using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozmoAPI.CozDataStructures;

namespace CozmoAPI.Tasks
{
    public class TaskPatrol : TaskBase
    {
        public TaskPatrol(int id = 0, string name = "Patrol")
            : base()
        {
            Id = id;
            Name = name;
            Points = new CozPointCollection();
        }

        public CozPointCollection Points
        {
            get;
            set;
        }

        protected override void DoTask(TaskEventArgs e)
        {
            if (Points == null) return;
            CozPoint current = new CozPoint() { X = 0f, Y = 0f };
            while (!e.Stack.IsAborted)
            {
                foreach (CozPoint pt in Points)
                {
                    double x1 = (double)(current.X - pt.X);
                    double y1 = (double)(current.Y - pt.Y);
                    double h = Math.Sqrt(Math.Pow(x1, 2) + Math.Pow(y1, 2));
                    double o = x1 > y1 ? y1 : x1;
                    double theta = Math.Sin(o / h);
                    Console.WriteLine("Moving to point {0:N2}, {1:N2}", pt.X, pt.Y);
                    e.Stack.Connection.MoveToPosition(pt.X, pt.Y, (float)theta).Wait();
                    e.Stack.Connection.PanAndTilt(Utilities.ToRadians(45)).Wait();
                }
            }
        }
    }

    
}
