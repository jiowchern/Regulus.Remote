using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Types
{
    public class Point
    {
        private double p1;
        private double p2;

        public Point(double p1, double p2)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
        }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
