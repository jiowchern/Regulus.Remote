<<<<<<< HEAD
﻿using VGame.Project.FishHunter.Common.GPI;
=======
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpansionFeature.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the ExpansionFeature type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System.CodeDom;
using System.ComponentModel.Design.Serialization;



using Regulus.Remoting;


using VGame.Project.FishHunter.Common;
using VGame.Project.FishHunter.Common.GPI;

#endregion
>>>>>>> bb08c0b8a8aa5ec0c708cd9f624c302cd192eb5d

namespace VGame.Project.FishHunter.Formula
{
	public struct ExpansionFeature
	{
		public IFormulaStageDataRecorder FormulaStageDataRecorder { get; private set; }

		public IAccountFinder AccountFinder { get; private set; }

		public IFormulaPlayerRecorder FormulaPlayerRecorder { get; private set; }

		public ExpansionFeature(
			IAccountFinder account_finder, 
			IFormulaStageDataRecorder formula_stage_data_recorder, 
			IFormulaPlayerRecorder formula_player_recorder) : this()
		{
			AccountFinder = account_finder;
			FormulaStageDataRecorder = formula_stage_data_recorder;
			FormulaPlayerRecorder = formula_player_recorder;
		}
	}
}
