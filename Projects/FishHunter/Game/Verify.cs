using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VGame.Project.FishHunter;

namespace VGame.Project.FishHunter
{
    public class Verify : IVerify 
    {

        public delegate void DoneCallback(Data.Account account);
        public event DoneCallback DoneEvent;

        IStorage _Storage;
        public Verify(IStorage storage)
        {
            _Storage = storage;
        }
        Regulus.Remoting.Value<bool> IVerify.Login(string id, string password)
        {
            Regulus.Remoting.Value<bool> returnValue = new Regulus.Remoting.Value<bool>();
            var val = _Storage.FindAccount(id);
            val.OnValue += (account) =>
            {
                var found = account != null;
                if (found && account.IsPassword(password))
                {                    
                    DoneEvent(account);
                    returnValue.SetValue(true);
                }
                else
                    returnValue.SetValue(false);
            };
            return returnValue;
        }

        
    }
}
