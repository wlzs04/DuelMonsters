using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using XLua;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 效果处理类型
    /// </summary>
    public enum EffectProcessType
    {
        RemoveAfterFinish,//在完成后进行删除
        Forever,//永远
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
        protected UnityAction finishAction;
        protected EffectProcessType effectProcessType=EffectProcessType.RemoveAfterFinish;
        protected Player ownerPlayer;
        protected DuelScene duelScene;
        protected bool haveProcess = false;//是否已经执行

        public EffectProcessBase(Player ownerPlayer)
        {
            this.ownerPlayer = ownerPlayer;
            duelScene = GameManager.GetDuelScene();
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
        public void AfterFinishProcessFunction()
        {
            haveProcess = false;
            if (effectProcessType== EffectProcessType.RemoveAfterFinish)
            {
                ownerPlayer.RemoveEffectProcess(this);
            }
            else
            {
                ownerPlayer.RemoveCurrentEffectProcess(this);
            }
            if (finishAction!=null)
            {
                finishAction();
            }
        }

        /// <summary>
        /// 当前效果处理被中断或停止
        /// </summary>
        public virtual void Stop()
        {

        }

        public void SetEffectProcessType(EffectProcessType effectProcessType)
        {
            this.effectProcessType = effectProcessType;
        }
    }
}
