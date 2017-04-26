using System;




namespace Regulus.CustomType
{
	
    [Serializable]
	public struct Size
	{

		public float Height;


		public float Width;

		public Size(float width, float height)
		{
			Width = width;
			Height = height;
		}
	}
}
