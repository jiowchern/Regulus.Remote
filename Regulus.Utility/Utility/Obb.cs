using System;




namespace Regulus.Utility
{
    /**
 * @author scott.cgi
 * @since  2012-11-19
 *  
 * Oriented bounding box 
 */

    [Serializable]
    public class OBB
    {
        // unit vector of x axis

        private readonly float[] axisX;

        // unit vector of y axis

        private readonly float[] axisY;


        private readonly float[] centerPoint;


        private readonly float halfHeight;


        private readonly float halfWidth;

        // 0 -360

        private float rotation;

        /**
		 * CreateInstnace default OBB
		 * 
		 * @param x Top left x
		 * @param y Top left y
		 * @param width
		 * @param height
		 */
        public OBB() : this(0, 0, 0, 0)
        {
        }

        public OBB(float x, float y, float width, float height)
        {
            axisX = new float[2];
            axisY = new float[2];

            setRotation(0.0f);

            halfWidth = width / 2;
            halfHeight = height / 2;

            centerPoint = new float[2];

            setXY(x, y);
        }

        /**
		 * Get axisX and axisY projection radius distance on axis
		 */
        public float getProjectionRadius(float[] axis)
        {
            // axis, axisX and axisY are unit vector

            // projected axisX to axis
            float projectionAxisX = dot(axis, axisX);

            // projected axisY to axis
            float projectionAxisY = dot(axis, axisY);

            return halfWidth * projectionAxisX + halfHeight * projectionAxisY;
        }

        /**
		 * OBB is collision with other OBB
		 */
        public bool isCollision(OBB obb)
        {
            // two OBB center distance vector
            float[] centerDistanceVertor =
            {
                centerPoint[0] - obb.centerPoint[0],
                centerPoint[1] - obb.centerPoint[1]
            };

            float[][] axes =
            {
                axisX,
                axisY,
                obb.axisX,
                obb.axisY
            };

            for (int i = 0; i < axes.Length; i++)
            {
                // compare OBB1 radius projection add OBB2 radius projection to centerDistance projection
                if (getProjectionRadius(axes[i]) + obb.getProjectionRadius(axes[i]) <= dot(centerDistanceVertor, axes[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /**
		 * dot-multiply
		 */
        private float dot(float[] axisA, float[] axisB)
        {
            return Math.Abs(axisA[0] * axisB[0] + axisA[1] * axisB[1]);
        }

        /**
		 * Set axis x and y by rotation
		 * 
		 * @param rotation float 0 - 360 
		 */
        public OBB setRotation(float rotation)
        {
            this.rotation = rotation;

            float t = (float)(-(rotation - 180) * Math.PI / 180);

            axisX[0] = (float)Math.Cos(t);
            axisX[1] = (float)Math.Sin(t);

            axisY[0] = (float)-Math.Sin(t);
            axisY[1] = (float)Math.Cos(t);

            // this.axisX[0] = -(float)Math.Cos(t);
            // this.axisX[1] = -(float)Math.Sin(t);

            // this.axisY[0] = (float)Math.Sin(t);
            // this.axisY[1] = (float)-Math.Cos(t);

            // axisX[0] = -(float)Math.Sin(t);
            // axisX[1] = -(float)Math.Cos(t);

            // axisY[0] = (float)Math.Cos(t);
            // axisY[1] = -(float)Math.Sin(t);
            return this;
        }

        /**
		 * Set OBB top left x, y
		 */
        public OBB setLeftTop(float l, float t)
        {
            centerPoint[0] = l + halfWidth;
            centerPoint[1] = t + halfHeight;

            return this;
        }

        public OBB setXY(float x, float y)
        {
            centerPoint[0] = x;
            centerPoint[1] = y;

            return this;
        }

        public float getRotation()
        {
            return rotation;
        }

        public float getLeft()
        {
            return centerPoint[0] - halfWidth;
        }

        public float getX()
        {
            return centerPoint[0];
        }

        public float getTop()
        {
            return centerPoint[1] - halfHeight;
        }

        public float getY()
        {
            return centerPoint[1];
        }

        public float getWidth()
        {
            return halfWidth * 2;
        }

        public float getHeight()
        {
            return halfHeight * 2;
        }

        public static OBB[] Read(string path)
        {
            OBB[] obbs = Serialization.Read<OBB[]>(path);
            foreach (OBB obb in obbs)
            {
                obb.setXY(obb.getX(), obb.getY());
                obb.setRotation(obb.getRotation());
            }

            return obbs;
        }

        public static void Write(string path, OBB[] obbs)
        {
            Serialization.Write(obbs, path);
        }
    }
}
