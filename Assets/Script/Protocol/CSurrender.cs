using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    class CSurrender : ClientProtocol
    {
        public override void Process()
        {
            GameManager.GetDuelScene().GetOpponentPlayer().Surrender();
        }
    }
}
