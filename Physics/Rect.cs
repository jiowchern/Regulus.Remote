using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Regulus.Types
{
    public class Rect
    {
        private Point rootOrigin;
        private Size rootSize;
        private double newX;
        private double newY;
        private double p1;
        private double p2;

        public Rect(Point rootOrigin, Size rootSize)
        {
            // TODO: Complete member initialization
            this.rootOrigin = rootOrigin;
            this.rootSize = rootSize;
        }

        public Rect(double newX, double newY, double p1, double p2)
        {
            // TODO: Complete member initialization
            this.newX = newX;
            this.newY = newY;
            this.p1 = p1;
            this.p2 = p2;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        internal bool Contains(Rect bounds)
        {
            throw new NotImplementedException();
        }

        internal bool IntersectsWith(Rect rect)
        {
            throw new NotImplementedException();
        }

        public Point Location { get; set; }
    }
    
}

