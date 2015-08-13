using System;

namespace VGame.Project.FishHunter.Common.Data
{
	public class Stage : IEquatable<Stage>
	{
		public int Id { get; set; }

		public bool Pass { get; set; }

		bool IEquatable<Stage>.Equals(Stage other)
		{
			return other.Id == Id;
		}
	}
}
