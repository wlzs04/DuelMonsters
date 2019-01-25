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
        MonsterCard attackCard = null;
        MonsterCard beAttackedCard;
        bool directAttack = false;

        public AttackEffectProcess(MonsterCard attackCard, MonsterCard beAttackedCard, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            //如果对方没有可以被攻击的怪兽，则进行直接攻击
            if(!ownerPlayer.GetOpponentPlayer().HaveBeAttackedMonster())
            {
                directAttack = true;
            }
            this.attackCard = attackCard;
            this.beAttackedCard = beAttackedCard;

            finishAction = () => { attackCard.GetDuelCardScript().ClearPrepareAttackState();};
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void ProcessFunction()
        {
            if (directAttack)
            {
                attackCard.GetDuelCardScript().Attack();
                duelScene.SetAttackAnimationFinishEvent(() =>
                {
                    ownerPlayer.GetOpponentPlayer().BeDirectAttackedNotify(attackCard.GetID());

                    ReduceLifeEffectProcess reduceLifeEffectProcess = new ReduceLifeEffectProcess(attackCard.GetAttackNumber(), ReduceLifeType.Battle, attackCard.GetDuelCardScript().GetOwner().GetOpponentPlayer());
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
        public void ChooseBeAttackedMonster(MonsterCard monsterCard)
        {
            if (attackCard.GetDuelCardScript().GetOwner() !=
                monsterCard.GetDuelCardScript().GetOwner() &&
                (monsterCard.GetCardGameState() == CardGameState.FrontAttack ||
                monsterCard.GetCardGameState() == CardGameState.FrontDefense ||
                monsterCard.GetCardGameState() == CardGameState.Back) &&
                monsterCard.CanBeAttacked)
            {
                beAttackedCard = monsterCard;
                duelScene.SetAttackAnimationFinishEvent(() =>
                {
                    beAttackedCard.GetDuelCardScript().GetOwner().BeAttackedMonsterNotify(attackCard.GetID(), beAttackedCard.GetID());
                    duelScene.AttackMonster(attackCard, beAttackedCard);
                    AfterFinishProcessFunction();
                });
                duelScene.StartPlayAttackAnimation(attackCard.GetDuelCardScript().GetPosition(), beAttackedCard.GetDuelCardScript().GetPosition());
            }
        }

        public override void Stop()
        {
            base.Stop();
            AfterFinishProcessFunction();
        }
    }
}
