// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HitBase.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the HitBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using VGame.Project.FishHunter.Common;

#endregion

namespace VGame.Project.FishHunter.Formula
{
	public abstract class HitBase
	{
		public abstract HitResponse Request(HitRequest request);
	}
}