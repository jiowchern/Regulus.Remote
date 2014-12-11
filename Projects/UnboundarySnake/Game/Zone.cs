using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    public class Zone : Regulus.Utility.IUpdatable
    {
        Map _Map;
        Controller _Controller;
        bool Utility.IUpdatable.Update()
        {
            IEnumerable<Snake> snakes = _FlushSnakes();

            foreach(var snake in snakes)
            {
                DIRECTION direction =  _Controller.GetDirection(snake);

                IEnumerable<SnakeBody> bodys = _UpdateBody(snake, direction);                

                foreach (SnakeBody body in bodys)
                {
                    _Map.RemoveCollider(body);
                    Collider predator = body;
                    Collider prey = _Map.FindCollider(predator);
                    CollisionResult result = predator.Collision(prey);
                    if (result == CollisionResult.Survival)
                    {
                        _Map.AddCollider(body);
                    }                    
                }                
            }
            return true;
        }

        private IEnumerable<Snake> _FlushSnakes()
        {
            throw new NotImplementedException();
        }

        private IEnumerable<SnakeBody> _UpdateBody(Snake snake, DIRECTION direction)
        {            
            return null;                        
        }

        private Vector _GetVector(DIRECTION direction)
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Launch()
        {
            throw new NotImplementedException();
        }

        void Framework.ILaunched.Shutdown()
        {
            throw new NotImplementedException();
        }

        internal void Join(Snake snake)
        {
            
        }

        internal void Left(Snake _Snake)
        {
            throw new NotImplementedException();
        }
    }
}
