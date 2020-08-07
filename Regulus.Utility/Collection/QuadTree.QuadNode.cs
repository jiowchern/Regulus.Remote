using Regulus.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Regulus.Collection
{
    public partial class QuadTree<T> where T : class, IQuadObject
    {
        public class QuadNode
        {
            private static int _id;

            private readonly QuadNode[] _nodes = new QuadNode[4];

            public readonly int ID = QuadNode._id++;

            internal List<T> QuadObjects = new List<T>();

            public ReadOnlyCollection<QuadNode> Nodes { get; set; }

            public ReadOnlyCollection<T> Objects { get; set; }

            public QuadNode Parent { get; internal set; }

            public QuadNode this[QuadtreeDirection direction]
            {
                get
                {
                    switch (direction)
                    {
                        case QuadtreeDirection.NW:
                            return _nodes[0];
                        case QuadtreeDirection.NE:
                            return _nodes[1];
                        case QuadtreeDirection.SW:
                            return _nodes[2];
                        case QuadtreeDirection.SE:
                            return _nodes[3];
                        default:
                            return null;
                    }
                }

                set
                {
                    switch (direction)
                    {
                        case QuadtreeDirection.NW:
                            _nodes[0] = value;
                            break;
                        case QuadtreeDirection.NE:
                            _nodes[1] = value;
                            break;
                        case QuadtreeDirection.SW:
                            _nodes[2] = value;
                            break;
                        case QuadtreeDirection.SE:
                            _nodes[3] = value;
                            break;
                    }

                    if (value != null)
                    {
                        value.Parent = this;
                    }
                }
            }

            public Rect Bounds { get; internal set; }

            public QuadNode(Rect bounds)
            {
                Bounds = bounds;
                Nodes = new ReadOnlyCollection<QuadNode>(_nodes);
                Objects = new ReadOnlyCollection<T>(QuadObjects);
            }

            public QuadNode(float x, float y, float width, float height)
                : this(new Rect(x, y, width, height))
            {
            }

            public bool HasChildNodes()
            {
                return _nodes[0] != null;
            }
        }
    }
}
