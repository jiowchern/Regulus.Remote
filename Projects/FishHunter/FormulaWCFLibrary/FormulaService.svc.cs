using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VGame.Project.FishHunter.WCF
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼、svc 和組態檔中的類別名稱 "FormulaService"。
    // 注意: 若要啟動 WCF 測試用戶端以便測試此服務，請在 [方案總管] 中選取 FormulaService.svc 或 FormulaService.svc.cs，然後開始偵錯。
[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]    
    public class FormulaService : IFormulaService 
    {
        VGame.Project.FishHunter.Formula.Center _Formula;

        Regulus.Remoting.Soul.Native.Server _Server;
        Regulus.Utility.Launcher _Launcher;

        public FormulaService()
        {
            _Formula = new Formula.Center();
            _Server = new Regulus.Remoting.Soul.Native.Server(_Formula, 38971);
            _Launcher = new Regulus.Utility.Launcher();

            _Launcher.Push(_Server);
        }
        
       

        public void Launch()
        {
            _Launcher.Launch();
        }

        public void Shutdown()
        {
            _Launcher.Shutdown();
        }

        int IFormulaService.GetCoreFPS()
        {
            return _Server.CoreFPS;
        }
    }
}
