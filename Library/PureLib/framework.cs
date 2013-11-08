using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Framework
{
    public interface ILaunched
    {
        void Launch();
        void Shutdown();
    }
}
namespace Regulus.Game    
{
    

    public interface IFramework 
    {
        bool Update();
        void Launch();
        void Shutdown();
    }

    public class FrameworkRoot  
    {
        Queue<Regulus.Game.IFramework> _AddFrameworks = new Queue<Game.IFramework>();
        Queue<Regulus.Game.IFramework> _RemoveFramework = new Queue<Game.IFramework>();

        List<Regulus.Game.IFramework> _Frameworks = new List<Game.IFramework>();

        public void AddFramework(Regulus.Game.IFramework framework)
        {
            _AddFrameworks.Enqueue(framework);            
        }

        public void RemoveFramework(Regulus.Game.IFramework framework)
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
            foreach (var framework in _Frameworks)
            {
                if (framework.Update() == false)
                {
                    
                    RemoveFramework(framework);
                }                    
            }
            
        }

        private void _Shutdown(List<Game.IFramework> frameworks)
        {
            
            foreach (var framework in frameworks)
            {
                framework.Shutdown();
            }
        }

        public void Shutdown()
        {
            _Shutdown(_Frameworks);
            _Frameworks.Clear();
        }

        
    }
}
