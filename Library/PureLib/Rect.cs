using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    public struct Rect
    {
        
        private Size _Size;
        public Point Location;
        
        public Rect(Point rootOrigin, Size rootSize)
        {
            // TODO: Complete member initialization
            Location = rootOrigin;
            this._Size = rootSize;
        }

        public Rect(double newX, double newY, double p1, double p2)
        {
            // TODO: Complete member initialization
            Location.X = newX;
            Location.Y = newY;
            _Size.Width = p1;
            _Size.Height = p2;
        }



        public double Width { get { return _Size.Width; } set { _Size.Width = value; } }

        public double Height { get { return _Size.Height; } set { _Size.Height = value; } }

        public double X { get { return Location.X; } set { Location.X = value; } }

        public double Y { get { return Location.Y; } set { Location.Y = value; } }

        internal bool Contains(Rect rect)
        {
            return (X <= rect.X &&
                    Y <= rect.Y &&
                    X + Width >= rect.X+ rect.Width&&
                    Y + Height >= rect.Y + rect.Height);
        }

        internal bool IntersectsWith(Rect rect)
        {
            return (rect.Left <= Right) &&
                  (rect.Right>= Left) &&
                  (rect.Top<= Bottom) &&
                  (rect.Bottom>= Top);
        }



        public double Left { get { return Location.X; } set { Location.X = value; } }

        public double Top { get { return Location.Y; } set { Location.Y = value; } }

        public double Right { get { return Location.X + _Size.Width; } 
            set 
            {
                _Size.Width = value - Location.X;
            } }

        public double Bottom
        {
            get { return Location.Y + _Size.Height; }
            set
            {
                _Size.Height = value - Location.Y;
            }
        }
    }
    
}

