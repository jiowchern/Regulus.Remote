using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    internal interface ISkillCaptureAbility
    {
        bool TryGetBounds(ref CustomType.Rect bounds , ref int Skill);
        void Hit();
    }
}
