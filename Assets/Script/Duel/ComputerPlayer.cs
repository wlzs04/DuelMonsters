using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel
{
    class ComputerPlayer : Player
    {
        public ComputerPlayer(DuelScene duelScene) : base("电脑", duelScene)
        {

        }

        public override void InitCardGroup()
        {
            List<CardBase> opponentCards = duelCardGroup.GetCards();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                GameObject go = UnityEngine.Object.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(opponentCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                opponentCards[i].cardObject = go;
                go.transform.SetParent(duelScene.duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.opponentCardGroupPositionX, DuelCommonValue.opponentCardGroupPositionY, 0);
            }
        }

        public override void DrawNotify()
        {

        }

        /// <summary>
        /// 电脑思考行动，暂时制作简单处理。
        /// </summary>
        public override void ThinkAction()
        {
            //先召唤怪兽
            if(CanCallMonster())
            {
                foreach (var item in handCards)
                {
                    DuelCardScript duelCardScript = item.cardObject.GetComponent<DuelCardScript>();
                    if (duelCardScript.CanCall())
                    {
                        CallMonster((MonsterCard)item);
                        return;
                    }
                }
            }
            EndTurn();
        }
    }
}
