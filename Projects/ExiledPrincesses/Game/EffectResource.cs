using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    public class EffectResources : Regulus.Utility.Singleton<EffectResources>
    {
        Dictionary<int, Skill.Effect> _Effects;

        public EffectResources()
        {
            _Effects = new Dictionary<int, Skill.Effect>();
            _Add(_Effects, 1, 5.0f, new int[] { 3, 0, 0, 0 }, _SwordAttack1);
            _Add(_Effects, 2, 5.0f, new int[] { 0, 3, 0, 0 }, _AxAttack1);
            _Add(_Effects, 3, 5.0f, new int[] { 0, 0, 3, 0 }, _StaffAttack1);
            _Add(_Effects, 4, 5.0f, new int[] { 0, 0, 0, 3 }, _ShieldAttack1);

            _Add(_Effects, 5, 1.0f, new int[] { 0, 0, 0, 0 }, _SwordRecovery1);
            _Add(_Effects, 6, 1.0f, new int[] { 0, 0, 0, 0 }, _AxRecovery1);
            _Add(_Effects, 7, 1.0f, new int[] { 0, 0, 0, 0 }, _StaffRecovery1);
            _Add(_Effects, 8, 1.0f, new int[] { 0, 0, 0, 0 }, _ShieldRecovery1);

            _Add(_Effects, 9, 5.0f, new int[] { 10, 0, 0, 0 }, _SwordAttack2);
            _Add(_Effects, 10, 5.0f, new int[] { 0, 10, 0, 0 }, _AxAttack2);
            _Add(_Effects, 11, 5.0f, new int[] { 0, 0, 10, 0 }, _StaffAttack2);
            _Add(_Effects, 12, 5.0f, new int[] { 0, 0, 0, 10 }, _ShieldAttack2);
        }

        private void _ShieldAttack2(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach(var target in targets)
                    _BaseInjury(target, Strategy.Shield, 30 / targets.Length);
            }
        }

        private void _StaffAttack2(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Staff, 30 / targets.Length);
            }
        }

        private void _AxAttack2(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Ax, 30 / targets.Length);
            }
        }

        private void _SwordAttack2(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Sword, 30 / targets.Length);
            }
        }

        

        private void _ShieldRecovery1(Team.Member member, Team[] target_teams)
        {
            _BaseRecovery(member, Strategy.Shield, 1);
        }

        private void _StaffRecovery1(Team.Member member, Team[] target_teams)
        {
            _BaseRecovery(member, Strategy.Staff, 1);
        }

        private void _AxRecovery1(Team.Member member, Team[] target_teams)
        {
            _BaseRecovery(member, Strategy.Ax, 1);
        }

        private void _SwordRecovery1(Team.Member member, Team[] target_teams)
        {
            _BaseRecovery(member, Strategy.Sword, 1);
        }

        private void _ShieldAttack1(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Shield, 10 / targets.Length);
            }
        }

        private void _StaffAttack1(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Staff, 10 / targets.Length);
            }
        }

        private void _AxAttack1(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Ax, 10 / targets.Length);
            }
        }

        private void _SwordAttack1(Team.Member member, Team[] target_teams)
        {
            foreach (var targets in _GetTargetGroup(target_teams))
            {
                foreach (var target in targets)
                    _BaseInjury(target, Strategy.Sword, 10 / targets.Length);
            }
        }

        private void _BaseRecovery(Team.Member member, Strategy strategy, int amount)
        {
            member.Owner.AddStrategy(strategy, amount);
        }

        private void _BaseInjury(ITeammate target, Strategy strategy, int damage)
        {
            double hit = (double)target.AddHit(strategy);
            hit = hit > 10 ? 10 : hit;
            double combo = (double)target.AddCombo(strategy);
            combo = combo > 10 ? combo : 10;
            float trauma = (float)(1 / Math.Log(10 / (combo / 10 * 6 + hit / 10* 3 + 1)));
            target.Injury((int)(trauma * damage) + damage);
        }

        IEnumerable<ITeammate> _GetTarget(Team[] target_teams)
        {
            foreach (var targetTeam in target_teams)
            {
                var targets = targetTeam.GetFront();
                foreach (var target in targets)
                {
                    yield return target;
                }
            }
        }

        IEnumerable<ITeammate[]> _GetTargetGroup(Team[] target_teams)
        {
            foreach (var targetTeam in target_teams)
            {
                yield return targetTeam.GetFront();                
            }
        }

        private void _Add(Dictionary<int, Skill.Effect> effects, int id, float duration, int[] demand, Skill.Effect.OnAction action)
        {
            effects.Add(id, new Skill.Effect(duration, demand, action));
        }

        internal Skill.Effect Find(int id)
        {
            return _Effects[id];
        }
    }
}
