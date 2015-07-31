// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Record.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the Record type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Test_Region

using System;

using ProtoBuf;

#endregion

namespace VGame.Project.FishHunter.Common
{
	[ProtoContract]
	public class Record
	{
		[ProtoMember(1)]
		public Guid Id { get; set; }

		[ProtoMember(2)]
		public int Money { get; set; }

		[ProtoMember(3)]
		public Guid Owner { get; set; }

		public Record()
		{
			this.Id = Guid.NewGuid();
			this.Owner = Guid.Empty;
		}
	}
}