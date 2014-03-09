using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{

    public class Polygon
    {
        // Structure that stores the results of the PolygonCollision function
        static public CollisionResult Collision(Polygon polygonA, Polygon polygonB, Vector2 velocity)
        {
            CollisionResult result = new CollisionResult();
            result.Intersect = true;
            result.WillIntersect = true;

            int edgeCountA = polygonA.Edges.Count;
            int edgeCountB = polygonB.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = polygonA.Edges[edgeIndex];
                }
                else
                {
                    edge = polygonB.Edges[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0) result.Intersect = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                float velocityProjection = axis.DotProduct(velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersect && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation Vector2
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = polygonA.Center - polygonB.Center;
                    if (d.DotProduct(translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation Vector2 can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector2.
            if (result.WillIntersect) result.MinimumTranslationVector2 = translationAxis * minIntervalDistance;

            return result;
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        static public float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        static public void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product
            float d = axis.DotProduct(polygon.Points[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = polygon.Points[i].DotProduct(axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }


        public struct CollisionResult
        {
            public bool WillIntersect; // Are the polygons going to intersect forward in time?
            public bool Intersect; // Are the polygons currently intersecting
            public Vector2 MinimumTranslationVector2; // The translation to apply to polygon A to push the polygons appart.
        }

        private List<Vector2> points = new List<Vector2>();
        private List<Vector2> edges = new List<Vector2>();


        
        public void BuildEdges()
        {
            Vector2 p1;
            Vector2 p2;
            edges.Clear();
            for (int i = 0; i < points.Count; i++)
            {
                p1 = points[i];
                if (i + 1 >= points.Count)
                {
                    p2 = points[0];
                }
                else
                {
                    p2 = points[i + 1];
                }
                edges.Add(p2 - p1);
            }
        }

        public List<Vector2> Edges
        {
            get { return edges; }
        }

        public List<Vector2> Points
        {
            get { return points; }
        }

        public Vector2 Center
        {
            get
            {
                float totalX = 0;
                float totalY = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    totalX += points[i].X;
                    totalY += points[i].Y;
                }

                return new Vector2(totalX / (float)points.Count, totalY / (float)points.Count);
            }
        }

        public void Offset(Vector2 v)
        {
            Offset(v.X, v.Y);
        }

        public void Offset(float x, float y)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p = points[i];
                points[i] = new Vector2(p.X + x, p.Y + y);
            }
        }

        public override string ToString()
        {
            string result = "";

            for (int i = 0; i < points.Count; i++)
            {
                if (result != "") result += " ";
                result += "{" + points[i].ToString(true) + "}";
            }

            return result;
        }



        

        private void MergeSort(int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSort(left, mid);
                MergeSort(mid + 1, right);
                Merge(left, mid, right);
            }
        }

        private void Merge(int left, int mid, int right)
        {
            int i = left, j = mid + 1, top = 0;
            Point[] data = new Point[right + 1];
            while (i <= mid && j <= right)
            {
                if (cmp(points[i], points[j])) data[top++] = points[i++];
                else data[top++] = points[j++];
            }
            while (i <= mid) data[top++] = points[i++];
            while (j <= right) data[top++] = points[j++];
            for (i = 0, j = left; i < top; i++, j++)
                points[j] = Vector2.FromPoint(data[i].X, data[i].Y);
        }
        private float cross(Point o, Point a, Point b)
        {
            return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
        }
        private bool cmp(Point a, Point b)
        {
            return (a.Y < b.Y) || (a.Y == b.Y && a.X < b.X);
        }

        public void Convex()
        {
            MergeSort(0, points.Count - 1);

            Vector2[] CH = new Vector2[points.Count + 1];
            int m = 0;
            for (int i = 0; i < points.Count; i++)
            {
                while (m >= 2 && cross(CH[m - 2], CH[m - 1], points[i]) <= 0) m--;
                CH[m++] = points[i];
            }
            for (int i = points.Count - 2, t = m + 1; i >= 0; i--)
            {
                while (m >= t && cross(CH[m - 2], CH[m - 1], points[i]) <= 0) m--;
                CH[m++] = points[i];
            }
            points.Clear();
            for (int i = 0; i < m-1; ++i)
            {
                points.Add(CH[i]);
            }


            BuildEdges();
        }
    }
}
