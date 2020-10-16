using System;

namespace Regulus.Utility
{




    public partial class Command
    {
        private class Infomation
        {
            public readonly Func<string[],object> Handler;

            public readonly string Name;
            internal readonly Guid Id;

            public Infomation(string name, Func<string[],object> handler)
            {
                Name = name;
                Handler = handler;
                Id = System.Guid.NewGuid();
            }
        }
    }
}
