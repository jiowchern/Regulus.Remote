// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Point.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Point type.
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
	public struct Point
	{
		[ProtoMember(1)]
		public float X;

		[ProtoMember(2)]
		public float Y;

		public Point(float x, float y)
		{
			// TODO: Complete member initialization
			this.X = x;
			this.Y = y;
		}
	}
}