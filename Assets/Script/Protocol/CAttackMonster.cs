using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 怪兽攻击
    /// key：cardID anotherCardID
    /// value：int int
    /// </summary>
    class CAttackMonster : ClientProtocol
    {
        public CAttackMonster() : base("CAttackMonster") { }

        public override ClientProtocol GetInstance()
        {
            return new CAttackMonster();
        }

        public override void Process()
        {
            int cardID = int.Parse(GetContent("cardID"));
            int anotherCardID = int.Parse(GetContent("anotherCardID"));
            GameManager.GetSingleInstance().GetDuelScene().OpponentAttack(cardID, anotherCardID);
        }
    }
}
