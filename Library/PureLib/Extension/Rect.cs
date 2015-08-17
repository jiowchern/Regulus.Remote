using System.Diagnostics.CodeAnalysis;


using Regulus.CustomType;

namespace Regulus.Extension
{


    public static class RectExtension
    {        
        public static Rect LeftToCenter(this Rect rect)
        {
            var x = rect.X - rect.Width / 2;
            var y = rect.Y - rect.Height / 2;

            return new Rect(x,y , rect.Width , rect.Height );
        }

        public static Rect CenterToLeft(this Rect rect)
        {
            var x = rect.X + rect.Width / 2;
            var y = rect.Y + rect.Height/ 2;

            return new Rect(x, y, rect.Width, rect.Height);
        }
    }
}