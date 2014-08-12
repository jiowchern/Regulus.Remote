using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys.Session
{


    class MeetingRoom : Regulus.Utility.IUpdatable
    {        
        private string _Lession;
        private StuffTeacher _Teacher;
        private StuffStudent _Student;

        bool _Done;

        public MeetingRoom(string lession, StuffTeacher teacher, StuffStudent student)
        {
            // TODO: Complete member initialization
            this._Lession = lession;
            this._Teacher = teacher;
            this._Student = student;
        }

        bool Utility.IUpdatable.Update()
        {
            return _Done == false && _Teacher.Enable && _Student.Enable;
        }

        void Framework.ILaunched.Launch()
        {
            _Done = false;


            _Teacher.BeginLession(_Lession);
            _Student.BeginLession(_Lession);

            _Teacher.StudentSpeakEvent += _Student.TeacherSpeak;
            _Student.SpeakEvent += _Teacher.StudentSpeak;
            _Teacher.DoneEvent += _DoDone;
            _Teacher.NextEvent += _Student.Next;
            _Teacher.TextureEvent += _Student.Texture;
        }
        void _DoDone(Score score)
        {
            _Student.Done(score);
            _Done = true;
        }
        void Framework.ILaunched.Shutdown()
        {
            _Teacher.EndLession();
            _Student.EndLession();

            _Teacher.StudentSpeakEvent -= _Student.TeacherSpeak;
            _Student.SpeakEvent -= _Teacher.StudentSpeak;
            _Teacher.DoneEvent -= _DoDone;
            _Teacher.NextEvent -= _Student.Next;
            _Teacher.TextureEvent -= _Student.Texture;
        }

        public string Name { get { return _Teacher.Name;  } }

        internal void Leave(StuffTeacher teacher)
        {
            _Done = teacher.Name == _Teacher.Name;
        }

        internal void Leave(StuffStudent student)
        {
            _Done = student.Name == _Student.Name;
        }
    }
}
