using System;


using ProtoBuf;

namespace Regulus.CustomType
{
	[Serializable]
	[ProtoContract]
	public struct Point
	{
		[ProtoMember(1)]
		public float X;

		[ProtoMember(2)]
		public float Y;

		public Point(float x, float y)
		{
			// TODO: Complete member initialization
			X = x;
			Y = y;
		}
	}
}
