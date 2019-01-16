using Assets.Script.Card;
using Assets.Script.Duel.Rule;
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

        public override void BeAttackedMonsterNotify(int attackCardId, int beAttackedCardId)
        {
            base.BeAttackedMonsterNotify(attackCardId, beAttackedCardId);
        }

        /// <summary>
        /// 电脑思考行动，暂时制作简单处理。
        /// </summary>
        public override void ThinkAction()
        {
            if(duelScene.currentDuelProcess == DuelProcess.Draw)
            {
                duelScene.EnterDuelProcess(DuelProcess.Prepare);
                return;
            }
            else if (duelScene.currentDuelProcess == DuelProcess.Prepare)
            {
                duelScene.EnterDuelProcess(DuelProcess.Main);
                return;
            }
            //在主要流程中
            else if (duelScene.currentDuelProcess == DuelProcess.Main)
            {
                //先召唤怪兽
                if (CanCallMonster())
                {
                    foreach (var item in handCards)
                    {
                        DuelCardScript duelCardScript = item.cardObject.GetComponent<DuelCardScript>();
                        if (duelCardScript.CanCall())
                        {
                            MonsterCard monsterCard = (MonsterCard)item;
                            int sacrificeMonsterNumer = monsterCard.NeedSacrificeMonsterNumer();
                            if(sacrificeMonsterNumer>0)
                            {
                                string sacrificeInfo = "";
                                int index = 0;
                                for (int i = 0; i < DuelRuleManager.GetMonsterAreaNumber(); i++)
                                {
                                    if (monsterCardArea[i] != null)
                                    {
                                        sacrificeInfo += monsterCardArea[i].GetID()+":";
                                        sacrificeMonsterNumer--;
                                        index = i;
                                        if (sacrificeMonsterNumer<=0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                if(sacrificeInfo.EndsWith(":"))
                                {
                                    sacrificeInfo = sacrificeInfo.Substring(0, sacrificeInfo.Length - 1);
                                }
                                CallMonsterByProtocol(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.FrontAttack, index,sacrificeInfo);
                            }
                            else
                            {
                                for (int i = 0; i < DuelRuleManager.GetMonsterAreaNumber(); i++)
                                {
                                    if (monsterCardArea[i] == null)
                                    {
                                        CallMonsterByProtocol(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.FrontAttack, i);
                                        return;
                                    }
                                }
                            }
                            return;
                        }
                    }
                }
                if (CanBattle())
                {
                    duelScene.EnterDuelProcess(DuelProcess.Battle);
                    return;
                }
            }
            else if(duelScene.currentDuelProcess == DuelProcess.Battle)
            {

            }
            if(duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Battle ||
                duelScene.currentDuelProcess == DuelProcess.Second)
            {
                EndTurn();
            }
        }
    }
}
