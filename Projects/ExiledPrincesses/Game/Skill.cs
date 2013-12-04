using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{

    public class SkillResources : Regulus.Utility.Singleton<SkillResources>
    {
        Dictionary<int, Skill> _Skills;
        public SkillResources()
        {
            _Skills = new Dictionary<int, Skill>();
            _Add(_Skills, 1, 1, 5);
            _Add(_Skills, 2, 2, 6);
            _Add(_Skills, 3, 3, 7);
            _Add(_Skills, 4, 4, 8);
            _Add(_Skills, 5, 9, 5);
            _Add(_Skills, 6, 10, 6);
            _Add(_Skills, 7, 11, 7);
            _Add(_Skills, 8, 12, 8);
        }

        private void _Add(Dictionary<int, Skill> skills, int id, int effect1, int effect2)
        {
            skills.Add(id, new Skill(id , EffectResources.Instance.Find(effect1), EffectResources.Instance.Find(effect2)));
        }
        public Skill Find(int id)
        {
            return _Skills[id];
        }

        

    }
   

    public class Skill
    {
        public class Effect
        {
            public delegate void OnAction(Team.Member member, Team[] target_teams);
            OnAction _Action;
            public Effect(float duration , int[] demand , OnAction action)
            {
                Duration = duration;
                Demand = demand;
                _Action = action;
            }

            public float Duration { get; private set; }
            internal void Action(Team.Member member, Team[] target)
            {
                _Action(member, target);
            }
            public int[] Demand { get; private set; }
        }

        bool _Consumed;
        public void Flip()
        {
            _Consumed = !_Consumed;
        }
        internal bool IsConsumed()
        {
            return _Consumed;
        }
        Effect _Effect1;
        Effect _Effect2;        

        public Skill(int id ,Effect effect1, Effect effect2)
        {
            Id = id;
            this._Effect1 = effect1;
            this._Effect2 = effect2;
        }
        internal Effect GetActivitiesEffect()
        {
            if (_Consumed)
            {
                return _Effect1;
            }
            return _Effect2;
        }

        public static void Shuffle(Skill[] skills)
        {
            for (int i = 0; i < skills.Length; ++i)
            {
                var tmp = skills[i];
                var idx = Regulus.Utility.Random.Next(0, skills.Length);
                skills[i] = skills[idx];
                skills[idx] = tmp;
            }
        }

        public int Id { get; private set; }
    }

    public class CommonSkillSet
    {
        Queue<Skill> _Skills;

        public delegate void OnEmpty();
        public event OnEmpty EmptyEvent;
        public CommonSkillSet()
        {
            _Skills = new Queue<Skill>();
            for (int i = 0; i < 100; ++i )
            {
                _Skills.Enqueue(SkillResources.Instance.Find(1));
                _Skills.Enqueue(SkillResources.Instance.Find(2));
                _Skills.Enqueue(SkillResources.Instance.Find(3));
                _Skills.Enqueue(SkillResources.Instance.Find(4));
            }
        }
        public void Shuffle()
        {
            var skills = _Skills.ToArray();
            Skill.Shuffle(skills);
            _Skills = new Queue<Skill>(skills);
        }

        
        public Skill Licensing()
        {
            if (_Skills.Count > 0)
                return _Skills.Dequeue();
            EmptyEvent();
            return null;
        }
    }
    
}
