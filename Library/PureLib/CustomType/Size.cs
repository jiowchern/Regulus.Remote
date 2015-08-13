using System;


using ProtoBuf;

namespace Regulus.CustomType
{
	[Serializable]
	[ProtoContract]
	public struct Size
	{
		[ProtoMember(1)]
		public float Height;

		[ProtoMember(2)]
		public float Width;

		public Size(float width, float height)
		{
			Width = width;
			Height = height;
		}
	}
}
