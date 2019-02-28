using Assets.Script.Card;
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
        protected bool haveFinish = false;//是否已经执行完成

        protected bool beDisabled = false;//是否被禁止
        protected bool canBeChained = false;//是否可以被连锁
        protected bool canChain = true;//是否可以进入连锁
        protected bool beInterrupted = false;//是否被中断
        protected string effectName = "未命名";//效果的名称


        public EffectProcessBase(Player ownerPlayer,string effectName)
        {
            this.ownerPlayer = ownerPlayer;
            this.effectName = effectName;
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

        public Player GetOwnPlayer()
        {
            return ownerPlayer;
        }

        /// <summary>
        /// 继续执行，一般在连锁中让中断了的效果继续执行
        /// </summary>
        public void ContinueProcess()
        {
            RealProcessFunction();
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Process()
        {
            if(CheckCanTrigger())
            {
                ownerPlayer.SetCurrentEffectProcess(this);
                duelScene.AddEffectProcessChain(this);
                BeforeProcessFunction();
            }
        }

        /// <summary>
        /// 在执行具体方法前的处理
        /// </summary>
        protected abstract void BeforeProcessFunction();

        /// <summary>
        /// 需要具体执行的方法
        /// </summary>
        protected virtual void RealProcessFunction()
        {

        }

        /// <summary>
        /// 在完成执行方法后的处理
        /// </summary>
        public void AfterFinishProcessFunction()
        {
            haveProcess = false;
            if (effectProcessType == EffectProcessType.RemoveAfterFinish)
            {
                ownerPlayer.RemoveEffectProcess(this);
            }
            else
            {
                ownerPlayer.RemoveCurrentEffectProcess(this);
            }
            finishAction?.Invoke();
            haveFinish = true;
        }

        /// <summary>
        /// 当前效果处理被中断或停止
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// 检测是否存在卡牌可以连锁发动效果
        /// </summary>
        /// <returns></returns>
        protected void CheckCardCanChainLaunch()
        {
            Player chainPlayer = duelScene.CheckCardCanChainLaunch();
            if (chainPlayer!=null && chainPlayer==duelScene.GetMyPlayer())
            {
                string titleText = $"在效果：{effectName}中，是否发动卡牌效果。";
                duelScene.ShowMakeSurePanel(titleText, ChooseCanChainCard, RealProcessFunction);
            }
            else
            {
                RealProcessFunction();
            }
        }

        /// <summary>
        /// 选择可以进行连锁的卡牌
        /// </summary>
        void ChooseCanChainCard()
        {
            beInterrupted = true;
            ChooseCardEffectProcess chooseCardEffectProcess = new ChooseCardEffectProcess(null, ChooseCardJudgeAction, ChooseCardCallback, duelScene.GetMyPlayer());
            chooseCardEffectProcess.SetTitle("请选择发动的卡牌！");
            duelScene.GetMyPlayer().AddEffectProcess(chooseCardEffectProcess);
        }

        /// <summary>
        /// 选择条件
        /// </summary>
        bool ChooseCardJudgeAction(CardBase launchCardBase, CardBase chooseCard)
        {
            return chooseCard.CanLaunchEffect();
        }

        /// <summary>
        /// 选择后回调
        /// </summary>
        void ChooseCardCallback(CardBase launchCardBase, CardBase chooseCard)
        {
            chooseCard.LaunchEffect();
        }

        public bool GetHaveFinish()
        {
            return haveFinish;
        }

        public bool GetBeInterrupted()
        {
            return beInterrupted;
        }

        public bool GetCanChain()
        {
            return canChain;
        }

        public void SetBeDisabled(bool beDisabled)
        {
            this.beDisabled = beDisabled;
        }
    }
}
