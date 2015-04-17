using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regulus.Utility;
namespace VGame.Project.FishHunter.Data
{

    [ProtoBuf.ProtoContract]
    public class Account
    {

        [Flags]
        public enum COMPETENCE
        {
            [EnumDescription("帳號管理")]
            ACCOUNT_MANAGER = 0x1,
            [EnumDescription("算法查詢")]
            FORMULA_QUERYER = 0x2,
            [EnumDescription("遊戲體驗")]
            GAME_PLAYER = 0x4,
            [EnumDescription("後端管理")]
            BACKEND_MANAGER = 0x8,
            [EnumDescription("算法記錄")]
            FORMULA_RECORDER = 0x10,
            [EnumDescription("帳號查詢")]
            ACCOUNT_FINDER = 0x20,            
            ALL = int.MaxValue
        };

        [ProtoBuf.ProtoMember(1)]
        public Guid Id { get; set; }
        [ProtoBuf.ProtoMember(2)]
        public string Name { get; set; }

        [ProtoBuf.ProtoMember(3)]
        public string Password { get; set; }
        [ProtoBuf.ProtoMember(4)]
        public COMPETENCE Competnce { get; set; }
        
        public bool IsPassword(string password)
        {
            return Password == password;
        }

        public bool IsFormulaQueryer()
        {
            var val = Competnce & COMPETENCE.FORMULA_QUERYER;
            return val == COMPETENCE.FORMULA_QUERYER;
        }
    }
}
