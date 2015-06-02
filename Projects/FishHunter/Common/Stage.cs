using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Data
{
    public class Stage : IEquatable<Stage>
    {
        public int Id { get; set; }
        public bool Pass { get; set; }

        bool IEquatable<Stage>.Equals(Stage other)
        {
            return other.Id == Id;
        }
    }
}
