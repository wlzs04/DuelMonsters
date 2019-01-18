using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 怪兽直接攻击
    /// key：cardID
    /// value：int
    /// </summary>
    class CStopDuel : ClientProtocol
    {
        public override void Process()
        {
            GameManager.GetSingleInstance().GetDuelScene().opponentPlayer.StopDuel();
        }
    }
}
