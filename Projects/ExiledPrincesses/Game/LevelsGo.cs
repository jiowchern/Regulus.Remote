using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.ExiledPrincesses.Game
{
    partial class Levels
    {
        class GoForwardStage : Regulus.Game.IStage, IAdventureGo
        {
            Station _Station;
            private float _Position;
            const float _DistancePerSeconds = 140.0f;

            public delegate void OnArrival(float position, Station station);
            public event OnArrival ArrivalEvent;

            Regulus.Standalong.Agent _Agent;
            Platoon _Platoon;

            public GoForwardStage(float position, Station station, Platoon platoon)
            {
                _Station = station;
                this._Position = position;
                _Platoon = platoon;

                _Agent = new Standalong.Agent();

            }

            void Regulus.Game.IStage.Enter()
            {
                _Agent.Launch();
                _Agent.Bind<IAdventureGo>(this);
                var ghost = _Agent.QueryProvider<IAdventureGo>().Ghosts[0];
                _Platoon.Go(ghost);

                _ForwardEvent(LocalTime.Instance.Ticks, _Position, _DistancePerSeconds);

            }

            void Regulus.Game.IStage.Leave()
            {

                _ForwardEvent(LocalTime.Instance.Ticks, _Position, 0);
                _Platoon.Stop();
                _Agent.Unbind<IAdventureGo>(this);

                _Agent.Shutdown();
            }

            void Regulus.Game.IStage.Update()
            {
                _Agent.Update();
                _Position += (_DistancePerSeconds * LocalTime.Instance.DeltaSecond);
                if (_Position > _Station.Position)
                {
                    ArrivalEvent(_Position, _Station);
                }
            }

            event Action<long /*time_tick*/ , float /*position*/ , float /*speed*/> _ForwardEvent;
            event Action<long /*time_tick*/ , float /*position*/ , float /*speed*/> IAdventureGo.ForwardEvent
            {
                add { _ForwardEvent += value; }
                remove { _ForwardEvent -= value; }
            }

            Station IAdventureGo.Site
            {
                get { return _Station; }
            }
        }
    }
}
