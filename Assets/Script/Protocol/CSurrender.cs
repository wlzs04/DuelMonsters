using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    class CSurrender : ClientProtocol
    {
        public CSurrender() : base("CSurrender") { }

        public override ClientProtocol GetInstance()
        {
            return new CSurrender();
        }

        public override void Process()
        {
            GameManager.GetSingleInstance().GetDuelScene().opponentPlayer.Surrender();
        }
    }
}
