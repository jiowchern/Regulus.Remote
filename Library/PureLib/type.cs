using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{

    
    [ProtoBuf.ProtoContract]
    [Serializable]
    public struct Vector2 : IEquatable<object>
    {
        [ProtoBuf.ProtoMember(1)]
        public float X;
        [ProtoBuf.ProtoMember(2)]
        public float Y;

        static public Vector2 FromPoint(Point p)
        {
            return Vector2.FromPoint(p.X, p.Y);
        }

        static public Vector2 FromPoint(float x, float y)
        {
            return new Vector2(x, y);
        }

        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
        }

        public void Normalize()
        {
            float magnitude = Magnitude;
            X = X / magnitude;
            Y = Y / magnitude;
        }

        public Vector2 GetNormalized()
        {
            float magnitude = Magnitude;

            return new Vector2(X / magnitude, Y / magnitude);
        }

        public float DotProduct(Vector2 vector)
        {
            return this.X * vector.X + this.Y * vector.Y;
        }

        public float DistanceTo(Vector2 vector)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X - this.X, 2) + Math.Pow(vector.Y - this.Y, 2));
        }

        public static implicit operator Point(Vector2 p)
        {
            return new Point((int)p.X, (int)p.Y);
        }
        

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a)
        {
            return new Vector2(-a.X, -a.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator *(Vector2 a, int b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator *(Vector2 a, double b)
        {
            return new Vector2((float)(a.X * b), (float)(a.Y * b));
        }

        public override bool Equals(object obj)
        {
            Vector2 v = (Vector2)obj;

            return X == v.X && Y == v.Y;
        }

        public bool Equals(Vector2 v)
        {
            return X == v.X && Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override string ToString()
        {
            return X + ", " + Y;
        }

        public string ToString(bool rounded)
        {
            if (rounded)
            {
                return (int)Math.Round(X) + ", " + (int)Math.Round(Y);
            }
            else
            {
                return ToString();
            }
        }


        

        bool IEquatable<object>.Equals(object other)
        {
            if (other is Vector2)
            {
                Vector2 v = (Vector2)other;
                return X == v.X && Y == v.Y;
            }
            return false;
        }
    }
}
