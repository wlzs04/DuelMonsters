using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 进入决斗阶段
    /// key：phaseType
    /// value：PhaseType 
    /// </summary>
    class CEnterPhaseType : ClientProtocol
    {
        public override void Process()
        {
            PhaseType opponentPhaseType = (PhaseType)Enum.Parse(typeof(PhaseType), GetContent("phaseType"));
            GameManager.GetDuelScene().OpponentEnterPhaseType(opponentPhaseType);
        }
    }
}
