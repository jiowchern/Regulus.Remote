using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    public class Snake
    {

        System.Collections.Generic.Queue<SnakeBody> _Waits;
        System.Collections.Generic.LinkedList<SnakeBody> _Bodys;

        public int TotalLength { get { return _Bodys.Count + _Waits.Count ; } }

        public Snake()
        {
            _Waits = new Queue<SnakeBody>();
            _Bodys = new LinkedList<SnakeBody>();
            _Bodys.AddLast(new SnakeBody());
        }

        private void _GrowUp(int length)
        {
            for (int i = 0; i < length; ++i )
            {
                _Waits.Enqueue(new SnakeBody(_Bodys.Last.Value));
            }                
        }

        public void Move(DIRECTION direction)
        {
            _Move(_Bodys.First, direction);            
        }

        
        private void _Move(LinkedListNode<SnakeBody> first)
        {
            var current = first;
            do
            {
                var body = current.Value;
                body.Move();

                current = current.Next;
            }
            while (current != null);
        }

        private void _ConveyCommand(LinkedListNode<SnakeBody> last, DIRECTION direction)
        {
            var current = last;
            while (current.Previous != null)
            {
                var prev = current.Previous.Value;
                current.Value.Direction = prev.Direction;
                current = current.Previous;
            }
        }

        public IEnumerable<ISnakeBodyItem> GetBody()
        {
            return from body in _Bodys                   
                   select body as ISnakeBodyItem;
        }

        public void GrowUp(int count)
        {
            _GrowUp(count);
        }

        public void Break(ISnakeBodyItem item)
        {
            var body = (from b in _Bodys                        
                        where b == item
                        select b).SingleOrDefault();

            if(body != null)
            {
                var node = _Bodys.Find(body);
                if (node.Next != null)
                {
                    _Move(node.Next, body.Direction);
                }                
                _Bodys.Remove(node);                
            }
        }

        private void _Move(LinkedListNode<SnakeBody> node, DIRECTION direction)
        {
            _ConveyCommand(_GetTail(node), direction);
            _SetCommand(node, direction);
            _Move(node);
            _AddTail();
        }

       
        private void _AddTail()
        {
            if (_Waits.Count > 0)
            {
                _Bodys.AddLast(_Waits.Dequeue());
            }                
        }

        private static DIRECTION _SetCommand(LinkedListNode<SnakeBody> node, DIRECTION direction)
        {
            return node.Value.Direction = direction;
        }

        private LinkedListNode<SnakeBody> _GetTail(LinkedListNode<SnakeBody> head)
        {
            var node  = head;
            while(node.Next != null)
            {
                node = node.Next;
            }
            return node;
        }
    }
}
