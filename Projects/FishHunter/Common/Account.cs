using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Extension;
using Regulus.Utility;
namespace VGame.Project.FishHunter.Data
{

    [ProtoBuf.ProtoContract]
    public class Account
    {

        
        public enum COMPETENCE
        {
            [EnumDescription("帳號管理")]
            ACCOUNT_MANAGER  ,
            [EnumDescription("算法查詢")]
            FORMULA_QUERYER ,
            [EnumDescription("遊戲體驗")]
            GAME_PLAYER ,
            [EnumDescription("後端管理")]
            BACKEND_MANAGER ,
            [EnumDescription("算法記錄")]
            FORMULA_RECORDER ,
            [EnumDescription("帳號查詢")]
            ACCOUNT_FINDER ,                        
        };

        [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public string Password { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public Regulus.CustomType.Flag<COMPETENCE> Competnces { get; set; }
        
        public bool IsPassword(string password)
        {
            return Password == password;
        }

        public bool IsFormulaQueryer()
        {
            return _HasCompetence(COMPETENCE.FORMULA_QUERYER);
        }

        private bool _HasCompetence(COMPETENCE competence)
        {
            return Competnces[competence];
        }

        public static Regulus.CustomType.Flag<COMPETENCE> AllCompetnce()
        {
            var flags = EnumHelper.GetFlags<COMPETENCE>().ToArray();
            var f =flags[0];
            return new Regulus.CustomType.Flag<COMPETENCE>(flags);
        }

        public bool HasCompetnce(COMPETENCE cOMPETENCE)
        {
            return _HasCompetence(cOMPETENCE);
        }
    }
}
