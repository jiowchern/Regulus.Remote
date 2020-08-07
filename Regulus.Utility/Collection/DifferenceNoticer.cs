using System.Collections.Generic;
using System.Linq;
namespace Regulus.Collection
{
    public class DifferenceNoticer<T>
    {
        public delegate void MoveCallback(IEnumerable<T> instances);

        public event MoveCallback JoinEvent;
        public event MoveCallback LeaveEvent;
        private readonly List<T> _Controllers;

        private readonly IEqualityComparer<T> _EqualityComparer;

        private readonly System.Action<IEnumerable<T>, IEnumerable<T>> _Broadcast;

        public DifferenceNoticer()
        {
            _Controllers = new List<T>();
            _Broadcast = _DefaultBroadcast;
        }

        private void _DefaultBroadcast(IEnumerable<T> targets, IEnumerable<T> current)
        {
            _BroadcastJoin(targets.Except(current));
            _BroadcastLeft(current.Except(targets));
        }

        public DifferenceNoticer(IEqualityComparer<T> equality_comparer) : this()
        {
            _EqualityComparer = equality_comparer;

            _Broadcast = _EqualityBroadcast;
        }

        private void _EqualityBroadcast(IEnumerable<T> targets, IEnumerable<T> current)
        {
            _BroadcastJoin(targets.Except(current, _EqualityComparer));
            _BroadcastLeft(current.Except(targets, _EqualityComparer));
        }

        public void Set(IEnumerable<T> targets)
        {
            List<T> current = _Controllers;

            _Broadcast(targets, current);

            _Controllers.Clear();
            _Controllers.AddRange(targets);
        }

        private void _BroadcastLeft(IEnumerable<T> controllers)
        {
            if (LeaveEvent != null)
                LeaveEvent(controllers);
        }

        private void _BroadcastJoin(IEnumerable<T> controllers)
        {
            if (JoinEvent != null)
                JoinEvent(controllers);
        }

    }
}