// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnqueueHelper.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the EnqueueHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using Regulus.Collection;

#endregion

namespace PureLibraryTest
{
	internal class EnqueueHelper
	{
		private readonly int i;

		private readonly Queue<int> ints;

		public EnqueueHelper(Queue<int> ints, int i)
		{
			// TODO: Complete member initialization
			this.ints = ints;
			this.i = i;
		}

		internal void Run()
		{
			this.ints.Enqueue(this.i);
		}
	}
}