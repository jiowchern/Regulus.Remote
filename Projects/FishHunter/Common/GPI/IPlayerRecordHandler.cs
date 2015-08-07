// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRecordHandler.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the IRecordHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;


using Regulus.Remoting;


using VGame.Project.FishHunter.Common.Data;

#endregion

namespace VGame.Project.FishHunter.Common.GPI
{
	public interface IRecordHandler
	{
		Value<PlayerRecord> Load(Guid id);

		void Save(PlayerRecord player_record);
	}
}