using System;

namespace Regulus.Utility
{




    public partial class Command
    {
        private class Infomation
		{
			public readonly Action<string[]> Handler;

			public readonly string Name;
			internal readonly Guid Id;

			public Infomation(string name , Action<string[]> handler)
			{
				Name = name;
				Handler = handler;
				Id = System.Guid.NewGuid();
			}
		}
	}
}
