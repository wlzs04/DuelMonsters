using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 攻击
    /// </summary>
    class AttackEffectProcess : EffectProcessBase
    {
        CardBase attackCard = null;
        CardBase beAttackedCard;
        bool directAttack = false;

        public AttackEffectProcess(CardBase attackCard, CardBase beAttackedCard, Player ownerPlayer) : base(ownerPlayer, "攻击")
        {
            canBeChained = true;
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            //如果对方没有可以被攻击的怪兽，则进行直接攻击
            if(!ownerPlayer.GetOpponentPlayer().HaveBeAttackedMonster())
            {
                directAttack = true;
            }
            this.attackCard = attackCard;
            this.beAttackedCard = beAttackedCard;

            finishAction = () => 
            {
                duelScene.SetTitle("");
                attackCard.GetDuelCardScript().ClearPrepareAttackState();
            };
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            if (directAttack)
            {
                attackCard.GetDuelCardScript().SetAttackState(false);
                duelScene.SetAttackAnimationFinishEvent(() =>
                {
                    CheckCardCanChainLaunch();
                });
                duelScene.StartPlayAttackAnimation(attackCard.GetDuelCardScript().GetPosition(), ownerPlayer.GetOpponentPlayer().GetHeartPosition());
            }
            else if (beAttackedCard == null)
            {
                duelScene.SetTitle(ConfigManager.GetConfigByName("StringRes").GetRecordValueById(22));
            }
            else
            {
                ChooseBeAttackedMonster(beAttackedCard);
            }
        }

        /// <summary>
        /// 判断当前是否在等待玩家选择将被攻击的怪兽
        /// </summary>
        /// <returns></returns>
        public bool WaitChooseBeAttackedMonster()
        {
            return !directAttack && beAttackedCard == null;
        }

        /// <summary>
        /// 选择我方怪兽将攻击的怪兽
        /// </summary>
        /// <param name=""></param>
        public void ChooseBeAttackedMonster(CardBase monsterCard)
        {
            if (attackCard.GetDuelCardScript().GetOwner() !=
                monsterCard.GetDuelCardScript().GetOwner() &&
                (monsterCard.GetCardGameState() == CardGameState.FrontAttack ||
                monsterCard.GetCardGameState() == CardGameState.FrontDefense ||
                monsterCard.GetCardGameState() == CardGameState.Back) &&
                monsterCard.CanBeAttacked())
            {
                beAttackedCard = monsterCard;
                attackCard.GetDuelCardScript().SetAttackState(false);
                duelScene.SetAttackAnimationFinishEvent(() =>
                {
                    CheckCardCanChainLaunch();
                });
                duelScene.StartPlayAttackAnimation(attackCard.GetDuelCardScript().GetPosition(), beAttackedCard.GetDuelCardScript().GetPosition());
            }
        }

        /// <summary>
        /// 进行攻击
        /// </summary>
        public void Attack()
        {
            //直接攻击
            if(beAttackedCard==null)
            {
                attackCard.Attack();
                ChangeLifeEffectProcess changeLifeEffectProcess = new ChangeLifeEffectProcess(attackCard.GetAttackValue(), ChangeLifeType.Battle, attackCard.GetDuelCardScript().GetOwner().GetOpponentPlayer());
                attackCard.GetDuelCardScript().GetOwner().GetOpponentPlayer().AddEffectProcess(changeLifeEffectProcess);
                return;
            }
            //怪兽间攻击
            attackCard.Attack();
            int card2Value = 0;
            bool card2Defense = false;
            if (beAttackedCard.GetCardGameState() == CardGameState.FrontAttack)
            {
                card2Value = beAttackedCard.GetAttackValue();
            }
            else if (beAttackedCard.GetCardGameState() == CardGameState.FrontDefense ||
                beAttackedCard.GetCardGameState() == CardGameState.Back)
            {
                card2Value = beAttackedCard.GetDefenseValue();
                card2Defense = true;
            }
            int differenceValue = attackCard.GetAttackValue() - card2Value;
            if (differenceValue == 0)
            {
                if (!card2Defense)
                {
                    SendCardToTomb(attackCard, MoveCardToTombType.Battle);
                    SendCardToTomb(beAttackedCard, MoveCardToTombType.Battle);
                }
            }
            else if (differenceValue > 0)
            {
                if (!card2Defense || attackCard.GetCanPenetrateDefense())
                {
                    ChangeLifeEffectProcess changeLifeEffectProcess = new ChangeLifeEffectProcess(differenceValue, ChangeLifeType.Battle, beAttackedCard.GetDuelCardScript().GetOwner());
                    beAttackedCard.GetDuelCardScript().GetOwner().AddEffectProcess(changeLifeEffectProcess);
                }
                SendCardToTomb(beAttackedCard, MoveCardToTombType.Battle);
            }
            else
            {
                ChangeLifeEffectProcess changeLifeEffectProcess = new ChangeLifeEffectProcess(-differenceValue, ChangeLifeType.Battle, attackCard.GetDuelCardScript().GetOwner());
                attackCard.GetDuelCardScript().GetOwner().AddEffectProcess(changeLifeEffectProcess);
                if (!card2Defense)
                {
                    SendCardToTomb(attackCard, MoveCardToTombType.Battle);
                }
            }
        }

        /// <summary>
        /// 将卡牌送入墓地
        /// </summary>
        /// <param name="card"></param>
        /// <param name="moveCardToTombType"></param>
        public void SendCardToTomb(CardBase card, MoveCardToTombType moveCardToTombType)
        {
            MoveCardToTombEffectProcess moveCardToTombEffectProcess = new MoveCardToTombEffectProcess(card, moveCardToTombType, card.GetDuelCardScript().GetOwner());
            card.GetDuelCardScript().GetOwner().AddEffectProcess(moveCardToTombEffectProcess);
        }

        public override void Stop()
        {
            base.Stop();
            AfterFinishProcessFunction();
        }

        protected override void RealProcessFunction()
        {
            if(directAttack)
            {
                ownerPlayer.GetOpponentPlayer().BeDirectAttackedNotify(attackCard.GetID());
            }
            else
            {
                ownerPlayer.GetOpponentPlayer().BeAttackedMonsterNotify(attackCard.GetID(), beAttackedCard.GetID());
            }
            Attack();
            AfterFinishProcessFunction();
        }
    }
}
