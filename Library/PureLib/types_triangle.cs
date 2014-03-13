using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    [ProtoBuf.ProtoContract]
    public struct Triangle : System.IEquatable<Triangle>, IRotatable
    {
        [ProtoBuf.ProtoMember(1)]
        public Regulus.Types.Vector2 Point1;
        [ProtoBuf.ProtoMember(2)]
        public Regulus.Types.Vector2 Point2;
        [ProtoBuf.ProtoMember(3)]
        public Regulus.Types.Vector2 Point3;

        bool System.IEquatable<Triangle>.Equals(Triangle other)
        {
            return other.Point1 == Point1 && other.Point3 == Point3 && other.Point2 == Point2;
        }


        static Triangle[] Rotate(Regulus.Types.Vector2 center, Triangle[] tris, float angle)
        {
            return new Rotator<Triangle>(center, tris).Rotation(angle);
        }

        Vector2[] IRotatable.Points
        {
            get
            {
                return new Vector2[] { Point1, Point2, Point3};
            }
            set
            {
                Point1 = value[0];
                Point2 = value[1];
                Point3 = value[2];
            }
        }
    }

    public interface IRotatable
    {
        Vector2[] Points { get; set; }
    }
    public class Rotator<T>
        where T : IRotatable, new()
        
    {
        private Vector2 _Center;
        private T[] tris;

        public Rotator(Vector2 center, T[] tris)
        {            
            this._Center = center;
            this.tris = tris;
        }


        public T[] Rotation(float angle)
        {
            List<T> rotaters = new List<T>();
            foreach (var t in tris)
            {
                var tri = new T();
                tri.Points = _RotatePoint(t.Points, _Center, angle);
                rotaters.Add(tri);
            }
            return rotaters.ToArray();
        }
        public Vector2[] _RotatePoint(Vector2[] points, Vector2 centroid, double angle)
        {
            List<Vector2> ret = new List<Vector2>();
            foreach (var point in points)
            {
                ret.Add(_RotatePoint(point, centroid, angle));
            }
            return ret.ToArray();
        }
        public Vector2 _RotatePoint(Vector2 point, Vector2 centroid, double angle)
        {
            var x = centroid.X + ((point.X - centroid.X) * Math.Cos(angle) - (point.Y - centroid.Y) * Math.Sin(angle));

            var y = centroid.Y + ((point.X - centroid.X) * Math.Sin(angle) + (point.Y - centroid.Y) * Math.Cos(angle));

            return new Vector2((float)x, (float)y);
        }        
    }
}
