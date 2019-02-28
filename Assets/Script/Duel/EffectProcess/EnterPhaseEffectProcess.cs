using Assets.Script.Config;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 进入阶段
    /// </summary>
    class EnterPhaseEffectProcess : EffectProcessBase
    {
        PhaseType phaseType;
        public EnterPhaseEffectProcess(PhaseType phaseType,Player ownerPlayer) : base(ownerPlayer, "进入阶段")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.phaseType = phaseType;
            finishAction = () =>
            {
                duelScene.ResetCurrentPointCardAllowedOperation();
                duelScene.CheckAllEffectProcess();
            };
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            if (duelScene.GetCurrentPhaseType() == phaseType)
            {
                Debug.LogError("当前流程已经是：" + phaseType + "无法重复进入！");
                return;
            }
            switch (duelScene.GetCurrentPhaseType())
            {
                case PhaseType.Unknown:
                    break;
                case PhaseType.Draw:
                    break;
                case PhaseType.Prepare:
                    break;
                case PhaseType.Main:
                    break;
                case PhaseType.Battle:
                    EndBattlePhaseTypeEvent();
                    break;
                case PhaseType.Second:
                    break;
                case PhaseType.End:
                    break;
                default:
                    break;
            }
            duelScene.SetCurrentPhaseType(phaseType);
            duelScene.ResetPhaseTypePanelInfo();
            string ex = duelScene.GetCurrentPlayer() == duelScene.GetMyPlayer() ? "我方进入" : "对方进入";
            PhaseTypeConfig phaseTypeConfig = ConfigManager.GetConfigByName("PhaseType") as PhaseTypeConfig;
            GameManager.ShowMessage(ex + phaseTypeConfig.GetRecordById((int)phaseType).value + "流程！");
            duelScene.GetCurrentPlayer().GetOpponentPlayer().EnterPhaseNotify(phaseType);
            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                DoProcessFunction();
            });
            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 开始抽卡流程事件
        /// </summary>
        void BeginDrawPhaseTypeEvent()
        {
            beInterrupted = true;
            DrawCardEffectProcess drawCardEffectProcess = new DrawCardEffectProcess(1, DrawCardType.EveryTurn, duelScene.GetCurrentPlayer());
            duelScene.GetCurrentPlayer().AddEffectProcess(drawCardEffectProcess);
        }

        /// <summary>
        /// 开始主要流程事件
        /// </summary>
        void BeginMainPhaseTypeEvent()
        {
            duelScene.GetCurrentPlayer().CheckAndSetAllMonsterChangeAttackOrDefenseNumber();
        }

        /// <summary>
        /// 开始战斗流程事件
        /// </summary>
        void BeginBattlePhaseTypeEvent()
        {
            duelScene.GetCurrentPlayer().CheckAndShowAllMonsterCanAttack();
        }

        /// <summary>
        /// 结束战斗流程事件
        /// </summary>
        void EndBattlePhaseTypeEvent()
        {
            duelScene.GetCurrentPlayer().ClearAllMonsterCanAttack();
        }

        /// <summary>
        /// 开始结束流程事件
        /// </summary>
        void BeginEndPhaseTypeEvent()
        {
            //在回合结束后检查手牌是否超过规定的最大数量，超过的话丢弃多余数量
            if (duelScene.GetCurrentPlayer().GetHandCards().Count > DuelRuleManager.GetHandCardNumberUpperLimit())
            {
                GameManager.ShowMessage("当前手牌数量大于规定数量！");
                beInterrupted = true;
                int number = duelScene.GetCurrentPlayer().GetHandCards().Count - DuelRuleManager.GetHandCardNumberUpperLimit();
                DiscardHandCardEffectProcess discardHandCardEffectProcess = new DiscardHandCardEffectProcess(null, number, null, duelScene.GetCurrentPlayer());
                duelScene.GetCurrentPlayer().AddEffectProcess(discardHandCardEffectProcess);
            }
            finishAction += duelScene.ChangeCurrentPlayer;
        }

        /// <summary>
        /// 做一些进入阶段的特殊处理
        /// </summary>
        void DoProcessFunction()
        {
            switch (phaseType)
            {
                case PhaseType.Unknown:
                    break;
                case PhaseType.Draw:
                    BeginDrawPhaseTypeEvent();
                    break;
                case PhaseType.Prepare:
                    break;
                case PhaseType.Main:
                    BeginMainPhaseTypeEvent();
                    break;
                case PhaseType.Battle:
                    BeginBattlePhaseTypeEvent();
                    break;
                case PhaseType.Second:
                    break;
                case PhaseType.End:
                    BeginEndPhaseTypeEvent();
                    break;
                default:
                    break;
            }
            if(!beInterrupted)
            {
                RealProcessFunction();
            }
        }

        protected override void RealProcessFunction()
        {
            duelScene.GetStartPlayer().CheckAllCardEffect();
            duelScene.GetStartPlayer().GetOpponentPlayer().CheckAllCardEffect();
            AfterFinishProcessFunction();
        }
    }
}
