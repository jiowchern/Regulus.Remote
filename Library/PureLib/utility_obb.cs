using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Utility
{
	/**
 * @author scott.cgi
 * @since  2012-11-19
 *  
 * Oriented bounding box 
 */

	[Serializable]
    [ProtoBuf.ProtoContract]
	public class OBB
	{
        [ProtoBuf.ProtoMember(1)]
		private float[] centerPoint;

        [ProtoBuf.ProtoMember(2)]
		private float halfWidth;

        [ProtoBuf.ProtoMember(3)]
		private float halfHeight;

		// unit vector of x axis
        [ProtoBuf.ProtoMember(4)]
		private float[] axisX;
		// unit vector of y axis
        [ProtoBuf.ProtoMember(5)]
		private float[] axisY;
        
		// 0 -360
        [ProtoBuf.ProtoMember(6)]
		private float rotation;


        
		/**
		 * Create default OBB
		 * 
		 * @param x Top left x
		 * @param y Top left y
		 * @param width
		 * @param height
		 */
        public OBB() : this(0,0,0,0)
        { 
        }
		public OBB(float x, float y, float width, float height)
		{

			this.axisX = new float[2];
			this.axisY = new float[2];

			this.setRotation(0.0f);

			this.halfWidth = width / 2;
			this.halfHeight = height / 2;

			this.centerPoint = new float[2];

			this.setXY(x, y);
		}

		/**
		 * Get axisX and axisY projection radius distance on axis
		 */
		public float getProjectionRadius(float[] axis)
		{

			// axis, axisX and axisY are unit vector

			// projected axisX to axis
			float projectionAxisX = this.dot(axis, this.axisX);
			// projected axisY to axis
			float projectionAxisY = this.dot(axis, this.axisY);

			return this.halfWidth * projectionAxisX + this.halfHeight * projectionAxisY;
		}

		/**
		 * OBB is collision with other OBB
		 */
		public bool isCollision(OBB obb)
		{
			// two OBB center distance vector
			float[] centerDistanceVertor = {
                this.centerPoint[0] - obb.centerPoint[0],
                this.centerPoint[1] - obb.centerPoint[1]       };

			float[][] axes = {
                this.axisX,
                this.axisY,
                obb.axisX,
                obb.axisY,
            };

			for (int i = 0; i < axes.Length; i++)
			{
				// compare OBB1 radius projection add OBB2 radius projection to centerDistance projection
				if (this.getProjectionRadius(axes[i]) + obb.getProjectionRadius(axes[i])  <= this.dot(centerDistanceVertor, axes[i]))
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

            var t =  (float)(-(rotation - 180) * Math.PI / 180);
			
			this.axisX[0] = (float)Math.Cos(t);
			this.axisX[1] = (float)Math.Sin(t);

			this.axisY[0] = (float)-Math.Sin(t);
			this.axisY[1] = (float)Math.Cos(t);

            //this.axisX[0] = -(float)Math.Cos(t);
            //this.axisX[1] = -(float)Math.Sin(t);

            //this.axisY[0] = (float)Math.Sin(t);
            //this.axisY[1] = (float)-Math.Cos(t);

            //axisX[0] = -(float)Math.Sin(t);
            //axisX[1] = -(float)Math.Cos(t);

            //axisY[0] = (float)Math.Cos(t);
            //axisY[1] = -(float)Math.Sin(t);


			return this;
		}

		/**
		 * Set OBB top left x, y
		 */
        public OBB setLeftTop(float l, float t)
        {
            this.centerPoint[0] = l + this.halfWidth;
            this.centerPoint[1] = t + this.halfHeight;

            return this;
        }

		public OBB setXY(float x, float y)
		{
			this.centerPoint[0] = x ;
			this.centerPoint[1] = y ;

			return this;
		}

		public float getRotation()
		{
			return this.rotation;
		}

        public float getLeft()
        {
            return this.centerPoint[0] - this.halfWidth;
        }
		public float getX()
		{
			return this.centerPoint[0] ;
		}
        public float getTop()
        {
            return this.centerPoint[1] - this.halfHeight;
        }
		public float getY()
		{
            return this.centerPoint[1];
		}

		public float getWidth()
		{
			return this.halfWidth * 2;
		}

		public float getHeight()
		{
			return this.halfHeight * 2;
		}


		public static OBB[] Read(string path)
		{
            OBB[] obbs = Regulus.Utility.IO.Serialization.Read<OBB[]>(path);
            foreach(var obb in obbs)
            {
                obb.setXY(obb.getX(), obb.getY());
                obb.setRotation(obb.getRotation());
            }
            return obbs;
		}

		public static void Write(string path , OBB[] obbs)
		{
			Regulus.Utility.IO.Serialization.Write<OBB[]>(obbs , path);
		}
        
	}
}
