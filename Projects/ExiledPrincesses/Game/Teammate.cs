using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    static class SkillExt
    {
        public static CombatSkill ToIndex(this Skill skill ,int idx )
        {
            return new CombatSkill() { Status = skill.IsConsumed() ? SkillStatus.Initiative :SkillStatus.Passive
                , Id = skill.Id , Index = idx};
        }
    }
    public interface ITeammate : IActor , ICombatController
    {       

        Skill.Effect[] GetActivitiesEffects(Team team , CommonSkillSet common_skill_set);

        void Take(CommonSkillSet common_skill_set);
        int AddHit(Strategy strategy);
        void Injury(int damage);
        int AddCombo(Strategy strategy);

        void SetSide(TeamSide side);
        void SetPlatoonNumber(int number);

        void SetBattleThinkTime(long tick, float time , bool idle);
        void AddBattleThinkTime(long tick, float time);
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
                _FlipEnableEvent(new CombatSkill[]{ skill.ToIndex(activities_sn)} );
            }
        }

        void _EnableSkill(int idle_sn)
        {
            if (_ActivitiesSkills.Count < 3)
            {
                Skill skill;
                if (_Out(_Idle , idle_sn, out skill))
                {
                    _RemoveIdleEvent(new CombatSkill[] { skill.ToIndex(idle_sn) });
                    int sn= _Add(_ActivitiesSkills, skill);
                    _AddIEnableEvent(new CombatSkill[] { skill.ToIndex(sn) });
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
                var newSkill = _Wait.Dequeue();
                
                var newSn = _Add(skills, newSkill);
                _AddIdleEvent(new CombatSkill[] { newSkill.ToIndex(newSn) });
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
            if (_RemoveEnableEvent != null)
                _RemoveEnableEvent( (from removeSkill in removSkills select _ActivitiesSkills[removeSkill].ToIndex(removeSkill)).ToArray() );
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
        private int _Add(Dictionary<int , Skill> skills , Skill skill)
        {
            skills.Add(++_SkillSn, skill);
            return _SkillSn;
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
            var combatSkills = from pair in skills select new CombatSkill() { Id = pair.Value.Id, Index = pair.Key, Status = pair.Value.IsConsumed() ? SkillStatus.Initiative : SkillStatus.Passive };
            return combatSkills;
        }

        Remoting.Value<CombatSkill[]> ICombatController.QueryIdleSkills()
        {
            var combatSkills = _QueryCombatSkill(_Idle);
            return combatSkills.ToArray();
        }



        TeamSide _Side;
        TeamSide IActor.Side
        {
            get { return _Side; }
        }


        int _PlatoonNo;
        int IActor.PlatoonNo
        {
            get { return _PlatoonNo; }
        }


        void ITeammate.SetSide(TeamSide side)
        {
            _Side = side;
        }

        void ITeammate.SetPlatoonNumber(int number)
        {
            _PlatoonNo = number;
            if (_ChangePlatoonNoEvent != null)
                _ChangePlatoonNoEvent(_PlatoonNo);
        }


        event Action<CombatSkill[]> _AddIdleEvent;
        event Action<CombatSkill[]> ICombatController.AddIdleEvent
        {
            add { _AddIdleEvent += value; }
            remove { _AddIdleEvent -= value; }
        }

        event Action<CombatSkill[]> _RemoveIdleEvent;
        event Action<CombatSkill[]> ICombatController.RemoveIdleEvent
        {
            add { _RemoveIdleEvent += value; }
            remove { _RemoveIdleEvent -= value; }
        }
        event Action<CombatSkill[]> _AddIEnableEvent;
        event Action<CombatSkill[]> ICombatController.AddIEnableEvent
        {
            add { _AddIEnableEvent += value; }
            remove { _AddIEnableEvent -= value; }
        }

        event Action<CombatSkill[]> _RemoveEnableEvent;
        event Action<CombatSkill[]> ICombatController.RemoveEnableEvent
        {
            add { _RemoveEnableEvent += value; }
            remove { _RemoveEnableEvent -= value; }
        }

        event Action<CombatSkill[]> _FlipEnableEvent;
        event Action<CombatSkill[]> ICombatController.FlipEnableEvent
        {
            add { _FlipEnableEvent += value;  }
            remove { _FlipEnableEvent -= value; }
        }


        int IActor.MaxHp
        {
            get { return _Ability.Hp; }
        }

      

        void ITeammate.SetBattleThinkTime(long tick, float time , bool idle)
        {
            _SetBattleThinkTimeEvent(tick, time, idle);
        }

        void ITeammate.AddBattleThinkTime(long tick, float time)
        {
            _AddBattleThinkTimeEvent(tick, time);
        }

        event Action<long, float , bool> _SetBattleThinkTimeEvent;
        event Action<long, float, bool> IActor.SetBattleThinkTimeEvent
        {
            add { _SetBattleThinkTimeEvent += value; }
            remove { _SetBattleThinkTimeEvent -= value; }
        }

        event Action<long, float> _AddBattleThinkTimeEvent;
        event Action<long, float> IActor.AddBattleThinkTimeEvent
        {
            add { _AddBattleThinkTimeEvent += value; }
            remove { _AddBattleThinkTimeEvent -= value; }
        }

        


        Remoting.Value<int> IActor.GetPlatoonNo()
        {
            return _PlatoonNo;
        }

        event Action<int> _ChangePlatoonNoEvent;
        event Action<int> IActor.ChangePlatoonNoEvent
        {
            add { _ChangePlatoonNoEvent += value; }
            remove { _ChangePlatoonNoEvent -= value; }
        }
    }

   
    
    class Adventurer
    {
        public string Map;
        public Regulus.Project.ExiledPrincesses.Contingent.FormationType Formation;
        public ITeammate[] Teammates;
        public IController Controller;
    }
}