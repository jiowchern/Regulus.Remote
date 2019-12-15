using System;
using System.Collections.Generic;

namespace Regulus.Game
{
    internal class ComponmentSet
    {
        private readonly List<IComponment> _Componments;
        private readonly List<IComponment> _ComponmentUpdates;
        private readonly Queue<IComponment> _ComponmentAdds;
        private readonly Queue<IComponment> _ComponmentRemoves;

        public ComponmentSet()
        {
            _Componments = new List<IComponment>();
            _ComponmentUpdates = new List<IComponment>();
            _ComponmentAdds = new Queue<IComponment>();
            _ComponmentRemoves = new Queue<IComponment>();
        }
        public void Add<T>(T componment) where T : IComponment
        {
            _Componments.Add(componment);
            _ComponmentAdds.Enqueue(componment);
        }
            
        public IEnumerable<T> Get<T>() where T : IComponment
        {
            foreach (var componment in _Componments.ToArray())
            {
                yield return (T)componment ;
            }
        }

        public void Remove<T>(T componment) where T : IComponment
        {
            _Componments.Remove(componment);
            _ComponmentRemoves.Enqueue(componment);
        }

        public void Update()
        {
            while (_ComponmentAdds.Count > 0)
            {
                var componment = _ComponmentAdds.Dequeue();
                componment.Start();
                _ComponmentUpdates.Add(componment);
            }

            while (_ComponmentRemoves.Count > 0)
            {
                var componment = _ComponmentRemoves.Dequeue();
                if (_ComponmentUpdates.Remove(componment))
                {
                    componment.End();
                }
            }

            foreach (var componmentUpdate in _ComponmentUpdates)
            {
                componmentUpdate.Update();
            }
        }

        public void Clear()
        {
            foreach (var componmentUpdate in _ComponmentUpdates)
            {
                componmentUpdate.End();
            }

            _ComponmentUpdates.Clear();
        }
    }
}