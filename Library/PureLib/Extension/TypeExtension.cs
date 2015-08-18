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
    }
}