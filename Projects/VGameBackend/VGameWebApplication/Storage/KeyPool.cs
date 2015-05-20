using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter.Storage
{
    class KeyPool : Regulus.Utility.Singleton<KeyPool>
    {

        System.Collections.Generic.Dictionary<Guid, VGameWebApplication.Models.VerifyData> _Keys;

        public KeyPool()
        {
            _Keys = new Dictionary<Guid, VGameWebApplication.Models.VerifyData>();
        }
        

        internal Guid Query(string user, string password)
        {
            var id = (from kp in _Keys where kp.Value.Account == user select kp.Key).FirstOrDefault();
            if(id == Guid.Empty)
            {
                id = Guid.NewGuid();
                _Keys.Add(id, new VGameWebApplication.Models.VerifyData { Account = user, Password = password });
            }

            return id;

        }

        internal void Destroy(Guid guid)
        {
            _Keys.Remove(guid);
        }

        internal VGameWebApplication.Models.VerifyData Find(Guid id)
        {
            VGameWebApplication.Models.VerifyData data = null;
            _Keys.TryGetValue(id, out data);
            return data;
        }
    }
}
