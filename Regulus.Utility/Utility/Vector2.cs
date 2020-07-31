using System;
using System.Xml.Serialization;




namespace Regulus.Utility
{
    [Serializable]
	
    [XmlRoot("Vector2")]
    public struct Vector2
	{
        [XmlAttribute("x")]
    
		public float X;

        [XmlAttribute("y")]
    
		public float Y;

		public float Magnitude
		{
			get { return (float)Math.Sqrt(X * X + Y * Y); }
		}

        public Vector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public static Vector2 FromPoint(Point p)
		{
			return Vector2.FromPoint(p.X, p.Y);
		}

		public static Vector2 FromPoint(float x, float y)
		{
			return new Vector2(x, y);
		}

		public void Normalize()
		{
			var magnitude = Magnitude;
			X = X / magnitude;
			Y = Y / magnitude;
		}

		public Vector2 GetNormalized()
		{
			var magnitude = Magnitude;

			return new Vector2(X / magnitude, Y / magnitude);
		}

		public float DotProduct(Vector2 vector)
		{
			return X * vector.X + Y * vector.Y;
		}

	    public float DistanceToNoSqrt(Vector2 vector)
	    {
	        return (float)(Math.Pow(vector.X - X, 2) + Math.Pow(vector.Y - Y, 2));

	    }

        public float DistanceTo(Vector2 vector)
		{
			return (float)Math.Sqrt(Math.Pow(vector.X - X, 2) + Math.Pow(vector.Y - Y, 2));
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
			var v = (Vector2)obj;

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
			if(rounded)
			{
				return (int)Math.Round(X) + ", " + (int)Math.Round(Y);
			}

			return ToString();
		}

	    public static Vector2 AngleToVector(float angle)
	    {
            angle %= 360;
            var radians = angle * 0.0174532924;
            return new Vector2((float)Math.Cos(radians), (float)-Math.Sin(radians));
        }

	    public static float VectorToAngle(Vector2 vec)
	    {
	        return (float) -(Math.Atan2(vec.Y, vec.X) * 180 / Math.PI);
	    }

	    public void Set(float x, float y)
	    {
	        X = x;
	        Y = y;
	    }
	}
}
