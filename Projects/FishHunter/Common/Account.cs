using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Data
{

    
    public class Account
    {

        [Flags]
        public enum COMPETENCE
        {
            ACCOUNT_MANAGER = 0x1,
            FORMULA_QUERYER = 0x2,            
            GAME_PLAYER = 0x4,
            BACKEND_MANAGER = 0x8,
            FORMULA_RECORDER = 0x10,
            ACCOUNT_FINDER = 0x11,
            ALL = int.MaxValue
        };
        public Guid Id;
        public string Name;
        public string Password;

        public COMPETENCE Competnce;
        
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
