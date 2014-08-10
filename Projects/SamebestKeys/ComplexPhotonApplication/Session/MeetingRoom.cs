using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{
    class MeetingRoom : Regulus.Utility.IUpdatable
    {
        Staff _Leader;
        Staff _Member;

        public void Join(Staff member)
        {
            
        }

        public MeetingRoom(Staff leader)
        {
            
        }

        bool Utility.IUpdatable.Update()
        {
            return _CheckLeader();
        }

        private bool _CheckLeader()
        {
            return _Leader != null;
        }

        void Framework.ILaunched.Launch()
        {
            
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
