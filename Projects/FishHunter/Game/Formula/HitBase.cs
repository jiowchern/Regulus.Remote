using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Formula
{
    public abstract class HitBase
    {
        public abstract HitResponse Request(HitRequest request);
    }
}
