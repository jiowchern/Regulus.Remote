// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WEAPON_TYPE.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the WEAPON_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.Play
{
	internal class Bullet
	{
		private static int _Sn;

		public int Id { get; set; }

		public Bullet(int id)
		{
			Id = id;
		}

		public Bullet()
		{
			Id = ++Bullet._Sn;
		}
	}
}