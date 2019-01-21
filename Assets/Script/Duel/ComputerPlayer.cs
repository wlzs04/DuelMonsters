using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Duel
{
    class ComputerPlayer : Player
    {
        public ComputerPlayer(DuelScene duelScene) : base("电脑", duelScene)
        {

        }

        public override void InitBeforDuel()
        {
            heartPosition = new Vector3(DuelCommonValue.opponentHeartPositionX, DuelCommonValue.opponentHeartPositionY);

            lifeScrollBar = GameObject.Find("opponentLifeScrollbar").GetComponent<Scrollbar>();
            lifeNumberText = GameObject.Find("opponentLifeNumberText").GetComponent<Text>();

            life = DuelRuleManager.GetPlayerStartLife();

            List<CardBase> opponentCards = duelCardGroup.GetCards();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                GameObject go = UnityEngine.Object.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(opponentCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                opponentCards[i].SetCardObject(go);
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
                        DuelCardScript duelCardScript = item.GetDuelCardScript();
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
            else if(duelScene.currentDuelProcess == DuelProcess.End)
            {
                if(duelScene.currentPlayer==this)
                {
                    duelScene.ChangeCurrentPlayer();
                }
                return;
            }
            if(duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Battle ||
                duelScene.currentDuelProcess == DuelProcess.Second)
            {
                EndTurn();
            }
        }

        public override void GuessFirstNotify(GuessEnum guessEnum)
        {
            if (guessEnum == GuessEnum.Unknown)
            {
                return;
            }
            if (GameManager.GetSingleInstance().GetUserData().guessMustWin)
            {
                int tempMyGuessEnum = (int)duelScene.myPlayer.GetGuessEnum();
                int tempOpponentGuessEnum = tempMyGuessEnum - 1 > 0 ? tempMyGuessEnum - 1 : 3;
                GameObject.Find("Main Camera").GetComponent<GuessFirstSceneScript>().SetOpponentGuess((GuessEnum)tempOpponentGuessEnum);

            }
            else
            {
                GameObject.Find("Main Camera").GetComponent<GuessFirstSceneScript>().SetOpponentGuess((GuessEnum)UnityEngine.Random.Range(1, 4));
            }
        }

        public override void SelectFristOrBack()
        {
            System.Random random = new System.Random();
            int i = random.Next(2);
            if (i == 0)
            {
                duelScene.SetFirst(true);
            }
            else
            {
                duelScene.SetFirst(false);
            }
            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                GameManager.GetSingleInstance().EnterDuelScene();
            });
            GameManager.AddTimerFunction(timerFunction);
        }
    }
}
