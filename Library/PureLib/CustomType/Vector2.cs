// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vector2.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Vector2 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using ProtoBuf;

#endregion

namespace Regulus.CustomType
{
	[ProtoContract]
	[Serializable]
	public class Vector2
	{
		[ProtoMember(1)]
		public float X;

		[ProtoMember(2)]
		public float Y;

		public float Magnitude
		{
			get { return (float)Math.Sqrt(this.X * this.X + this.Y * this.Y); }
		}

		public Vector2()
		{
		}

		public Vector2(float x, float y)
		{
			this.X = x;
			this.Y = y;
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
			var magnitude = this.Magnitude;
			this.X = this.X / magnitude;
			this.Y = this.Y / magnitude;
		}

		public Vector2 GetNormalized()
		{
			var magnitude = this.Magnitude;

			return new Vector2(this.X / magnitude, this.Y / magnitude);
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
			var v = (Vector2)obj;

			return this.X == v.X && this.Y == v.Y;
		}

		public bool Equals(Vector2 v)
		{
			return this.X == v.X && this.Y == v.Y;
		}

		public override int GetHashCode()
		{
			return this.X.GetHashCode() ^ this.Y.GetHashCode();
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
			return this.X + ", " + this.Y;
		}

		public string ToString(bool rounded)
		{
			if (rounded)
			{
				return (int)Math.Round(this.X) + ", " + (int)Math.Round(this.Y);
			}

			return this.ToString();
		}
	}
}