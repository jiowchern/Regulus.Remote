using System;
using System.Collections.Generic;
using System.Linq;


using Regulus.CustomType;

namespace Regulus.Extension
{
    public static class TypeExtension
    {
        public static Rect ToRect(this IEnumerable<Vector2> points)
        {
            
            if (!points.Any())
                throw new Exception("invalid points to rect.");

            var rect = new Rect();
            var first = points.First();
            rect.Left = first.X;
            rect.Top = first.Y;
            rect.Right = first.X;
            rect.Bottom = first.Y;

            foreach(var point in points.Skip(1))
            {
                if(rect.Left > point.X)
                    rect.Left = point.X;

                if(rect.Right < point.X)
                {
                    rect.Right = point.X;
                }

                if(rect.Top > point.Y)
                {
                    rect.Top = point.Y;
                }

                if(rect.Bottom < point.Y)
                    rect.Bottom = point.Y;
            }
            return rect;
        }

        public static IEnumerable<Vector2> FindHull(this IEnumerable<Vector2> points)
        {
            List<PointToProcess> pointsToProcess = new List<PointToProcess>();

            // convert input points to points we can process
            foreach (Vector2 point in points)
            {
                pointsToProcess.Add(new PointToProcess(point));
            }

            // find a point, with lowest X and lowest Y
            int firstCornerIndex = 0;
            PointToProcess firstCorner = pointsToProcess[0];

            for (int i = 1, n = pointsToProcess.Count; i < n; i++)
            {
                if ((pointsToProcess[i].x < firstCorner.x) ||
                     ((pointsToProcess[i].x == firstCorner.x) && (pointsToProcess[i].y < firstCorner.y)))
                {
                    firstCorner = pointsToProcess[i];
                    firstCornerIndex = i;
                }
            }

            // remove the just found point
            pointsToProcess.RemoveAt(firstCornerIndex);

            // find K (tangent of line's angle) and distance to the first corner
            for (int i = 0, n = pointsToProcess.Count; i < n; i++)
            {
                float dx = pointsToProcess[i].x - firstCorner.x;
                float dy = pointsToProcess[i].y - firstCorner.y;

                // don't need square root, since it is not important in our case
                pointsToProcess[i].Distance = dx * dx + dy * dy;
                // tangent of lines angle
                pointsToProcess[i].K = (dx == 0) ? float.PositiveInfinity : (float)dy / dx;
            }

            // sort points by angle and distance
            pointsToProcess.Sort();

            List<PointToProcess> convexHullTemp = new List<PointToProcess>();

            // add first corner, which is always on the hull
            convexHullTemp.Add(firstCorner);
            // add another point, which forms a line with lowest slope
            convexHullTemp.Add(pointsToProcess[0]);
            pointsToProcess.RemoveAt(0);

            PointToProcess lastPoint = convexHullTemp[1];
            PointToProcess prevPoint = convexHullTemp[0];

            while (pointsToProcess.Count != 0)
            {
                PointToProcess newPoint = pointsToProcess[0];

                // skip any point, which has the same slope as the last one or
                // has 0 distance to the first point
                if ((newPoint.K == lastPoint.K) || (newPoint.Distance == 0))
                {
                    pointsToProcess.RemoveAt(0);
                    continue;
                }

                // check if current point is on the left side from two last points
                if ((newPoint.x - prevPoint.x) * (lastPoint.y - newPoint.y) - (lastPoint.x - newPoint.x) * (newPoint.y - prevPoint.y) < 0)
                {
                    // add the point to the hull
                    convexHullTemp.Add(newPoint);
                    // and remove it from the list of points to process
                    pointsToProcess.RemoveAt(0);

                    prevPoint = lastPoint;
                    lastPoint = newPoint;
                }
                else
                {
                    // remove the last point from the hull
                    convexHullTemp.RemoveAt(convexHullTemp.Count - 1);

                    lastPoint = prevPoint;
                    prevPoint = convexHullTemp[convexHullTemp.Count - 2];
                }
            }

            // convert points back
            List<Vector2> convexHull = new List<Vector2>();

            foreach (PointToProcess pt in convexHullTemp)
            {
                convexHull.Add(pt.ToPoint());
            }

            return convexHull;
        }

        // Internal comparer for sorting points
        private class PointToProcess : IComparable
        {
            public float x;
            public float y;
            public float K;
            public float Distance;

            public PointToProcess(Vector2 point)
            {
                x = point.X;
                y = point.Y;

                K = 0;
                Distance = 0;
            }

            public int CompareTo(object obj)
            {
                PointToProcess another = (PointToProcess)obj;

                return (K < another.K) ? -1 : (K > another.K) ? 1 :
                    ((Distance > another.Distance) ? -1 : (Distance < another.Distance) ? 1 : 0);
            }

            public Vector2 ToPoint()
            {
                return new Vector2(x, y);
            }
        }
    }
}