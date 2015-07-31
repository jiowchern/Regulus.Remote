// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Iquadobject.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IQuadObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using Regulus.CustomType;

#endregion

namespace Regulus.Collection
{
	public interface IQuadObject
	{
		event EventHandler BoundsChanged;

		Rect Bounds { get; }
	}
}