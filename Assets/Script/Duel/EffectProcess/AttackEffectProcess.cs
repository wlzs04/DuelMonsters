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

        public AttackEffectProcess(CardBase attackCard, CardBase beAttackedCard, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            //如果对方没有可以被攻击的怪兽，则进行直接攻击
            if(!ownerPlayer.GetOpponentPlayer().HaveBeAttackedMonster())
            {
                directAttack = true;
            }
            this.attackCard = attackCard;
            this.beAttackedCard = beAttackedCard;

            finishAction = () => { attackCard.GetDuelCardScript().ClearPrepareAttackState(); ownerPlayer.ThinkAction(); };
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void ProcessFunction()
        {
            haveProcess = true;
            if (directAttack)
            {
                attackCard.GetDuelCardScript().SetAttackState(false);
                duelScene.SetAttackAnimationFinishEvent(() =>
                {
                    ownerPlayer.GetOpponentPlayer().BeDirectAttackedNotify(attackCard.GetID());

                    attackCard.Attack();
                    ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(attackCard.GetAttackValue(), ReduceLifeType.Battle, attackCard.GetDuelCardScript().GetOwner().GetOpponentPlayer());
                    attackCard.GetDuelCardScript().GetOwner().GetOpponentPlayer().AddEffectProcess(reduceLifeEffectProcess);

                    AfterFinishProcessFunction();
                });
                duelScene.StartPlayAttackAnimation(attackCard.GetDuelCardScript().GetPosition(), ownerPlayer.GetOpponentPlayer().GetHeartPosition());
            }
            else if (beAttackedCard == null)
            {
                GameManager.ShowMessage("请选择将被攻击的怪兽");
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
                    beAttackedCard.GetDuelCardScript().GetOwner().BeAttackedMonsterNotify(attackCard.GetID(), beAttackedCard.GetID());
                    Attack();
                    AfterFinishProcessFunction();
                });
                duelScene.StartPlayAttackAnimation(attackCard.GetDuelCardScript().GetPosition(), beAttackedCard.GetDuelCardScript().GetPosition());
            }
        }

        /// <summary>
        /// 进行攻击
        /// </summary>
        public void Attack()
        {
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
                    ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(differenceValue, ReduceLifeType.Battle, beAttackedCard.GetDuelCardScript().GetOwner());
                    beAttackedCard.GetDuelCardScript().GetOwner().AddEffectProcess(reduceLifeEffectProcess);
                }
                SendCardToTomb(beAttackedCard, MoveCardToTombType.Battle);
            }
            else
            {
                ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(-differenceValue, ReduceLifeType.Battle, attackCard.GetDuelCardScript().GetOwner());
                attackCard.GetDuelCardScript().GetOwner().AddEffectProcess(reduceLifeEffectProcess);
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
    }
}
