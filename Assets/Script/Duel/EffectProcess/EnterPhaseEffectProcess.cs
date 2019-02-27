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
            string ex = duelScene.currentPlayer == duelScene.myPlayer ? "我方进入" : "对方进入";

            switch (phaseType)
            {
                case PhaseType.Unknown:
                    GameManager.ShowMessage(ex + "未知流程！");
                    break;
                case PhaseType.Draw:
                    GameManager.ShowMessage(ex + "抽牌流程！");
                    break;
                case PhaseType.Prepare:
                    GameManager.ShowMessage(ex + "准备流程！");
                    break;
                case PhaseType.Main:
                    GameManager.ShowMessage(ex + "主要流程！");
                    BeginMainPhaseTypeEvent();
                    break;
                case PhaseType.Battle:
                    GameManager.ShowMessage(ex + "战斗流程！");
                    BeginBattlePhaseTypeEvent();
                    break;
                case PhaseType.Second:
                    GameManager.ShowMessage(ex + "第二主要流程！");
                    break;
                case PhaseType.End:
                    GameManager.ShowMessage(ex + "结束流程！");
                    break;
                default:
                    break;
            }
            duelScene.currentPlayer.GetOpponentPlayer().EnterPhaseNotify(phaseType);
            duelScene.startPlayer.CheckAllCardEffect();
            duelScene.startPlayer.GetOpponentPlayer().CheckAllCardEffect();
            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                AfterFinishProcessFunction();
            });
            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 开始主要流程事件
        /// </summary>
        void BeginMainPhaseTypeEvent()
        {
            duelScene.currentPlayer.CheckAndSetAllMonsterChangeAttackOrDefenseNumber();
        }

        /// <summary>
        /// 开始战斗流程事件
        /// </summary>
        void BeginBattlePhaseTypeEvent()
        {
            duelScene.currentPlayer.CheckAndShowAllMonsterCanAttack();
        }

        /// <summary>
        /// 结束战斗流程事件
        /// </summary>
        void EndBattlePhaseTypeEvent()
        {
            duelScene.currentPlayer.ClearAllMonsterCanAttack();
        }
    }
}
