using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    
    public interface ITeammate : IActor , ICombatController
    {       

        Skill.Effect[] GetActivitiesEffects(Team team , CommonSkillSet common_skill_set);

        void Take(CommonSkillSet common_skill_set);
        int AddHit(Strategy strategy);
        void Injury(int damage);
        int AddCombo(Strategy strategy);        
    }

    public class Teammate : ITeammate
    {        
        Dictionary<int , Skill> _Idle;
        Dictionary<int, Skill> _ActivitiesSkills;

        Queue<Skill> _Wait;
        List<Skill> _Recover;
        
        Ability _Ability;
        int _Hp;
        Strategy _Specializes ;
        Strategy _Weakness ;
        
        int _Prototype;
        public Teammate(ActorInfomation actor)
        {
            
            _Prototype = actor.Prototype;
            var prototypeActor = ActorResource.Instance.Find(actor.Prototype);
            _ActivitiesSkills = new Dictionary<int, Skill>();
            _Ability = prototypeActor.FindAbility(actor.Exp);
            _Hp = _Ability.Hp;
            _Specializes = prototypeActor.Specializes;
            _Weakness = prototypeActor.Weakness;
            _Hit = 0;
            _Combo = 0;
        }

        int IActor.Dex
        {
            get { return _Ability.Dex; }
        }

        Strategy IActor.Specializes
        {
            get { return _Specializes; }
        }

        int IActor.Int
        {
            get { return _Ability.Int; }
        }

        void _FlipSkill(int activities_sn)
        { 
            Skill skill;
            if (_ActivitiesSkills.TryGetValue(activities_sn, out skill))
            {
                skill.Flip();
            }
        }

        void _EnableSkill(int idle_sn)
        {
            if (_ActivitiesSkills.Count < 2)
            {
                Skill skill;
                if (_Out(_Idle , idle_sn, out skill))
                {
                    _Add(_ActivitiesSkills, skill);
                    
                }
            }
            
        }

        private bool _Out(Dictionary<int, Skill> skills, int sn, out Skill skill)
        {
            if (skills.TryGetValue(sn, out skill))
            {
                skills.Remove(sn);
                if (_Wait.Count == 0)
                {
                    var recover = _Recover.ToArray();
                    Skill.Shuffle(recover);
                    _Wait = new Queue<Skill>(recover);
                    _Recover.Clear();
                }
                _Add(skills, _Wait.Dequeue());
                return true;
            }
            return false;
        }

        Skill.Effect[] ITeammate.GetActivitiesEffects(Team team, CommonSkillSet common_skill_set)
        {
            List<Skill.Effect> effects = new List<Skill.Effect>();
            List<int> removSkills = new List<int>();

            foreach (var pair in _ActivitiesSkills)
            {
                var skill = pair.Value;
                Skill.Effect effect = skill.GetActivitiesEffect();
                if (team.Consumer(effect.Demand))
                {
                    effects.Add(effect);
                    if (skill.IsConsumed())
                    {
                        removSkills.Add(pair.Key);
                        _Recover.Add(skill);
                        _Recover.Add(common_skill_set.Licensing());
                    }                    
                }
            }

            foreach(var skill in removSkills)
            {
                _ActivitiesSkills.Remove(skill);
            }
                        
            return effects.ToArray();
        }

        int IActor.Hp
        {
            get { return _Hp; }
        }

        int _Hit;
        int ITeammate.AddHit(Strategy strategy)
        {
            if (strategy == _Weakness)
            {
                _Hit++;
            }           
            return _Hit;
        }

        void ITeammate.Injury(int damage)
        {
            _Hp -= damage;
        }

        int _Combo;
        int ITeammate.AddCombo(Strategy strategy)
        {
            if (strategy == _Weakness)
            {
                _Combo++;
            }
            else
            {
                _Combo = 0;
            }
            return _Combo;
        }


        void ITeammate.Take(CommonSkillSet common_skill_set)
        {
            _Idle = new Dictionary<int, Skill>();
            _Wait = new Queue<Skill>();
            _Recover = new List<Skill>();
            
            foreach (var skill in _Ability.Skills)
            {
                _Add(_Idle , SkillResources.Instance.Find(skill));
            }

            var needCount = 10 - _Ability.Skills.Length;
            for (int i = 0; i < needCount; ++i)
            {
                _Add(_Idle , common_skill_set.Licensing());
            }
            for (int i = 0; i < 20; ++i)
            {
                _Wait.Enqueue(common_skill_set.Licensing());
            }
        }

        int _SkillSn;
        private void _Add(Dictionary<int , Skill> skills , Skill skill)
        {
            skills.Add(++_SkillSn, skill);
        }

        

        int IActor.Pretotype
        {
            get { return _Prototype; }
        }


        void ICombatController.FlipSkill(int activities_sn)
        {
            _FlipSkill(activities_sn);
        }

        void ICombatController.EnableSkill(int idle_sn)
        {
            _EnableSkill(idle_sn);
        }

        Remoting.Value<CombatSkill[]> ICombatController.QueryEnableSkills()
        {
            var combatSkills = _QueryCombatSkill(_ActivitiesSkills);
            return combatSkills.ToArray();
        }

        private IEnumerable<CombatSkill> _QueryCombatSkill(Dictionary<int , Skill> skills)
        {
            var combatSkills = from pair in skills select new CombatSkill() { Id = pair.Value.Id, Index = pair.Key };
            return combatSkills;
        }

        Remoting.Value<CombatSkill[]> ICombatController.QueryIdleSkills()
        {
            var combatSkills = _QueryCombatSkill(_Idle);
            return combatSkills.ToArray();
        }


        
    }

   
    
    class Adventurer
    {
        public string Map;
        public Regulus.Project.ExiledPrincesses.Contingent.FormationType Formation;
        public ITeammate[] Teammates;
        public PlayerController Controller;
    }
}