using System.Collections.Generic;
using System.Reflection;

namespace Regulus.BehaviourTree
{
    public interface ITicker
    {
        void  GetInfomation(ref List<Infomation> nodes);

        void Reset();
        TICKRESULT Tick(float delta);
    }

    public struct Infomation
    {
        internal static readonly Infomation Empty = new Infomation() {Tag =string.Empty};
        public string Tag;
    }
}