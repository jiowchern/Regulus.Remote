// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DummyView.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the DummyView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Regulus.Utility
{
	public class DummyView : Console.IViewer
	{
		void Console.IViewer.WriteLine(string message)
		{
		}

		void Console.IViewer.Write(string message)
		{
		}
	}
}