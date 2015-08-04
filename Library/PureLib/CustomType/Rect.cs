// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rect.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Rect type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using ProtoBuf;

#endregion

namespace Regulus.CustomType
{
	[Serializable]
	[ProtoContract]
	public struct Rect
	{
		[ProtoMember(1)]
		private Size _Size;

		[ProtoMember(2)]
		public Point Location;

		public float Width
		{
			get { return this._Size.Width; }
			set { this._Size.Width = value; }
		}

		public float Height
		{
			get { return this._Size.Height; }
			set { this._Size.Height = value; }
		}

		public float X
		{
			get { return this.Location.X; }
			set { this.Location.X = value; }
		}

		public float Y
		{
			get { return this.Location.Y; }
			set { this.Location.Y = value; }
		}

		public float Left
		{
			get { return this.Location.X; }
			set { this.Location.X = value; }
		}

		public float Top
		{
			get { return this.Location.Y; }
			set { this.Location.Y = value; }
		}

		public float Right
		{
			get { return this.Location.X + this._Size.Width; }
			set { this._Size.Width = value - this.Location.X; }
		}

		public float Bottom
		{
			get { return this.Location.Y + this._Size.Height; }
			set { this._Size.Height = value - this.Location.Y; }
		}

		public Rect(Point rootOrigin, Size rootSize)
		{
			// TODO: Complete member initialization
			this.Location = rootOrigin;
			this._Size = rootSize;
		}

		public Rect(float x, float y, float w, float h)
		{
			// TODO: Complete member initialization
			this.Location.X = x;
			this.Location.Y = y;
			this._Size.Width = w;
			this._Size.Height = h;
		}

		public bool Contains(Rect rect)
		{
			return this.X <= rect.X &&
			        this.Y <= rect.Y &&
			        this.X + this.Width >= rect.X + rect.Width &&
			        this.Y + this.Height >= rect.Y + rect.Height;
		}

		public bool IntersectsWith(Rect rect)
		{
			return (rect.Left <= this.Right) &&
			       (rect.Right >= this.Left) &&
			       (rect.Top <= this.Bottom) &&
			       (rect.Bottom >= this.Top);
		}
	}
}