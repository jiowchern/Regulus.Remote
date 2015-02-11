using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    public interface IVerify
    {        
        /// <summary>
        /// 驗證
        /// </summary>
        /// <param name="帳號"></param>
        /// <param name="密碼"></param>
        /// <returns>回傳真則為成功</returns>
        Regulus.Remoting.Value<bool> Login(string id , string password);
    }
}
