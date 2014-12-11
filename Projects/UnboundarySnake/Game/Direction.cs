using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.UnboundarySnake
{
    public enum DIRECTION
    {

        [DirectToVector(0,0)]
        None,
        [DirectToVector(0, 1)]
        Up,
        [DirectToVector(0, -1)]
        Dowm,
        [DirectToVector(-1, 0)]
        Left,
        [DirectToVector(1, 0)]
        Right
    }

    namespace Extension
    {
        static public class DirectionExtension
        {
            static public Vector ToVector(this DIRECTION dir)
            {                
                var type = typeof(DIRECTION);
                var fields = type.GetFields();
                
                var attributes = (from field in fields
                        where field.Name == dir.ToString()
                        select field.GetCustomAttributes(typeof(DirectToVectorAttribute), false)).SingleOrDefault();

                DirectToVectorAttribute dtv = (DirectToVectorAttribute)attributes[0];
                return dtv.Vector;
            }
            
        }

        static public class DirectionExtension2
        {
            static public Vector ToVector2(this DIRECTION dir, int i)
            {
                return new Vector();
            }
        }
    }

}
