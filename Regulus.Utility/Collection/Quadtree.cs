using Regulus.Utility;
using System;
using System.Collections.Generic;

namespace Regulus.Collection
{
    public partial class QuadTree<T> where T : class, IQuadObject
    {

        private readonly int maxObjectsPerLeaf;

        private readonly Size minLeafSize;

        private readonly Dictionary<T, int> objectSortOrder = new Dictionary<T, int>();

        private readonly Dictionary<T, QuadNode> objectToNodeLookup = new Dictionary<T, QuadNode>();

        private readonly bool sort;

        private readonly object syncLock = new object();

        private int objectSortId;

        public QuadNode Root { get; private set; }

        public QuadTree(Size minLeafSize, int maxObjectsPerLeaf)
        {
            Root = null;
            this.minLeafSize = minLeafSize;
            this.maxObjectsPerLeaf = maxObjectsPerLeaf;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="QuadTree{T}" /> class.
        /// </summary>
        /// <param name="minLeafSize">
        ///     The smallest size a leaf will split into
        /// </param>
        /// <param name="maxObjectsPerLeaf">
        ///     Maximum number of objects per leaf before it forces a split into sub quadrants
        /// </param>
        /// <param name="sort">
        ///     Whether or not queries will return objects in the order in which they were added
        /// </param>
        public QuadTree(Size minLeafSize, int maxObjectsPerLeaf, bool sort)
            : this(minLeafSize, maxObjectsPerLeaf)
        {
            this.sort = sort;
        }

        public int GetSortOrder(T quadObject)
        {
            lock (objectSortOrder)
            {
                if (!objectSortOrder.ContainsKey(quadObject))
                {
                    return -1;
                }

                return objectSortOrder[quadObject];
            }
        }

        public void Insert(T quadObject)
        {
            lock (syncLock)
            {
                if (sort & !objectSortOrder.ContainsKey(quadObject))
                {
                    objectSortOrder.Add(quadObject, objectSortId++);
                }

                Rect bounds = quadObject.Bounds;
                if (Root == null)
                {
                    Size rootSize = new Size(
                        (float)Math.Ceiling(bounds.Width / minLeafSize.Width),
                        (float)Math.Ceiling(bounds.Height / minLeafSize.Height));
                    float multiplier = Math.Max(rootSize.Width, rootSize.Height);
                    rootSize = new Size(minLeafSize.Width * multiplier, minLeafSize.Height * multiplier);
                    Point center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
                    Point rootOrigin = new Point(center.X - rootSize.Width / 2, center.Y - rootSize.Height / 2);
                    Root = new QuadNode(new Rect(rootOrigin, rootSize));
                }

                while (!Root.Bounds.Contains(bounds))
                {
                    ExpandRoot(bounds);
                }

                InsertNodeObject(Root, quadObject);
            }
        }

        public List<T> Query(Rect bounds)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();
                if (Root != null)
                {
                    Query(bounds, Root, results);
                }

                if (sort)
                {
                    results.Sort((a, b) => { return objectSortOrder[a].CompareTo(objectSortOrder[b]); });
                }

                return results;
            }
        }

        private void Query(Rect bounds, QuadNode node, List<T> results)
        {
            lock (syncLock)
            {
                if (node == null)
                {
                    return;
                }

                if (bounds.IntersectsWith(node.Bounds))
                {
                    foreach (T quadObject in node.Objects)
                    {
                        if (bounds.IntersectsWith(quadObject.Bounds))
                        {
                            results.Add(quadObject);
                        }
                    }

                    foreach (QuadNode childNode in node.Nodes)
                    {
                        Query(bounds, childNode, results);
                    }
                }
            }
        }

        private void ExpandRoot(Rect newChildBounds)
        {
            lock (syncLock)
            {
                bool isNorth = Root.Bounds.Y < newChildBounds.Y;
                bool isWest = Root.Bounds.X < newChildBounds.X;

                QuadtreeDirection rootDirection;
                if (isNorth)
                {
                    rootDirection = isWest
                                        ? QuadtreeDirection.NW
                                        : QuadtreeDirection.NE;
                }
                else
                {
                    rootDirection = isWest
                                        ? QuadtreeDirection.SW
                                        : QuadtreeDirection.SE;
                }

                float newX = (rootDirection == QuadtreeDirection.NW || rootDirection == QuadtreeDirection.SW)
                               ? Root.Bounds.X
                               : Root.Bounds.X - Root.Bounds.Width;
                float newY = (rootDirection == QuadtreeDirection.NW || rootDirection == QuadtreeDirection.NE)
                               ? Root.Bounds.Y
                               : Root.Bounds.Y - Root.Bounds.Height;
                Rect newRootBounds = new Rect(newX, newY, Root.Bounds.Width * 2, Root.Bounds.Height * 2);
                QuadNode newRoot = new QuadNode(newRootBounds);
                SetupChildNodes(newRoot);
                newRoot[rootDirection] = Root;
                Root = newRoot;
            }
        }

        private void InsertNodeObject(QuadNode node, T quadObject)
        {
            lock (syncLock)
            {
                if (!node.Bounds.Contains(quadObject.Bounds))
                {
                    throw new Exception("This should not happen, child does not fit within node bounds");
                }

                if (!node.HasChildNodes() && node.Objects.Count + 1 > maxObjectsPerLeaf)
                {
                    SetupChildNodes(node);

                    List<T> childObjects = new List<T>(node.Objects);
                    List<T> childrenToRelocate = new List<T>();

                    foreach (T childObject in childObjects)
                    {
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode == null)
                            {
                                continue;
                            }

                            if (childNode.Bounds.Contains(childObject.Bounds))
                            {
                                childrenToRelocate.Add(childObject);
                            }
                        }
                    }

                    foreach (T childObject in childrenToRelocate)
                    {
                        RemoveQuadObjectFromNode(childObject);
                        InsertNodeObject(node, childObject);
                    }
                }

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        if (childNode.Bounds.Contains(quadObject.Bounds))
                        {
                            InsertNodeObject(childNode, quadObject);
                            return;
                        }
                    }
                }

                AddQuadObjectToNode(node, quadObject);
            }
        }

        private void ClearQuadObjectsFromNode(QuadNode node)
        {
            lock (syncLock)
            {
                List<T> quadObjects = new List<T>(node.Objects);
                foreach (T quadObject in quadObjects)
                {
                    RemoveQuadObjectFromNode(quadObject);
                }
            }
        }

        private void RemoveQuadObjectFromNode(T quadObject)
        {
            lock (syncLock)
            {
                QuadNode node = objectToNodeLookup[quadObject];
                node.QuadObjects.Remove(quadObject);
                objectToNodeLookup.Remove(quadObject);
                quadObject.BoundsChanged -= _OnBoundsChanged;
            }
        }

        private void AddQuadObjectToNode(QuadNode node, T quadObject)
        {
            lock (syncLock)
            {
                node.QuadObjects.Add(quadObject);
                objectToNodeLookup.Add(quadObject, node);
                quadObject.BoundsChanged += _OnBoundsChanged;
            }
        }

        private void _OnBoundsChanged(object sender, EventArgs e)
        {
            lock (syncLock)
            {
                T quadObject = sender as T;
                if (quadObject != null)
                {
                    QuadNode node = objectToNodeLookup[quadObject];
                    if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes())
                    {
                        RemoveQuadObjectFromNode(quadObject);
                        Insert(quadObject);
                        if (node.Parent != null)
                        {
                            CheckChildNodes(node.Parent);
                        }
                    }
                }
            }
        }

        private void SetupChildNodes(QuadNode node)
        {
            lock (syncLock)
            {
                if (minLeafSize.Width <= node.Bounds.Width / 2 && minLeafSize.Height <= node.Bounds.Height / 2)
                {
                    node[QuadtreeDirection.NW] = new QuadNode(
                        node.Bounds.X,
                        node.Bounds.Y,
                        node.Bounds.Width / 2,
                        node.Bounds.Height / 2);
                    node[QuadtreeDirection.NE] = new QuadNode(
                        node.Bounds.X + node.Bounds.Width / 2,
                        node.Bounds.Y,
                        node.Bounds.Width / 2,
                        node.Bounds.Height / 2);
                    node[QuadtreeDirection.SW] = new QuadNode(
                        node.Bounds.X,
                        node.Bounds.Y + node.Bounds.Height / 2,
                        node.Bounds.Width / 2,
                        node.Bounds.Height / 2);
                    node[QuadtreeDirection.SE] = new QuadNode(
                        node.Bounds.X + node.Bounds.Width / 2,
                        node.Bounds.Y + node.Bounds.Height / 2,
                        node.Bounds.Width / 2,
                        node.Bounds.Height / 2);
                }
            }
        }

        public void Remove(T quadObject)
        {
            lock (syncLock)
            {
                if (sort && objectSortOrder.ContainsKey(quadObject))
                {
                    objectSortOrder.Remove(quadObject);
                }

                if (!objectToNodeLookup.ContainsKey(quadObject))
                {
                    throw new KeyNotFoundException("QuadObject not found in dictionary for removal");
                }

                QuadNode containingNode = objectToNodeLookup[quadObject];
                RemoveQuadObjectFromNode(quadObject);

                if (containingNode.Parent != null)
                {
                    CheckChildNodes(containingNode.Parent);
                }
            }
        }

        private void CheckChildNodes(QuadNode node)
        {
            lock (syncLock)
            {
                if (GetQuadObjectCount(node) <= maxObjectsPerLeaf)
                {
                    // Move child objects into this node, and delete sub nodes
                    List<T> subChildObjects = GetChildObjects(node);
                    foreach (T childObject in subChildObjects)
                    {
                        if (!node.Objects.Contains(childObject))
                        {
                            RemoveQuadObjectFromNode(childObject);
                            AddQuadObjectToNode(node, childObject);
                        }
                    }

                    if (node[QuadtreeDirection.NW] != null)
                    {
                        node[QuadtreeDirection.NW].Parent = null;
                        node[QuadtreeDirection.NW] = null;
                    }

                    if (node[QuadtreeDirection.NE] != null)
                    {
                        node[QuadtreeDirection.NE].Parent = null;
                        node[QuadtreeDirection.NE] = null;
                    }

                    if (node[QuadtreeDirection.SW] != null)
                    {
                        node[QuadtreeDirection.SW].Parent = null;
                        node[QuadtreeDirection.SW] = null;
                    }

                    if (node[QuadtreeDirection.SE] != null)
                    {
                        node[QuadtreeDirection.SE].Parent = null;
                        node[QuadtreeDirection.SE] = null;
                    }

                    if (node.Parent != null)
                    {
                        CheckChildNodes(node.Parent);
                    }
                    else
                    {
                        // Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
                        int numQuadrantsWithObjects = 0;
                        QuadNode nodeWithObjects = null;
                        foreach (QuadNode childNode in node.Nodes)
                        {
                            if (childNode != null && GetQuadObjectCount(childNode) > 0)
                            {
                                numQuadrantsWithObjects++;
                                nodeWithObjects = childNode;
                                if (numQuadrantsWithObjects > 1)
                                {
                                    break;
                                }
                            }
                        }

                        if (numQuadrantsWithObjects == 1)
                        {
                            foreach (QuadNode childNode in node.Nodes)
                            {
                                if (childNode != nodeWithObjects)
                                {
                                    childNode.Parent = null;
                                }
                            }

                            Root = nodeWithObjects;
                        }
                    }
                }
            }
        }

        private List<T> GetChildObjects(QuadNode node)
        {
            lock (syncLock)
            {
                List<T> results = new List<T>();
                results.AddRange(node.QuadObjects);
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        results.AddRange(GetChildObjects(childNode));
                    }
                }

                return results;
            }
        }

        public int GetQuadObjectCount()
        {
            lock (syncLock)
            {
                if (Root == null)
                {
                    return 0;
                }

                int count = GetQuadObjectCount(Root);
                return count;
            }
        }

        private int GetQuadObjectCount(QuadNode node)
        {
            lock (syncLock)
            {
                int count = node.Objects.Count;
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        count += GetQuadObjectCount(childNode);
                    }
                }

                return count;
            }
        }

        public int GetQuadNodeCount()
        {
            lock (syncLock)
            {
                if (Root == null)
                {
                    return 0;
                }

                int count = GetQuadNodeCount(Root, 1);
                return count;
            }
        }

        private int GetQuadNodeCount(QuadNode node, int count)
        {
            lock (syncLock)
            {
                if (node == null)
                {
                    return count;
                }

                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public List<QuadNode> GetAllNodes()
        {
            lock (syncLock)
            {
                List<QuadNode> results = new List<QuadNode>();
                if (Root != null)
                {
                    results.Add(Root);
                    GetChildNodes(Root, results);
                }

                return results;
            }
        }

        private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
        {
            lock (syncLock)
            {
                foreach (QuadNode childNode in node.Nodes)
                {
                    if (childNode != null)
                    {
                        results.Add(childNode);
                        GetChildNodes(childNode, results);
                    }
                }
            }
        }
    }
}
