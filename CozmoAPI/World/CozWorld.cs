using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CozmoAPI.World
{
    public class CozWorld
    {
        private CozConnection mConnection;

        public CozWorld(CozConnection connection)
        {
            mConnection = connection;
            mConnection.RobotEvent += OnRobotEvent;
        }

        private void OnRobotEvent(RobotEventArgs e)
        {
            
        }
    }
}
