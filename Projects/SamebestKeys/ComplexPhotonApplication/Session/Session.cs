using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{
    class Session : Regulus.Utility.IUpdatable
    {
        List<StuffTeacher> _Teachers;
        List<StuffStudent> _Students;

        Regulus.Utility.TUpdater<MeetingRoom> _Rooms;
        
        internal void Join(StuffStudent student)
        {
            _Students.Add(student);
        }

        internal void Join(StuffTeacher teacher)
        {
            _Teachers.Add(teacher);
        }

        internal void Leave(StuffStudent student)
        {
            _Students.Remove(student);
            _RoomClose(_Rooms.Objects , student);
        }

        internal void Leave(StuffTeacher teacher)
        {
            _Teachers.Remove(teacher);
            _RoomClose(_Rooms.Objects ,teacher);
        }

        private void _RoomClose(MeetingRoom[] meetingrooms, StuffTeacher teacher)
        {
            foreach (var room in meetingrooms)
            {
                room.Leave(teacher);
            }
        }

        private void _RoomClose(MeetingRoom[] meetingrooms, StuffStudent student)
        {
            foreach (var room in meetingrooms)
            {
                room.Leave(student);
            }
        }       

        bool Utility.IUpdatable.Update()
        {            
            foreach(var student in  _Students)
            {
                var requester = student.PopRequester();
                if (requester == null)
                {
                    continue;                    
                }

                StuffTeacher teacher = _FindTeacher(requester.Name);
                if (teacher == null)
                {
                    student.NoTeacher();
                    continue;
                }
                if (CheckBusy(teacher.Name) == false)
                {
                    student.TeacherBusy();
                    continue;
                }
                if (student.CheckCoin(requester.Lession) == false)
                {
                    student.NoCoin();
                    continue;
                }

                Remoting.Value<bool> result = teacher.RequestOpenRoom(requester.Answer);
                result.OnValue += (respond) =>
                {                    

                    if (respond)
                    {
                        _CreateRoom(requester.Lession, teacher, student);
                    }
                    else
                        student.TeacherBusy();

                    
                };
                    
            }

            _RoomUpdate();
            return true;
        }

        

        private void _RoomUpdate()
        {
            _Rooms.Update();            
        }

        private void _CreateRoom(string lession, StuffTeacher teacher, StuffStudent student)
        {
            var meeting = new MeetingRoom(lession, teacher, student);
            _Rooms.Add(meeting);
        }

        private bool CheckBusy(string teacher_name)
        {
            return (from r in _Rooms.Objects where r.Name == teacher_name select true).SingleOrDefault() == false;            
        }

        private StuffTeacher _FindTeacher(string teacher_name)
        {
            return (from t in _Teachers where t.Name == teacher_name select t).SingleOrDefault();            
        }

        private StuffStudent _FindStudent(string name)
        {
            return (from s in _Students where s.Name == name select s).SingleOrDefault();            
        }

        void Framework.ILaunched.Launch()
        {
            _Teachers = new List<StuffTeacher>();
            _Students = new List<StuffStudent>();
            _Rooms = new Utility.TUpdater<MeetingRoom>();
        }

        void Framework.ILaunched.Shutdown()
        {
            
        }
    }
}
