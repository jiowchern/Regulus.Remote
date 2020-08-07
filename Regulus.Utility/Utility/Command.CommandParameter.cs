using System;

namespace Regulus.Utility
{




    public partial class Command
    {
        public class CommandParameter
        {
            public Type Param { get; private set; }

            public string Description { get; private set; }

            public CommandParameter(Type p, string description)
            {
                Param = p;
                Description = description;
            }

            public static implicit operator CommandParameter(Type type)
            {
                return new CommandParameter(type, string.Empty);
            }
        }
    }
}
