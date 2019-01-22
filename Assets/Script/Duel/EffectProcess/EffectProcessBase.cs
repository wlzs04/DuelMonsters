using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 效果处理类型
    /// </summary>
    public enum EffectProcessType
    {
        Unknown,//未知
        Forever,//永远
        RemoveAfterFinish,//在完成后进行删除
    }

    /// <summary>
    /// 效果是否触发的判断方法
    /// </summary>
    /// <returns></returns>
    public delegate bool EffectTriggerFunction();

    /// <summary>
    /// 效果处理基类：用于向场景、玩家或卡牌添加效果处理，当满足条件时触发特殊效果
    /// </summary>
    public abstract class EffectProcessBase
    {
        public UnityAction finishAction;
        protected EffectProcessType effectProcessType;
        protected Player ownerPlayer;
        protected DuelScene duelScene;
        protected bool haveProcess = false;//是否已经执行

        public EffectProcessBase(Player ownerPlayer, UnityAction finishAction=null)
        {
            this.ownerPlayer = ownerPlayer;
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
            this.finishAction = finishAction;
        }

        public virtual void Update()
        {

        }

        /// <summary>
        /// 检查当前效果是否可以触发
        /// </summary>
        /// <returns></returns>
        public abstract bool CheckCanTrigger();

        /// <summary>
        /// 执行
        /// </summary>
        public void Process()
        {
            if(CheckCanTrigger())
            {
                ownerPlayer.SetCurrentEffectProcess(this);
                ProcessFunction();
            }
        }

        /// <summary>
        /// 需要具体执行的方法
        /// </summary>
        protected abstract void ProcessFunction();

        /// <summary>
        /// 在完成执行方法后的处理
        /// </summary>
        protected void AfterFinishProcessFunction()
        {
            haveProcess = false;
            if (effectProcessType== EffectProcessType.RemoveAfterFinish)
            {
                ownerPlayer.RemoveEffectProcess(this);
            }
            if(finishAction!=null)
            {
                finishAction();
            }
        }
    }
}
