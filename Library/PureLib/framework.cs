using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samebest.Game    
{
    public interface IFramework
    {
        void Launch();
        bool Update();
        void Shutdown();
    }

    public class FrameworkRoot  : IDisposable
    {
        Queue<Samebest.Game.IFramework> _AddFrameworks = new Queue<Game.IFramework>();
        Queue<Samebest.Game.IFramework> _RemoveFramework = new Queue<Game.IFramework>();

        List<Samebest.Game.IFramework> _Frameworks = new List<Game.IFramework>();

        public void AddFramework(Samebest.Game.IFramework framework)
        {
            _AddFrameworks.Enqueue(framework);            
        }

        public void RemoveFramework(Samebest.Game.IFramework framework)
        {
            
            _RemoveFramework.Enqueue(framework);
        }

        public void Update()
        {
                        
            _Add(_AddFrameworks, _Frameworks);
                        
            _Remove(_RemoveFramework, _Frameworks);

            _Update();
        }

        private void _Remove(Queue<Game.IFramework> remove_framework, List<Game.IFramework> frameworks)
        {
            while (remove_framework.Count > 0)
            {
                var fw = remove_framework.Dequeue();
                frameworks.Remove(fw);
                fw.Shutdown();
            }                        
        }

        private void _Add(Queue<Game.IFramework> add_frameworks, List<Game.IFramework> frameworks)
        {
            while (add_frameworks.Count > 0)
            {
                var fw = add_frameworks.Dequeue();
                frameworks.Add(fw);
                fw.Launch();
            }                        
        }

        private void _Update()
        {
            Queue<Samebest.Game.IFramework> removeFrameworks = new Queue<IFramework>();
            
            foreach (var framework in _Frameworks)
            {
                if (framework.Update() == false)
                {
                    removeFrameworks.Enqueue(framework);
                }                    
            }

            foreach(var removeFramework in removeFrameworks)
                RemoveFramework(removeFramework);
        }

        

        private void _Shutdown(List<Game.IFramework> frameworks)
        {
            
            foreach (var framework in frameworks)
            {
                framework.Shutdown();
            }
        }

        void IDisposable.Dispose()
        {
            _Shutdown(_Frameworks);
            _Frameworks.Clear();
        }
    }
}
