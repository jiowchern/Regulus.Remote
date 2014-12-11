using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    using Extension;
    public class SnakeBody : Collider , ISnakeBodyItem
    {
        public static readonly Regulus.Types.Size Size = new Types.Size(10, 10);


        public DIRECTION Direction;
        Types.Point _Position;
        private SnakeBody snakeBody;
        Types.Point ISnakeBodyItem.Position
        {
            get { return _Position; }
        }

        public SnakeBody()
        {
            _Position = new Types.Point();
        }

        public SnakeBody(SnakeBody snakeBody) 
        {
            _Position = snakeBody._Position;
        }

        internal void Move()
        {
            var vector = Direction.ToVector();
            
            

            _Position.X += vector.X;
            _Position.Y += vector.Y;
        }
    }

    
}
