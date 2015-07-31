// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StageRecodeData.cs" company="Regulus Framework">
//   Regulus Framework
// </copyright>
// <summary>
//   Defines the Recode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VGame.Project.FishHunter.ZsFormula.DataStructs
{
	public class Recode
	{
		public enum RECODE_TYPE
		{
			PLAYTOTAL, // 0

			WINTOTAL, 

			PLAYTIMES, 

			WINTIMES, 

			ASNTIMES, 

			ASNWIN, 

			SP00_WINTIMES, // 6

			SP00_WINTOTAL, 

			SP01_WINTIMES, 

			SP01_WINTOTAL, 

			SP02_WINTIMES, 

			SP02_WINTOTAL, 

			SP03_WINTIMES, // 12

			SP03_WINTOTAL, 

			SP04_WINTIMES, 

			SP04_WINTOTAL, // 15

			SP05_WINTIMES, 

			SP05_WINTOTAL, 

			SP06_WINTIMES, 

			SP06_WINTOTAL, 

			SP07_WINTIMES, // 20

			SP07_WINTOTAL, 

			SP08_WINTIMES, 

			SP08_WINTOTAL, 

			SP09_WINTIMES, 

			SP09_WINTOTAL, // 25

			MAX = 30
		}

		public int PlayTotal { get; set; } // 0

		public int WinTotal { get; set; }

		public int PlayTimes { get; set; }

		public int WinTimes { get; set; }

		public int AsnTimes { get; set; }

		public int AsnWin { get; set; }

		public int Sp00WinTimes { get; set; } // 6

		public int Sp00WinTotal { get; set; }

		public int Sp01WinTimes { get; set; } // 6

		public int Sp02WinTotal { get; set; }

		public int Sp03WinTimes { get; set; } // 6

		public int Sp03WinTotal { get; set; }

		public int Sp04WinTimes { get; set; } // 6

		public int Sp04WinTotal { get; set; }

		public int Sp05WinTimes { get; set; } // 6

		public int Sp05WinTotal { get; set; }

		public int Sp06WinTimes { get; set; } // 6

		public int Sp06WinTotal { get; set; }

		public int Sp07WinTimes { get; set; } // 6

		public int Sp07WinTotal { get; set; }

		public int Sp08WinTimes { get; set; } // 6

		public int Sp08WinTotal { get; set; }

		public int Sp09WinTimes { get; set; } // 6

		public int Sp09WinTotal { get; set; }
	}
}