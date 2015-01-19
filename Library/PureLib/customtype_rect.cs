using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.CustomType
{
    [Serializable]
    [ProtoBuf.ProtoContract]
    public struct Rect
    {
        
        [ProtoBuf.ProtoMember(1)]
        private Size _Size;
        [ProtoBuf.ProtoMember(2)]
        public Point Location;

        public Rect(Point rootOrigin, Size rootSize)
        {
            // TODO: Complete member initialization
            Location = rootOrigin;
            this._Size = rootSize;
        }

        public Rect(float x, float y, float w, float h)
        {
            // TODO: Complete member initialization
            Location.X = x;
            Location.Y = y;
            _Size.Width = w;
            _Size.Height = h;
        }



        public float Width { get { return _Size.Width; } set { _Size.Width = value; } }

        public float Height { get { return _Size.Height; } set { _Size.Height = value; } }

        public float X { get { return Location.X; } set { Location.X = value; } }

        public float Y { get { return Location.Y; } set { Location.Y = value; } }

        public bool Contains(Rect rect)
        {
            return (X <= rect.X &&
                    Y <= rect.Y &&
                    X + Width >= rect.X+ rect.Width&&
                    Y + Height >= rect.Y + rect.Height);
        }

        public bool IntersectsWith(Rect rect)
        {
            return (rect.Left <= Right) &&
                  (rect.Right>= Left) &&
                  (rect.Top<= Bottom) &&
                  (rect.Bottom>= Top);
        }



        public float Left { get { return Location.X; } set { Location.X = value; } }

        public float Top { get { return Location.Y; } set { Location.Y = value; } }

        public float Right { get { return Location.X + _Size.Width; } 
            set 
            {
                _Size.Width = value - Location.X;
            } }

        public float Bottom
        {
            get { return Location.Y + _Size.Height; }
            set
            {
                _Size.Height = value - Location.Y;
            }
        }
    }
    
}

