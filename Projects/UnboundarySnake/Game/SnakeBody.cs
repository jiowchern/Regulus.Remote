using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    using Extension;
    public class SnakeBody : Collider , ISnakeBodyItem
    {
        public static readonly Regulus.CustomType.Size Size = new CustomType.Size(10, 10);


        public DIRECTION Direction;
        CustomType.Point _Position;
        private SnakeBody snakeBody;
        CustomType.Point ISnakeBodyItem.Position
        {
            get { return _Position; }
        }

        public SnakeBody()
        {
            _Position = new CustomType.Point();
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
