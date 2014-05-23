using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Project.SamebestKeys
{
    partial class Realm
    {
        

        abstract class JoinCondition
        {
            public interface IResourceProvider { int PlayerCount { get; } }
            bool _LastCheck;

            protected JoinCondition(bool default_check)
            {
                _LastCheck = default_check;
            }

            public bool Check(IResourceProvider provider)
            {
                _LastCheck = _Check(provider);
                return _LastCheck;
            }
            protected abstract bool _Check(IResourceProvider realm);


            public bool LastCheck()
            {
                return _LastCheck;
            }
        }

        class FreeJoinCondition : JoinCondition
        {
            public FreeJoinCondition() :base(true){ }
            protected override bool _Check(IResourceProvider provider)
            {
                return true;
            }
        }

        class LimitJoinCondition : JoinCondition
        {
            private int _Amount;

            public LimitJoinCondition(int amount) : base(true)
            {
                // TODO: Complete member initialization
                this._Amount = amount;
            }

            protected override bool _Check(IResourceProvider provider)
            {
                return provider.PlayerCount < _Amount;
            }
        }

        class ForbidJoinCondition : JoinCondition
        {
            public ForbidJoinCondition() : base(false) { }
            protected override bool _Check(IResourceProvider provider)
            {
                return false;
            }
        }

        
        
    }


    
}
