// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Stage.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Stage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

#endregion

namespace VGame.Project.FishHunter.Common.Data
{
	public class Stage : IEquatable<Stage>
	{
		public int Id { get; set; }

		public bool Pass { get; set; }

		bool IEquatable<Stage>.Equals(Stage other)
		{
			return other.Id == this.Id;
		}
	}
}