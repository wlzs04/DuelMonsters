using Assets.Script.Card;
using Assets.Script.Duel.EffectProcess;
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
            lifeScrollBar = GameObject.Find("opponentLifeScrollbar").GetComponent<Scrollbar>();
            lifeNumberText = GameObject.Find("opponentLifeNumberText").GetComponent<Text>();

            life = DuelRuleManager.GetPlayerStartLife();

            List<CardBase> myCards = duelCardGroup.GetCards();
            for (int i = 0; i < myCards.Count; i++)
            {
                GameObject go = UnityEngine.Object.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(myCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                myCards[i].SetCardObject(go);
                myCards[i].SetCardGameState(CardGameState.Group);
            }
        }

        public override Vector3 GetHeartPosition()
        {
            return new Vector3(0, duelScene.GetDuelHeight());
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
            //不是自己回合不进行操作
            if (duelScene.GetCurrentPlayer() != this)
            {
                return;
            }
            //受效果处理时进行根据效果类型做特殊操作
            if (currentEffectProcess != null)
            {
                if(currentEffectProcess is DiscardHandCardEffectProcess)
                {
                    (currentEffectProcess as DiscardHandCardEffectProcess).DiscardOneHandCard(null,FindWorstCardInHand());
                }
                if (currentEffectProcess is ChooseCardEffectProcess)
                {
                    ChooseCardEffectProcess chooseCardEffectProcess = currentEffectProcess as ChooseCardEffectProcess;

                    chooseCardEffectProcess.ChooseCardCallBack(chooseCardEffectProcess.GetLaunchEffectCard(),FindCanBeChooseCardFromList(chooseCardEffectProcess.GetCanChooseCardList()));
                }
                return;
            }
            if (duelScene.GetCurrentPhaseType() == PhaseType.Draw)
            {
                duelScene.EnterPhaseType(PhaseType.Prepare);
                return;
            }
            else if (duelScene.GetCurrentPhaseType() == PhaseType.Prepare)
            {
                duelScene.EnterPhaseType(PhaseType.Main);
                return;
            }
            //在主要流程中
            else if (duelScene.GetCurrentPhaseType() == PhaseType.Main)
            {
                //先召唤怪兽
                if (GameManager.GetSingleInstance().GetUserData().opponentCanCallMonster && CanCallMonster())
                {
                    foreach (var item in handCards)
                    {
                        DuelCardScript duelCardScript = item.GetDuelCardScript();
                        if (duelCardScript.CanCall())
                        {
                            int sacrificeMonsterNumer = item.NeedSacrificeMonsterNumer();
                            if (sacrificeMonsterNumer > 0)
                            {
                                List<CardBase> sacrificeCards = new List<CardBase>();//祭品列表
                                
                                for (int i = 0; i < DuelRuleManager.GetMonsterAreaNumber(); i++)
                                {
                                    if (monsterCardArea[i] != null)
                                    {
                                        sacrificeCards.Add(monsterCardArea[i]);
                                        sacrificeMonsterNumer-= monsterCardArea[i].GetCanBeSacrificedNumber();
                                        if (sacrificeMonsterNumer <= 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                CardGameState nextCardGameState = ThinkCallMonsterCardGameState(item);
                                
                                CallMonsterEffectProcess callMonsterEffectProcess = new CallMonsterEffectProcess(item, nextCardGameState, sacrificeCards, this);
                                AddEffectProcess(callMonsterEffectProcess);
                            }
                            else
                            {
                                CardGameState nextCardGameState = ThinkCallMonsterCardGameState(item);
                                
                                CallMonsterEffectProcess callMonsterEffectProcess = new CallMonsterEffectProcess(item, nextCardGameState, this);
                                AddEffectProcess(callMonsterEffectProcess);
                            }
                            return;
                        }
                    }
                }
                //然后使用魔法卡
                else if (GameManager.GetSingleInstance().GetUserData().opponentCanLaunchEffect && CanLanuchMagicCard())
                {
                    foreach (var item in magicTrapCardArea)
                    {
                        if (item != null && item.CanLaunchEffect())
                        {
                            LaunchEffect(item);
                            return;
                        }
                    }
                    foreach (var item in GetHandCards())
                    {
                        if (item.CanLaunchEffect())
                        {
                            LaunchEffect(item);
                            return;
                        }
                    }
                }
                //最后进入战斗流程
                else if (GameManager.GetSingleInstance().GetUserData().opponentCanAttack && CanEnterBattlePhaseType())
                {
                    Battle();
                    return;
                }
            }
            else if(duelScene.GetCurrentPhaseType() == PhaseType.Battle)
            {
                //先判断是否存在可以进行攻击的怪兽，并选出攻击力最大的怪兽先进行攻击
                int canAttackMonsterIndex = -1;
                int maxAttackValue = 0;
                for (int i = 0; i < DuelRuleManager.GetMonsterAreaNumber(); i++)
                {
                    if (monsterCardArea[i] != null && monsterCardArea[i].CanAttack() && monsterCardArea[i].GetAttackValue()> maxAttackValue)
                    {
                        canAttackMonsterIndex = i;
                        maxAttackValue = monsterCardArea[i].GetAttackValue();
                    }
                }

                //没有的话进入第二主要回合或回合结束
                if(canAttackMonsterIndex==-1)
                {
                    duelScene.EnterPhaseType(PhaseType.Second);
                    return;
                }
                else
                {
                    CardBase canAttackMonster = monsterCardArea[canAttackMonsterIndex];
                    //如果对方没有可以受攻击的怪兽时，进行直接攻击
                    if (!GetOpponentPlayer().HaveBeAttackedMonster() &&
                        GetCanDirectAttack() &&
                        GetOpponentPlayer().CanBeDirectAttacked() &&
                        canAttackMonster.CanDirectAttack())
                    {
                        AttackEffectProcess attackEffectProcess = new AttackEffectProcess(canAttackMonster, null, this);
                        AddEffectProcess(attackEffectProcess);
                        return;
                    }
                   
                    //如果当前怪兽攻击力，比对方场上表侧表示的怪兽最小的攻击力或守备力大的话进行攻击
                    //如果对方存在里侧表示的怪兽时进行攻击
                    //以上皆不满足的话退出战斗流程
                    foreach (var item in GetOpponentPlayer().GetMonsterCardArea())
                    {
                        bool attackThisMonster = false;
                        if(item != null && item.CanBeAttacked())
                        {
                            if (item.GetCardGameState()==CardGameState.FrontAttack)
                            {
                                if(maxAttackValue>= item.GetAttackValue())
                                {
                                    attackThisMonster = true;
                                }
                            }
                            else if (item.GetCardGameState() == CardGameState.FrontDefense)
                            {
                                if (maxAttackValue >= item.GetDefenseValue())
                                {
                                    attackThisMonster = true;
                                }
                            }
                            else if (item.GetCardGameState() == CardGameState.Back)
                            {
                                attackThisMonster = true;
                            }
                        }
                        
                        if(attackThisMonster)
                        {
                            AttackEffectProcess attackEffectProcess = new AttackEffectProcess(canAttackMonster, item, this);
                            AddEffectProcess(attackEffectProcess);
                            return;
                        }
                    }
                    return;
                }
            }
            else if(duelScene.GetCurrentPhaseType() == PhaseType.End)
            {
            }
            if(duelScene.GetCurrentPhaseType() == PhaseType.Main ||
                duelScene.GetCurrentPhaseType() == PhaseType.Battle ||
                duelScene.GetCurrentPhaseType() == PhaseType.Second)
            {
                EndTurn();
            }
        }

        /// <summary>
        /// 找出手卡中最差的卡,一般用于丢入墓地中(未考虑墓坑)
        /// </summary>
        CardBase FindWorstCardInHand()
        {
            if(handCards.Count!=0)
            {
                foreach (var item in handCards)
                {
                    switch (item.GetCardType())
                    {
                        case CardType.Monster:
                            if(item.GetAttackNumber()==0)
                            {
                                return item;
                            }
                            break;
                        case CardType.Magic:
                            return item;
                        case CardType.Trap:
                            return item;
                        default:
                            break;
                    }
                }
                return handCards[0];
            }
            return null;
        }

        /// <summary>
        /// 从指定列表中选择一张卡牌
        /// </summary>
        /// <param name="cardList"></param>
        /// <returns></returns>
        CardBase FindCanBeChooseCardFromList(List<CardBase> cardList)
        {
            if(cardList==null || cardList.Count<=0)
            {
                return null;
            }
            return cardList[0];
        }

        /// <summary>
        /// 在召唤怪兽时考虑召唤状态，攻击或防守
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        CardGameState ThinkCallMonsterCardGameState(CardBase card)
        {
            CardGameState nextCardGameState;
            int maxAttackValue = 0;
            foreach (var item in GetOpponentPlayer().GetMonsterCardArea())
            {
                if(item!=null && item.GetAttackValue()> maxAttackValue)
                {
                    maxAttackValue = item.GetAttackValue();
                }
            }
            if (card.GetAttackValue() >= maxAttackValue)
            {
                nextCardGameState = CardGameState.FrontAttack;
            }
            else
            {
                nextCardGameState = CardGameState.FrontDefense;
            }
            return nextCardGameState;
        }

        public override void GuessFirstNotify(GuessEnum guessEnum)
        {
            if (guessEnum == GuessEnum.Unknown)
            {
                return;
            }
            if (GameManager.GetSingleInstance().GetUserData().guessMustWin)
            {
                int tempMyGuessEnum = (int)duelScene.GetMyPlayer().GetGuessEnum();
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
