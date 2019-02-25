using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 猜先的选择
    /// key：duelProcess
    /// value：DuelProcess 
    /// </summary>
    class CEnterDuelProcess : ClientProtocol
    {
        public override void Process()
        {
            DuelProcess opponentDuelProcess = (DuelProcess)Enum.Parse(typeof(DuelProcess), GetContent("duelProcess"));
            GameManager.GetDuelScene().OpponentEnterDuelProcess(opponentDuelProcess);
        }
    }
}
