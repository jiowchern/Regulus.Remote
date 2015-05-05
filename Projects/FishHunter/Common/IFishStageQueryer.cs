using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VGame.Project.FishHunter
{
    /// <summary>
    /// 魚場請求
    /// </summary>
    public interface IFishStageQueryer
    {
        /// <summary>
        /// 查詢請求
        /// </summary>
        /// <param name="玩家id"></param>
        /// <param name="魚場id"></param>
        /// <returns>魚場</returns>
        Regulus.Remoting.Value<IFishStage> Query(long player_id, byte fish_stage);

    }
}
