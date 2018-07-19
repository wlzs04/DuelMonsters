using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 结束回合
    /// </summary>
    class CEndTurn : ClientProtocol
    {
        public CEndTurn() : base("CEndTurn") { }

        public override ClientProtocol GetInstance()
        {
            return new CEndTurn();
        }

        public override void Process()
        {
            GameManager.GetSingleInstance().GetDuelScene().opponentPlayer.EndTurn();
        }
    }
}
