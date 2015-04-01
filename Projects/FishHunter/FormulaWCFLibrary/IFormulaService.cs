using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace VGame.Project.FishHunter.WCF
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的介面名稱 "IFormulaService"。
    
    [Serializable]
    public struct FormulaState
    {
        public int CoreFps;
        public int PeerFps;
        public int PeerCount;
        public Int64 TotalReadBytes;
        public Int64 TotalWriteBytes;
        public int ReadBytesPerSecond;
        public int WriteBytesPerSecond;

        public int WattToRead;
        public int WattToWrite;

    }
    [ServiceContract]
    public interface IFormulaService
    {
        [OperationContract]
        FormulaState GetState();
        
    }


    
}
