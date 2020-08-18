using Regulus.Network;
using System;

namespace Regulus.Remote.Ghost
{
    public interface IAgent : INotifierQueryable
    {

        /// <summary>
        ///     Active
        /// </summary>
        bool Active { get; }
        /// <summary>
        ///     Ping
        /// </summary>
        long Ping { get; }


        /// <summary>
        /// 岿粇よ猭㊣
        /// 狦㊣よ猭把计Τ粇玥穦肚癟.
        /// ㄆン把计:
        ///     1.よ猭嘿
        ///     2.岿粇癟
        /// 穦祇ネ癟硄盽琌client籔serverセぃ甧┮璓.
        /// </summary>
        event Action<string, string> ErrorMethodEvent;


       


        void Start(IStreamable stream);
        void Stop();

        void Update();
    }
}