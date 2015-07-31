// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Size.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Size type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using ProtoBuf;

#endregion

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
			this.Width = width;
			this.Height = height;
		}
	}
}