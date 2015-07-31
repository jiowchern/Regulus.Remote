// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Quadtree.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the QuadTree type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Regulus.CustomType;

#endregion

namespace Regulus.Collection
{
	public class QuadTree<T> where T : class, IQuadObject
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
			this.Root = null;
			this.minLeafSize = minLeafSize;
			this.maxObjectsPerLeaf = maxObjectsPerLeaf;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QuadTree{T}"/> class. 
		/// </summary>
		/// <param name="minLeafSize">
		/// The smallest size a leaf will split into
		/// </param>
		/// <param name="maxObjectsPerLeaf">
		/// Maximum number of objects per leaf before it forces a split into sub quadrants
		/// </param>
		/// <param name="sort">
		/// Whether or not queries will return objects in the order in which they were added
		/// </param>
		public QuadTree(Size minLeafSize, int maxObjectsPerLeaf, bool sort)
			: this(minLeafSize, maxObjectsPerLeaf)
		{
			this.sort = sort;
		}

		public class QuadNode
		{
			private static int _id;

			private readonly QuadNode[] _nodes = new QuadNode[4];

			public readonly int ID = QuadNode._id++;

			public ReadOnlyCollection<QuadNode> Nodes { get; set; }

			public ReadOnlyCollection<T> Objects { get; set; }

			internal List<T> QuadObjects = new List<T>();

			public QuadNode Parent { get; internal set; }

			public QuadNode this[Direction direction]
			{
				get
				{
					switch (direction)
					{
						case Direction.NW:
							return this._nodes[0];
						case Direction.NE:
							return this._nodes[1];
						case Direction.SW:
							return this._nodes[2];
						case Direction.SE:
							return this._nodes[3];
						default:
							return null;
					}
				}

				set
				{
					switch (direction)
					{
						case Direction.NW:
							this._nodes[0] = value;
							break;
						case Direction.NE:
							this._nodes[1] = value;
							break;
						case Direction.SW:
							this._nodes[2] = value;
							break;
						case Direction.SE:
							this._nodes[3] = value;
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
				this.Bounds = bounds;
				this.Nodes = new ReadOnlyCollection<QuadNode>(this._nodes);
				this.Objects = new ReadOnlyCollection<T>(this.QuadObjects);
			}

			public QuadNode(float x, float y, float width, float height)
				: this(new Rect(x, y, width, height))
			{
			}

			public bool HasChildNodes()
			{
				return this._nodes[0] != null;
			}
		}

		public int GetSortOrder(T quadObject)
		{
			lock (this.objectSortOrder)
			{
				if (!this.objectSortOrder.ContainsKey(quadObject))
				{
					return -1;
				}

				return this.objectSortOrder[quadObject];
			}
		}

		public void Insert(T quadObject)
		{
			lock (this.syncLock)
			{
				if (this.sort & !this.objectSortOrder.ContainsKey(quadObject))
				{
					this.objectSortOrder.Add(quadObject, this.objectSortId++);
				}

				var bounds = quadObject.Bounds;
				if (this.Root == null)
				{
					var rootSize = new Size((float)Math.Ceiling(bounds.Width / this.minLeafSize.Width), 
						(float)Math.Ceiling(bounds.Height / this.minLeafSize.Height));
					var multiplier = Math.Max(rootSize.Width, rootSize.Height);
					rootSize = new Size(this.minLeafSize.Width * multiplier, this.minLeafSize.Height * multiplier);
					var center = new Point(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
					var rootOrigin = new Point(center.X - rootSize.Width / 2, center.Y - rootSize.Height / 2);
					this.Root = new QuadNode(new Rect(rootOrigin, rootSize));
				}

				while (!this.Root.Bounds.Contains(bounds))
				{
					this.ExpandRoot(bounds);
				}

				this.InsertNodeObject(this.Root, quadObject);
			}
		}

		public List<T> Query(Rect bounds)
		{
			lock (this.syncLock)
			{
				var results = new List<T>();
				if (this.Root != null)
				{
					this.Query(bounds, this.Root, results);
				}

				if (this.sort)
				{
					results.Sort((a, b) => { return this.objectSortOrder[a].CompareTo(this.objectSortOrder[b]); });
				}

				return results;
			}
		}

		private void Query(Rect bounds, QuadNode node, List<T> results)
		{
			lock (this.syncLock)
			{
				if (node == null)
				{
					return;
				}

				if (bounds.IntersectsWith(node.Bounds))
				{
					foreach (var quadObject in node.Objects)
					{
						if (bounds.IntersectsWith(quadObject.Bounds))
						{
							results.Add(quadObject);
						}
					}

					foreach (var childNode in node.Nodes)
					{
						this.Query(bounds, childNode, results);
					}
				}
			}
		}

		private void ExpandRoot(Rect newChildBounds)
		{
			lock (this.syncLock)
			{
				var isNorth = this.Root.Bounds.Y < newChildBounds.Y;
				var isWest = this.Root.Bounds.X < newChildBounds.X;

				Direction rootDirection;
				if (isNorth)
				{
					rootDirection = isWest
						? Direction.NW
						: Direction.NE;
				}
				else
				{
					rootDirection = isWest
						? Direction.SW
						: Direction.SE;
				}

				var newX = (rootDirection == Direction.NW || rootDirection == Direction.SW)
					? this.Root.Bounds.X
					: this.Root.Bounds.X - this.Root.Bounds.Width;
				var newY = (rootDirection == Direction.NW || rootDirection == Direction.NE)
					? this.Root.Bounds.Y
					: this.Root.Bounds.Y - this.Root.Bounds.Height;
				var newRootBounds = new Rect(newX, newY, this.Root.Bounds.Width * 2, this.Root.Bounds.Height * 2);
				var newRoot = new QuadNode(newRootBounds);
				this.SetupChildNodes(newRoot);
				newRoot[rootDirection] = this.Root;
				this.Root = newRoot;
			}
		}

		private void InsertNodeObject(QuadNode node, T quadObject)
		{
			lock (this.syncLock)
			{
				if (!node.Bounds.Contains(quadObject.Bounds))
				{
					throw new Exception("This should not happen, child does not fit within node bounds");
				}

				if (!node.HasChildNodes() && node.Objects.Count + 1 > this.maxObjectsPerLeaf)
				{
					this.SetupChildNodes(node);

					var childObjects = new List<T>(node.Objects);
					var childrenToRelocate = new List<T>();

					foreach (var childObject in childObjects)
					{
						foreach (var childNode in node.Nodes)
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

					foreach (var childObject in childrenToRelocate)
					{
						this.RemoveQuadObjectFromNode(childObject);
						this.InsertNodeObject(node, childObject);
					}
				}

				foreach (var childNode in node.Nodes)
				{
					if (childNode != null)
					{
						if (childNode.Bounds.Contains(quadObject.Bounds))
						{
							this.InsertNodeObject(childNode, quadObject);
							return;
						}
					}
				}

				this.AddQuadObjectToNode(node, quadObject);
			}
		}

		private void ClearQuadObjectsFromNode(QuadNode node)
		{
			lock (this.syncLock)
			{
				var quadObjects = new List<T>(node.Objects);
				foreach (var quadObject in quadObjects)
				{
					this.RemoveQuadObjectFromNode(quadObject);
				}
			}
		}

		private void RemoveQuadObjectFromNode(T quadObject)
		{
			lock (this.syncLock)
			{
				var node = this.objectToNodeLookup[quadObject];
				node.QuadObjects.Remove(quadObject);
				this.objectToNodeLookup.Remove(quadObject);
				quadObject.BoundsChanged -= this._OnBoundsChanged;
			}
		}

		private void AddQuadObjectToNode(QuadNode node, T quadObject)
		{
			lock (this.syncLock)
			{
				node.QuadObjects.Add(quadObject);
				this.objectToNodeLookup.Add(quadObject, node);
				quadObject.BoundsChanged += this._OnBoundsChanged;
			}
		}

		private void _OnBoundsChanged(object sender, EventArgs e)
		{
			lock (this.syncLock)
			{
				var quadObject = sender as T;
				if (quadObject != null)
				{
					var node = this.objectToNodeLookup[quadObject];
					if (!node.Bounds.Contains(quadObject.Bounds) || node.HasChildNodes())
					{
						this.RemoveQuadObjectFromNode(quadObject);
						this.Insert(quadObject);
						if (node.Parent != null)
						{
							this.CheckChildNodes(node.Parent);
						}
					}
				}
			}
		}

		private void SetupChildNodes(QuadNode node)
		{
			lock (this.syncLock)
			{
				if (this.minLeafSize.Width <= node.Bounds.Width / 2 && this.minLeafSize.Height <= node.Bounds.Height / 2)
				{
					node[Direction.NW] = new QuadNode(node.Bounds.X, node.Bounds.Y, node.Bounds.Width / 2, 
						node.Bounds.Height / 2);
					node[Direction.NE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2, node.Bounds.Y, 
						node.Bounds.Width / 2, 
						node.Bounds.Height / 2);
					node[Direction.SW] = new QuadNode(node.Bounds.X, node.Bounds.Y + node.Bounds.Height / 2, 
						node.Bounds.Width / 2, 
						node.Bounds.Height / 2);
					node[Direction.SE] = new QuadNode(node.Bounds.X + node.Bounds.Width / 2, 
						node.Bounds.Y + node.Bounds.Height / 2, 
						node.Bounds.Width / 2, node.Bounds.Height / 2);
				}
			}
		}

		public void Remove(T quadObject)
		{
			lock (this.syncLock)
			{
				if (this.sort && this.objectSortOrder.ContainsKey(quadObject))
				{
					this.objectSortOrder.Remove(quadObject);
				}

				if (!this.objectToNodeLookup.ContainsKey(quadObject))
				{
					throw new KeyNotFoundException("QuadObject not found in dictionary for removal");
				}

				var containingNode = this.objectToNodeLookup[quadObject];
				this.RemoveQuadObjectFromNode(quadObject);

				if (containingNode.Parent != null)
				{
					this.CheckChildNodes(containingNode.Parent);
				}
			}
		}

		private void CheckChildNodes(QuadNode node)
		{
			lock (this.syncLock)
			{
				if (this.GetQuadObjectCount(node) <= this.maxObjectsPerLeaf)
				{
					// Move child objects into this node, and delete sub nodes
					var subChildObjects = this.GetChildObjects(node);
					foreach (var childObject in subChildObjects)
					{
						if (!node.Objects.Contains(childObject))
						{
							this.RemoveQuadObjectFromNode(childObject);
							this.AddQuadObjectToNode(node, childObject);
						}
					}

					if (node[Direction.NW] != null)
					{
						node[Direction.NW].Parent = null;
						node[Direction.NW] = null;
					}

					if (node[Direction.NE] != null)
					{
						node[Direction.NE].Parent = null;
						node[Direction.NE] = null;
					}

					if (node[Direction.SW] != null)
					{
						node[Direction.SW].Parent = null;
						node[Direction.SW] = null;
					}

					if (node[Direction.SE] != null)
					{
						node[Direction.SE].Parent = null;
						node[Direction.SE] = null;
					}

					if (node.Parent != null)
					{
						this.CheckChildNodes(node.Parent);
					}
					else
					{
						// Its the root node, see if we're down to one quadrant, with none in local storage - if so, ditch the other three
						var numQuadrantsWithObjects = 0;
						QuadNode nodeWithObjects = null;
						foreach (var childNode in node.Nodes)
						{
							if (childNode != null && this.GetQuadObjectCount(childNode) > 0)
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
							foreach (var childNode in node.Nodes)
							{
								if (childNode != nodeWithObjects)
								{
									childNode.Parent = null;
								}
							}

							this.Root = nodeWithObjects;
						}
					}
				}
			}
		}

		private List<T> GetChildObjects(QuadNode node)
		{
			lock (this.syncLock)
			{
				var results = new List<T>();
				results.AddRange(node.QuadObjects);
				foreach (var childNode in node.Nodes)
				{
					if (childNode != null)
					{
						results.AddRange(this.GetChildObjects(childNode));
					}
				}

				return results;
			}
		}

		public int GetQuadObjectCount()
		{
			lock (this.syncLock)
			{
				if (this.Root == null)
				{
					return 0;
				}

				var count = this.GetQuadObjectCount(this.Root);
				return count;
			}
		}

		private int GetQuadObjectCount(QuadNode node)
		{
			lock (this.syncLock)
			{
				var count = node.Objects.Count;
				foreach (var childNode in node.Nodes)
				{
					if (childNode != null)
					{
						count += this.GetQuadObjectCount(childNode);
					}
				}

				return count;
			}
		}

		public int GetQuadNodeCount()
		{
			lock (this.syncLock)
			{
				if (this.Root == null)
				{
					return 0;
				}

				var count = this.GetQuadNodeCount(this.Root, 1);
				return count;
			}
		}

		private int GetQuadNodeCount(QuadNode node, int count)
		{
			lock (this.syncLock)
			{
				if (node == null)
				{
					return count;
				}

				foreach (var childNode in node.Nodes)
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
			lock (this.syncLock)
			{
				var results = new List<QuadNode>();
				if (this.Root != null)
				{
					results.Add(this.Root);
					this.GetChildNodes(this.Root, results);
				}

				return results;
			}
		}

		private void GetChildNodes(QuadNode node, ICollection<QuadNode> results)
		{
			lock (this.syncLock)
			{
				foreach (var childNode in node.Nodes)
				{
					if (childNode != null)
					{
						results.Add(childNode);
						this.GetChildNodes(childNode, results);
					}
				}
			}
		}
	}

	public enum Direction
	{
		NW = 0, 

		NE = 1, 

		SW = 2, 

		SE = 3
	}
}