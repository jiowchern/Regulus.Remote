using System;
using System.Linq;


namespace Regulus.Utility
{
	
	[Serializable]
	public struct Rect
	{
	
		private Size _Size;

	
		public Point Location;

		public float Width
		{
			get { return _Size.Width; }
			set { _Size.Width = value; }
		}

		public float Height
		{
			get { return _Size.Height; }
			set { _Size.Height = value; }
		}

		public float X
		{
			get { return Location.X; }
			set { Location.X = value; }
		}

		public float Y
		{
			get { return Location.Y; }
			set { Location.Y = value; }
		}

		public float Left
		{
			get { return Location.X; }
			set { Location.X = value; }
		}

		public float Top
		{
			get { return Location.Y; }
			set { Location.Y = value; }
		}

		public float Right
		{
			get { return Location.X + _Size.Width; }
			set { _Size.Width = value - Location.X; }
		}

		public float Bottom
		{
			get { return Location.Y + _Size.Height; }
			set { _Size.Height = value - Location.Y; }
		}

		public Rect(Point rootOrigin, Size rootSize)
		{
			
			Location = rootOrigin;
			_Size = rootSize;
		}

	    public Point Center
	    {
	        get { return new Point(Location.X + _Size.Width /2 , Location.Y + _Size.Height / 2);}
	    }
        public Rect(float x, float y, float w, float h)
		{
			
			Location.X = x;
			Location.Y = y;
			_Size.Width = w;
			_Size.Height = h;
		}

		public bool Contains(Rect rect)
		{
			return X <= rect.X &&
			       Y <= rect.Y &&
			       X + Width >= rect.X + rect.Width &&
			       Y + Height >= rect.Y + rect.Height;
		}

		public bool IntersectsWith(Rect rect)
		{
			return (rect.Left <= Right) &&
			       (rect.Right >= Left) &&
			       (rect.Top <= Bottom) &&
			       (rect.Bottom >= Top);
		}

	    

        public static Rect Merge(params Rect[] bounds)
        {
            
            var left  = (from b in bounds select b.Left).Min();
	        var right = (from b in bounds select b.Right).Max();
	        var top    = (from b in bounds select b.Top).Min();
	        var bottom = (from b in bounds select b.Bottom).Max();
            return new Rect(left, top, right - left, bottom - top);
        }
	}
}
