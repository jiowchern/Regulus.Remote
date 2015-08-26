using System.Collections.Generic;
using System.Linq;
namespace Regulus.Collection
{
    public class DifferenceNoticer<T>
    {
        public delegate void MoveCallback(IEnumerable<T> instances);

        public event MoveCallback JoinEvent;
        public event MoveCallback LeftEvent;
        private readonly List<T> _Controllers;

        public DifferenceNoticer()
        {
            _Controllers = new List<T>();
        }
        public void Set(IEnumerable<T> targets)
        {
            var current = _Controllers;

            _BroadcastJoin(targets.Except(current));
            _BroadcastLeft(current.Except(targets));

            _Controllers.Clear();
            _Controllers.AddRange(targets);
        }

        private void _BroadcastLeft(IEnumerable<T> controllers)
        {
            if(LeftEvent != null)
                LeftEvent(controllers);
        }

        private void _BroadcastJoin(IEnumerable<T> controllers)
        {
            if (JoinEvent != null)
                JoinEvent(controllers);
        }
    }
}